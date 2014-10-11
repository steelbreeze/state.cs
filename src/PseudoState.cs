/* State v5 finite state machine library
 * Copyright (c) 2014 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Diagnostics;
using System.Linq;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// A PseudoState is an abstraction that encompasses different types of transient vertices in the state machine.
	/// </summary>
	/// <typeparam name="TContext">The type of the state machine instance.</typeparam>
	/// <remarks>
	/// Pseudostates are typically used to connect multiple transitions into more complex state transitions path.
	/// </remarks>
	public sealed class PseudoState<TContext> : Vertex<TContext> where TContext : IContext<TContext> {
		/// <summary>
		/// The name of the type without generic considerations
		/// </summary>
		public override string Type { get { return "pseudoState"; } }

		/// <summary>
		/// Determines the precise type of the Pseudostate.
		/// </summary>
		/// <remarks>
		/// The default kind of a PseudoState is Initial.
		/// </remarks>
		public readonly PseudoStateKind Kind;

		internal Boolean IsHistory { get { return this.Kind == PseudoStateKind.DeepHistory || this.Kind == PseudoStateKind.ShallowHistory; } }
		internal Boolean IsInitial { get { return this.Kind == PseudoStateKind.Initial || this.IsHistory; } }

		/// <summary>
		/// Initialises a new instance of the PseudoState class.
		/// </summary>
		/// <param name="name">The name of the PseudoState.</param>
		/// <param name="parent">The parent Region.</param>
		/// <param name="kind">The kind of the PseudoState</param>
		/// <remarks>
		/// The kind of the PseudoState dictates is use and semantics; see the documentation of PseudoStateKind.
		/// </remarks>
		public PseudoState( String name, Region<TContext> parent, PseudoStateKind kind = PseudoStateKind.Initial )
			: base( name, parent, Transition<TContext>.PseudoState( kind ) ) {
			Trace.Assert( name != null, "PseudoStates must have a name" );
			Trace.Assert( parent != null, "PseudoStates must have a parent Region" );

			this.Kind = kind;

			if( this.IsInitial )
				this.Region.Initial = this;
		}

		/// <summary>
		/// Creates a new transition from this PseudoState.
		/// </summary>
		/// <param name="target">The Vertex to transition to.</param>
		/// <returns>An intance of the Transition class.</returns>
		public override Transition<TContext> To( Vertex<TContext> target ) {
			Trace.Assert( target != null, "Transitions from PseudoStates must have a target" );
			return base.To( target );
		}

		internal override void BootstrapElement( bool deepHistoryAbove ) {
			base.BootstrapElement( deepHistoryAbove );

			if( this.Kind == PseudoStateKind.Terminate )
				this.Enter += ( context, message, history ) => context.IsTerminated = true;
		}
	}
}