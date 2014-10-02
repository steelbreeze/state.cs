/* State v5 finite state machine library
 * Copyright (c) 2014 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// A Vertex is an abstraction of a node in a state machine graph; it can be the source or destination of any number of transitions.
	/// </summary>
	/// <typeparam name="TContext">The type of the state machine instance.</typeparam>
	public abstract class Vertex<TContext> : StateMachineElement<TContext> where TContext : IContext<TContext> {
		internal readonly Region<TContext> Region;
		internal readonly StateMachine<TContext> Root;
		internal Boolean IsFinal { get { return this.transitions == null; } }

		private Transition<TContext>[] transitions; // trading off model building performance for runtime performance
		private readonly Func<Transition<TContext>[], TContext, Object, Transition<TContext>> selector;

		/// <summary>
		/// Returns the Vertex's parent element.
		/// </summary>
		public override StateMachineElement<TContext> Parent { get { return this.Region; } }

		internal Vertex( String name, Region<TContext> parent, Func<Transition<TContext>[], TContext, Object, Transition<TContext>> selector )
			: base( name, parent ) {			
			this.Root = parent.Root;
			this.Region = parent;
			this.selector = selector;

			parent.Add( this );
		}

		/// <summary>
		/// Creates a new transition from this Vertex.
		/// </summary>
		/// <param name="target">The Vertex to transition to.</param>
		/// <returns>An intance of the Transition class.</returns>
		/// <remarks>
		/// To specify an internal transition, specify a null target.
		/// </remarks>
		public virtual Transition<TContext> To( Vertex<TContext> target ) {
			var transition = new Transition<TContext>( this, target );

			if( this.transitions == null )
				this.transitions = new Transition<TContext>[ 1 ] { transition };
			else {
				var transitions = new Transition<TContext>[ this.transitions.Length + 1 ];

				this.transitions.CopyTo( transitions, 0 );

				transitions[ this.transitions.Length ] = transition;

				this.transitions = transitions;
			}

			this.Root.Clean = false;

			return transition;
		}

		internal override void BootstrapElement( Boolean deepHistoryAbove ) {
			base.BootstrapElement( deepHistoryAbove );

			this.EndEnter += this.EvaluateCompletions;
			this.Enter = this.BeginEnter + this.EndEnter;
		}

		internal override void BootstrapTransitions() {
			if( this.transitions != null )
				foreach( var transition in this.transitions )
					transition.BootstrapTransitions();
		}

		internal void EvaluateCompletions( TContext context, Object message, Boolean history ) {
			if( this.IsComplete( context ) )
				this.Evaluate( context, this );
		}

		internal virtual Boolean IsComplete( TContext context ) {
			return true;
		}

		internal virtual Boolean Evaluate( TContext context, Object message ) {
			var transition = this.selector( this.transitions, context, message );

			if( transition == null )
				return false;

			transition.Traverse( context, message, false );

			return true;

		}
	}
}
