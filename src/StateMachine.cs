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
	/// A StateMachine is the root node of a hierarchical state machine model.
	/// </summary>
	/// <typeparam name="TContext">The type of the state machine context.</typeparam>
	public sealed class StateMachine<TContext> : StateMachineElement<TContext> where TContext : IContext<TContext> {
		/// <summary>
		/// The child Regions.
		/// </summary>
		public IEnumerable<Region<TContext>> Regions { get { return this.regions; } }
	
		internal Boolean Clean { get; set; }
		internal Region<TContext>[] regions;
	
		/// <summary>
		/// Returns the elements parent.
		/// </summary>
		/// <remarks>
		/// A StateMachine will have no parent; this value will always be null.
		/// </remarks>
		public override StateMachineElement<TContext> Parent { get { return null; } }

		/// <summary>
		/// Initialises a new instance of the StateMachine class.
		/// </summary>
		/// <param name="name">The name of the StateMachine.</param>
		public StateMachine( String name )
			: base( name, null ) {
			Trace.Assert( name != null, "StateMachines must have a name" );
		}

		internal void Add( Region<TContext> region ) {
			if( this.regions == null )
				this.regions = new Region<TContext>[ 1 ] { region };
			else {
				Trace.Assert( this.regions.Where( r => r.Name == region.Name ).Count() == 0, "Regions must have a unique name within the scope of their parent StateMachine" );

				var regions = new Region<TContext>[ this.regions.Length + 1 ];

				this.regions.CopyTo( regions, 0 );

				regions[ this.regions.Length ] = region;

				this.regions = regions;
			}

			this.Clean = false;
		}

		/// <summary>
		/// Initialises a state machine model.
		/// </summary>
		/// <remarks>
		/// Initialising a state machine model pre-compiles all the transitions.
		/// This process will be triggered automatically on any call to StateMachine.Initialise( TContext context ) or StateMachine.Process if the model structure has changed.
		/// If you want to take greater control of when this happens, pass autoInitialise = false to StateMachine.Initialise( TContext context ) or StateMachine.Process and call Initialise as required instead.
		/// </remarks>
		public void Initialise() {
			this.Reset();
			this.Clean = true;

			this.BootstrapElement( false );
			this.BootstrapTransitions();
		}

		/// <summary>
		/// Initialises a state machine context to its initial state; this causes the state machine context to enter its initial state.
		/// </summary>
		/// <param name="context">The state machine context.</param>
		/// <param name="autoInitialise">True if you wish to automatically re-initialise the state machine model prior to initialising the state machine context.</param>
		public void Initialise( TContext context, Boolean autoInitialise = true ) {
			if( !this.Clean && autoInitialise )
				this.Initialise();

			this.Enter( context, null, false );
		}

		/// <summary>
		/// Determines if the state machine context has completed its processsing.
		/// </summary>
		/// <param name="context">The state machine context to test completeness for.</param>
		/// <returns>True if the state machine context has completed.</returns>
		/// <remarks>
		/// A state machine context is deemed complete when all its child Regions are complete.
		/// A Region is deemed complete if its current state is a FinalState (States are also considered to be FinalStates if there are no outbound transitions).
		/// In addition, if a state machine context is terminated (by virtue of a transition to a Terminate PseudoState) it is also deemed to be completed.
		/// </remarks>
		public Boolean IsComplete( TContext context ) {
			return context.IsTerminated || this.regions.All( region => region.IsComplete( context ) );
		}

		/// <summary>
		/// Pass a message to a state machine context for evaluation.
		/// </summary>
		/// <param name="context">The state machine context to evaluate the message against.</param>
		/// <param name="message">The message to evaluate.</param>
		/// <param name="autoInitialise">True if you wish to automatically re-initialise the state machine model prior to evaluating the message.</param>
		/// <returns>True if the message triggered a state transition.</returns>
		/// <remarks>
		/// Note that due to the potential for orthogonal Regions in composite States, it is possible for multiple transitions to be triggered.
		/// </remarks>
		public Boolean Evaluate( TContext context, Object message, Boolean autoInitialise = true ) {
			if( !this.Clean && autoInitialise )
				this.Initialise();

			Boolean processed = false;

//			stopwatch.Restart();

			if( !context.IsTerminated )
				for( int i = 0, l = this.regions.Length; i < l; ++i )
					if( this.regions[ i ].Evaluate( context, message ) )
						processed = true;

//			stopwatch.Stop();

//			Console.WriteLine( "Message processing took {0}μs", stopwatch.ElapsedTicks * 1000 * 1000 / System.Diagnostics.Stopwatch.Frequency );

			return processed;
		}

		internal override void BootstrapElement( Boolean deepHistoryAbove ){
			foreach( var region in this.regions ) {
				region.Reset();
				region.BootstrapElement( deepHistoryAbove );

				this.EndEnter += region.Enter;
			}

			base.BootstrapElement( deepHistoryAbove);
		}

		internal override void BootstrapTransitions() {
			foreach( var region in this.regions )
				region.BootstrapTransitions();
		}

//		private static System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
	}
}
