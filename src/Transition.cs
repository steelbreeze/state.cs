/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// A Transition describes a valid path from one Vertex in a state machine to another, and the trigger that will cause it to be followed.
	/// </summary>
	/// <typeparam name="TContext">The type of the state machine instance.</typeparam>
	/// <remarks>
	/// There are two types of transition, completion transitions and message based transitions.
	/// Completion transitions are evaluated when a vertex has been entered and is deemed to be complete; this is the default for newly created transitions.
	/// Message based transitions have an additional guard condition that a message (event) and the current state machine context will be evaluated against; this is defined by the Transition.When method thereby turning a completion transition into a message based transition.
	/// </remarks>
	public class Transition<TContext> where TContext : IContext<TContext> {
		#region Static members
		private static Func<Object, TContext, Boolean> IsElse = ( message, context ) => false;

		internal static Func<Transition<TContext>[], Object, TContext, Transition<TContext>> PseudoState( PseudoStateKind kind ) {
			switch( kind ) {
				case PseudoStateKind.Initial:
				case PseudoStateKind.DeepHistory:
				case PseudoStateKind.ShallowHistory:
					return Transition<TContext>.Initial;

				case PseudoStateKind.Junction:
					return Transition<TContext>.Junction;
				
				case PseudoStateKind.Choice:
					return Transition<TContext>.Choice;
	
				case PseudoStateKind.Terminate:
					return Transition<TContext>.Terminate;

				default: // NOTE: all PseudoStateKinds dealt with above so should not be an issue
					return null;
			}
		}

		internal static Transition<TContext> State( Transition<TContext>[] transitions, Object message, TContext context ) {
			Transition<TContext> result = null;

			if( transitions != null ) {
				for( int i = 0, l = transitions.Length; i < l; ++i ) {
					if( transitions[ i ].Predicate( message, context ) ) {
						if( result != null )
							throw new InvalidOperationException( "Multiple outbound transitions evaluated true" );

						result = transitions[ i ];
					}
				}
			}

			return result;
		}

		private static Transition<TContext> Initial( Transition<TContext>[] transitions, Object message, TContext context ) {
			if( transitions.Length == 1 )
				return transitions[ 0 ];
			else
				throw new InvalidOperationException( "Initial transition must have a single outbound transition" );
		}

		private static Transition<TContext> Junction( Transition<TContext>[] transitions, Object message, TContext context ) {
			return transitions.SingleOrDefault( t => t.Predicate( message, context ) ) ?? transitions.Single( transition => transition.Predicate.Equals( Transition<TContext>.IsElse ) );
		}

		private static readonly Random random = new Random();

		private static Transition<TContext> Choice( Transition<TContext>[] transitions, Object message, TContext context ) {
			var transition = default( Transition<TContext> );
			var items = transitions.Where( t => t.Predicate( message, context ) );
			var count = items.Count();

			if( count == 1 )
				transition = items.First();

			else if( count > 1 )
				transition = items.ElementAt( random.Next( count ) );

			return transition ?? transitions.Single( t => t.Predicate.Equals( Transition<TContext>.IsElse ) );
		}

		internal static Transition<TContext> Terminate( Transition<TContext>[] transitions, Object message, TContext context ) {
			return null;
		}
		#endregion
		internal readonly Vertex<TContext> Source;
		internal readonly Vertex<TContext> Target;
		internal Func<Object, TContext, Boolean> Predicate;
		internal Action<Object, TContext, Boolean> Traverse;

		private event Action<Object, TContext> effect;

		internal Transition( Vertex<TContext> source, Vertex<TContext> target ) {
			Trace.Assert( source != null, "Transitions must have a source Vertex" );

			this.Source = source;
			this.Target = target;

			this.Completion();
		}

		/// <summary>
		/// Adds a typed guard condition to a transition that can evaluate both the state machine context and the triggering message.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that can trigger the transition.</typeparam>
		/// <param name="guard">The guard condition taking both the state machine context and the message that must evaluate true for the transition to be traversed.</param>
		/// <returns>Returns the transition.</returns>
		public Transition<TContext> When<TMessage>( Func<TMessage, TContext, Boolean> guard ) where TMessage : class {
			this.Predicate = ( message, context ) => message is TMessage && guard( message as TMessage, context );

			return this;
		}

		/// <summary>
		/// Adds a typed guard condition to a transition that can evaluate the triggering message.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that can trigger the transition.</typeparam>
		/// <param name="guard">A guard condition taking the message that must evaluate true for the transition to be traversed.</param>
		/// <returns>Returns the transition.</returns>
		public Transition<TContext> When<TMessage>( Func<TMessage, Boolean> guard ) where TMessage : class {
			this.Predicate = ( message, context ) => message is TMessage && guard( message as TMessage );

			return this;
		}

		/// <summary>
		/// Adds a guard condition to a transition that can evaluate the triggering message.
		/// </summary>
		/// <param name="guard">A guard condition, taking just the state machine context, that must evaluate true for the transition to be traversed.</param>
		/// <returns>Returns the transition.</returns>
		public Transition<TContext> When( Func<TContext, Boolean> guard ) {
			this.Predicate = ( message, context ) => guard( context );

			return this;
		}

		/// <summary>
		/// Turns a transition into a completion transition
		/// </summary>
		/// <returns>Returns the transition.</returns>
		/// <remarks>
		/// All transitions are completion tansitions when initially created; this method can be used to return a transition to be a completion transition if prior calls to When or Else have been made.
		/// </remarks>
		public Transition<TContext> Completion() {
			this.Predicate = ( message, context ) => message == this.Source;

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
		public Transition<TContext> Effect<TMessage>( params Action<TMessage, TContext>[] behavior ) where TMessage : class {
			foreach( var effect in behavior )
				this.effect += ( message, context ) => { if( message is TMessage ) effect( message as TMessage, context ); };

			this.Source.Root.Clean = false;
			
			return this;
		}

		/// <summary>
		/// Adds behavior to a transition.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that triggered the transition.</typeparam>
		/// <param name="behavior">An Action that takes the triggering message as a parameter.</param>
		/// <returns>Returns the transition.</returns>
		/// <remarks>
		/// If the type of the message that triggers the transition does not match TMessage, the behavior will not be called.
		/// </remarks>
		public Transition<TContext> Effect<TMessage>( params Action<TMessage>[] behavior ) where TMessage : class {
			foreach( var effect in behavior )
				this.effect += ( message, context ) => { if( message is TMessage ) effect( message as TMessage ); };

			this.Source.Root.Clean = false;
			
			return this;
		}

		/// <summary>
		/// Adds behavior to a transition.
		/// </summary>
		/// <param name="behavior">An Action that takes the state machine context as a parameter.</param>
		/// <returns>Returns the transition.</returns>
		public Transition<TContext> Effect( params Action<TContext>[] behavior ) {
			foreach( var effect in behavior )
				this.effect += ( message, context ) => effect( context );

			this.Source.Root.Clean = false;
			
			return this;
		}

		/// <summary>
		/// Adds behavior to a transition.
		/// </summary>
		/// <param name="behavior">An Action that takes no parameters.</param>
		/// <returns>Returns the transition.</returns>
		public Transition<TContext> Effect( params Action[] behavior ) {
			foreach( var effect in behavior )
				this.effect += ( message, context ) => effect();

			this.Source.Root.Clean = false;
		
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
		protected void OnEffect( Object message, TContext context, Boolean history ) {
			this.effect( message, context );
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
				var sourceAncestors = this.Source.Ancestors;
				var targetAncestors = this.Target.Ancestors;
				var sourceAncestorsCount = sourceAncestors.Count();
				var targetAncestorsCount = targetAncestors.Count();
				int i = 0, l = Math.Min( sourceAncestorsCount, sourceAncestorsCount );

				// find the index of the first uncommon ancestor
				while( ( i < l ) && sourceAncestors.ElementAt( i ) == targetAncestors.ElementAt( i ) ) ++i;

				// validation rule (not in the UML spec currently)
				Trace.Assert( sourceAncestors.ElementAt( i ) is Region<TContext> == false, "Transitions may not cross sibling orthogonal regions" );

				// leave the first uncommon ancestor
				this.Traverse = ( i < sourceAncestorsCount ? sourceAncestors.ElementAt( i ) : this.Source ).Leave;

				// perform the transition effect
				if( this.effect != null )
					this.Traverse += this.OnEffect;

				// edge case when transitioning to a state in the vertex ancestry
				if( i >= targetAncestorsCount )
					this.Traverse += this.Target.BeginEnter;

				// enter the target ancestry
				while( i < targetAncestorsCount )
					targetAncestors.ElementAt( i++ ).BootstrapEnter( ref this.Traverse, i < targetAncestorsCount ? targetAncestors.ElementAt( i ) : null );

				// trigger cascade
				this.Traverse += this.Target.EndEnter;
			}
		}
	}
}
