/* State v5 finite state machine library
 * Copyright (c) 2014 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// A state models a situation during which some (usually implicit) invariant condition holds.
	/// </summary>
	/// <typeparam name="TContext">The type of the state machine instance.</typeparam>
	/// <remarks>
	/// The invariant may represent a static situation such as an object waiting for some external event to occur.
	/// </remarks>
	public class State<TContext> : Vertex<TContext> where TContext : IContext<TContext> {
		/// <summary>
		/// The child Regions where the State is composite.
		/// </summary>
		IEnumerable<Region<TContext>> Regions { get { return this.regions; } }

		/// <summary>
		/// An optional behavior that is executed whenever this state is exited, regardless of which transition was taken out of the state.
		/// </summary>
		/// <remarks>
		/// If defined, exit actions are always executed to completion only after all internal activities and transition actions have completed execution.
		/// </remarks>
		public event Action<TContext> Exit;

		/// <summary>
		/// An optional behavior that is executed whenever this state is entered regardless of the transition taken to reach the state.
		/// </summary>
		/// <remarks>
		/// If defined, entry actions are always executed to completion prior to any internal behavior or transitions performed within the state.
		/// </remarks>
		public event Action<TContext> Entry;

		internal Region<TContext>[] regions;

		/// <summary>
		/// Creates a new instance of the State class.
		/// </summary>
		/// <param name="name">The name of the state.</param>
		/// <param name="parent">The parent Region.</param>
		public State( String name, Region<TContext> parent )
			: base( name, parent, Transition<TContext>.State ) {
			Trace.Assert( name != null, "States must have a name" );
			Trace.Assert( parent != null, "States must have a parent Region" );
		}

		// Constructor used by FinalState
		internal State( String name, Region<TContext> parent, Func<Transition<TContext>[], TContext, Object, Transition<TContext>> selector ) : base( name, parent, selector ) { }

		/// <summary>
		/// True if the State is a simple State.
		/// </summary>
		/// <remarks>
		/// A simple State is one that has no child Regions.
		/// </remarks>
		public Boolean IsSimple { get { return this.regions == null || this.regions.Length == 0; } }

		/// <summary>
		/// True if the State is a composite State.
		/// </summary>
		/// <remarks>
		/// A composite State is one that has one or more child Regions.
		/// </remarks>
		public Boolean IsComposite { get { return this.regions != null && this.regions.Length > 0; } }

		/// <summary>
		/// True if the State is an orthogonal State.
		/// </summary>
		/// <remarks>
		/// A composite State is one that has more than one child Regions.
		/// </remarks>
		public Boolean IsOrthogonal { get { return this.regions != null && this.regions.Length > 1; } }

		/// <summary>
		/// Creates an internal transition.
		/// </summary>
		/// <typeparam name="TMessage">The type of the messaage that the internal transition will react to.</typeparam>
		/// <param name="guard">The guard condition that must be met for hte transition to be traversed.</param>
		/// <returns>The internal transition.</returns>
		/// <remarks>
		/// An internal transition does not exit or enter any states, however the transitions effect will be invoked if the guard condition of the transition is met
		/// </remarks>
		public Transition<TContext> When<TMessage>( Func<TContext, TMessage, Boolean> guard ) where TMessage : class {
			return this.To( null ).When( guard );
		}

		/// <summary>
		/// Invokes the Exit behavior upon exiting a State.
		/// </summary>
		/// <param name="context">The state machine context.</param>
		/// <param name="message">The message that triggered the state transition.</param>
		/// <param name="history">A flag denoting if history semantics were in play during the transition.</param>
		protected virtual void OnExit( TContext context, Object message, Boolean history ) {
			this.Exit( context );
		}

		/// <summary>
		/// Invokes the Entry behavior upon entering a State.
		/// </summary>
		/// <param name="context">The state machine context.</param>
		/// <param name="message">The message that triggered the state transition.</param>
		/// <param name="history">A flag denoting if history semantics were in play during the transition.</param>
		protected virtual void OnEntry( TContext context, Object message, Boolean history ) {
			this.Entry( context );
		}

		internal override Boolean IsComplete( TContext context ) {
			return this.IsSimple || this.regions.All( region => region.IsComplete( context ) );
		}

		internal virtual void Add( Region<TContext> region ) {
			if( this.regions == null )
				this.regions = new Region<TContext>[ 1 ] { region };
			else {
				Trace.Assert( this.regions.Where( r => r.Name == region.Name ).Count() == 0, "Regions must have a unique name within the scope of their parent State" );

				var regions = new Region<TContext>[ this.regions.Length + 1 ];

				this.regions.CopyTo( regions, 0 );

				regions[ this.regions.Length ] = region;

				this.regions = regions;
			}

			region.Root.Clean = false;
		}

		internal override void BootstrapElement( Boolean deepHistoryAbove ) {
			if( this.IsComposite ) {
				foreach( var region in this.regions ) {
					region.Reset();
					region.BootstrapElement( deepHistoryAbove );

					this.Leave += ( context, message, history ) => region.Leave( context, message, history );
					this.EndEnter += region.Enter;
				}
			}

			base.BootstrapElement( deepHistoryAbove );

			if( this.Exit != null )
				this.Leave += this.OnExit;

			if( this.Entry != null )
				this.BeginEnter += this.OnEntry;

			this.BeginEnter += ( context, message, history ) => context[ this.Region ] = this;

			this.Enter = this.BeginEnter + this.EndEnter;
		}

		internal override void BootstrapTransitions() {
			if( this.IsComposite )
				foreach( var region in this.regions )
					region.BootstrapTransitions();

			base.BootstrapTransitions();
		}

		internal override void BootstrapEnter( ref Action<TContext, object, bool> traverse, StateMachineElement<TContext> next ) {
			base.BootstrapEnter( ref traverse, next );

			if( this.IsOrthogonal )
				foreach( var region in this.regions )
					if( region != next )
						traverse += region.Enter;
		}

		internal override Boolean Evaluate( TContext context, Object message ) {
			var processed = base.Evaluate( context, message );

			if( !processed )
				if( this.IsComposite )
					for( int i = 0, l = this.regions.Length; i < l; ++i )
						if( this.regions[ i ].Evaluate( context, message ) )
							processed = true;

			if( processed == true && message != this )
				this.EvaluateCompletions( context, this, false );

			return processed;
		}
	}
}
