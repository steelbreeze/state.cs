/* state v5 finite state machine library
 * Copyright (c) 2014 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// A Transition describes a vaild path from one Vertex in a state machine to another, and the trigger that will cause it to be followed.
	/// </summary>
	/// <typeparam name="TContext">The type of the state machine instance.</typeparam>
	/// <remarks>
	/// There are two types of transition, completion transitions and message based transitions.
	/// Completion transitions are evaluated when a vertex has been entered and is deemed to be complete; this is the default for newly created transitions.
	/// Message based transitions have an additional guard condition that a message (event) and the current state machine context will be evaluated against; this is defined by the Transition.When method thereby turning a completion transition into a message based transition.
	/// </remarks>
	public class Transition<TContext> where TContext : IContext<TContext> {
		#region Static members
		private static Func<TContext, Object, Boolean> IsElse = ( context, message ) => false;

		internal static Func<Transition<TContext>[], TContext, Object, Transition<TContext>> PseudoState( PseudoStateKind kind ) {
			switch( kind ) {
				case PseudoStateKind.Initial:
				case PseudoStateKind.DeepHistory:
				case PseudoStateKind.ShallowHistory:
					return Transition<TContext>.Initial;

				case PseudoStateKind.Junction:
					return Transition<TContext>.Junction;
				case PseudoStateKind.Choice:
					return Transition<TContext>.Choice;
				default:
					return Transition<TContext>.Null;
			}
		}

		internal static Transition<TContext> State( Transition<TContext>[] transitions, TContext context, object message ) {
			Transition<TContext> result = null;

			if( transitions != null ) {
				for( int i = 0, l = transitions.Length; i < l; ++i ) {
					if( transitions[ i ].Predicate( context, message ) ) {
						if( result != null )
							throw new InvalidOperationException( "Multiple outbound transitions evaluated true" );

						result = transitions[ i ];
					}
				}
			}

			return result;
		}

		private static Transition<TContext> Initial( Transition<TContext>[] transitions, TContext context, Object message ) {
			if( transitions.Length == 1 )
				return transitions[ 0 ];
			else
				throw new InvalidOperationException( "Initial transition must have a single outbound transition" );
		}

		private static Transition<TContext> Junction( Transition<TContext>[] transitions, TContext context, Object message ) {
			return transitions.SingleOrDefault( t => t.Predicate( context, message ) ) ?? transitions.Single( transition => transition.Predicate.Equals( Transition<TContext>.IsElse ) );
		}

		private static readonly Random random = new Random();

		private static Transition<TContext> Choice( Transition<TContext>[] transitions, TContext context, Object message ) {
			var transition = default( Transition<TContext> );
			var items = transitions.Where( t => t.Predicate( context, message ) );
			var count = items.Count();

			if( count == 1 )
				transition = items.First();

			else if( count > 1 )
				transition = items.ElementAt( random.Next( count ) );

			return transition ?? transitions.Single( t => t.Predicate.Equals( Transition<TContext>.IsElse ) );
		}

		internal static Transition<TContext> Null( Transition<TContext>[] transitions, TContext context, Object message ) {
			return null;
		}
		#endregion
		internal readonly Vertex<TContext> Source;
		internal readonly Vertex<TContext> Target;
		internal Func<TContext, Object, Boolean> Predicate;
		internal Action<TContext, Object, Boolean> Traverse;

		private event Action<TContext, Object> effect;

		internal Transition( Vertex<TContext> source, Vertex<TContext> target ) {
			Trace.Assert( source != null, "Transitions must have a source Vertex" );

			this.Source = source;
			this.Target = target;
			this.Predicate = ( context, message ) => message == this.Source;
		}

		/// <summary>
		/// Adds a typed guard condition to a transition.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that can trigger the transition.</typeparam>
		/// <param name="guard">The guard condition that must evaluate true for the transition to be traversed.</param>
		/// <returns>Returns the transition.</returns>
		public Transition<TContext> When<TMessage>( Func<TContext, TMessage, Boolean> guard ) where TMessage : class {
			this.Predicate = ( context, message ) => message is TMessage && guard( context, message as TMessage );

			return this;
		}

		/// <summary>
		/// Creates an else transition for use with Choice and Junction PseudoStates.
		/// </summary>
		/// <returns>Returns the transition.</returns>
		public Transition<TContext> Else() {
			Trace.Assert( this.Source is PseudoState<TContext> && ( ( this.Source as PseudoState<TContext> ).Kind == PseudoStateKind.Choice || ( this.Source as PseudoState<TContext> ).Kind == PseudoStateKind.Junction ), "Else is only allowed for transitions from Choice or Junction PseudoStates" );

			this.Predicate = Transition<TContext>.IsElse;

			return this;
		}

		/// <summary>
		/// Adds behavior to a transition.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that triggered the transition.</typeparam>
		/// <param name="behavior">An Action that takes the state machine context and triggering messages as parameters.</param>
		/// <returns>Returns the transition.</returns>
		/// <remarks>
		/// If the type of the message that triggers the transition does not match TMessage, the behavior will not be called.
		/// </remarks>
		public Transition<TContext> Do<TMessage>( params Action<TContext, TMessage>[] behavior ) where TMessage : class {
			foreach( var effect in behavior )
				this.effect += ( context, message ) => { if( message is TMessage ) effect( context, message as TMessage ); };

			return this;
		}

		/// <summary>
		/// Adds behavior to a transition.
		/// </summary>
		/// <param name="behavior">An Action that takes the state machine context as a parameter.</param>
		/// <returns>Returns the transition.</returns>
		public Transition<TContext> Do( params Action<TContext>[] behavior ) {
			foreach( var effect in behavior )
				this.effect += ( context, message ) => effect( context );

			return this;
		}

		/// <summary>
		/// Adds behavior to a transition.
		/// </summary>
		/// <param name="behavior">An Action that takes no parameters.</param>
		/// <returns>Returns the transition.</returns>
		public Transition<TContext> Do( params Action[] behavior ) {
			foreach( var effect in behavior )
				this.effect += ( context, message ) => effect();

			return this;
		}

		/// <summary>
		/// Invokes the transition behavior upon traversing a transition.
		/// </summary>
		/// <param name="context">The state machine context.</param>
		/// <param name="message">The message that triggered the state transition.</param>
		/// <param name="history">A flag denoting if history semantics were in play during the transition.</param>
		/// <remarks>
		/// For completion transitions, the message is the source vertex that was completed.
		/// </remarks>
		protected void OnEffect( TContext context, Object message, Boolean history ) {
			this.effect( context, message );
		}

		internal void BootstrapTransitions() {
			// reset the traverse operation to cater for re-initialisation
			this.Traverse = null;

			// internal transitions
			if( this.Target == null ) {
				// just perform the transition effect; no actual transition
				if( this.effect != null )
					this.Traverse += this.OnEffect;

				// local transitions
			} else if( this.Target.Region == this.Source.Region ) {
				// leave the source
				this.Traverse += this.Source.Leave;

				// perform the transition effect
				if( this.effect != null )
					this.Traverse += this.OnEffect;

				// enter the target
				this.Traverse += this.Target.Enter;

				// complex (external) transitions
			} else {
				int i = 0, l = Math.Min( this.Source.Ancestors.Count(), this.Source.Ancestors.Count() );

				// find the index of the first uncommon ancestor
				while( ( i < l ) && this.Source.Ancestors.ElementAt( i ) == this.Target.Ancestors.ElementAt( i ) ) ++i;

				// validation rule (not in hte UML spec currently)
				Trace.Assert( this.Source.Ancestors.ElementAt( i ) is Region<TContext> == false, "Transitions may not cross sibling orthogonal regions" );

				// leave the first uncommon ancestor
				this.Traverse = ( i < this.Source.Ancestors.Count() ? this.Source.Ancestors.ElementAt( i ) : this.Source ).Leave;

				// perform the transition effect
				if( this.effect != null )
					this.Traverse += this.OnEffect;

				// edge case when transitioning to a state in the vertex ancestry
				if( i >= this.Target.Ancestors.Count() )
					this.Traverse += this.Target.BeginEnter;

				// enter the target ancestry
				while( i < this.Target.Ancestors.Count() )
					this.Target.Ancestors.ElementAt( i++ ).BootstrapEnter( ref this.Traverse, i < this.Target.Ancestors.Count() ? this.Target.Ancestors.ElementAt( i ) : null );

				// trigger cascade
				this.Traverse += this.Target.EndEnter;
			}
		}
	}
}
