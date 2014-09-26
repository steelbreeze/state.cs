/* State v5 finite state machine library
 * Copyright (c) 2014 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Diagnostics;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// A sepcial kind of State signifying that its parent Region is completed.
	/// </summary>
	/// <typeparam name="TContext">The type of the state machine context.</typeparam>
	/// <remarks>To be complete, final states cannot have any child model structure beneath them (Region's) or outgoing transitions.</remarks>
	public sealed class FinalState<TContext> : State<TContext> where TContext : IContext<TContext> {
		/// <summary>
		/// Initializes a new instance of a FinalState within the parent Region.
		/// </summary>
		/// <param name="name">The name of the FinalState.</param>
		/// <param name="parent">The parent Region of the FinalState.</param>
		public FinalState( String name, Region<TContext> parent ) : base( name, parent, Transition<TContext>.Null ){
			Trace.Assert( name != null, "FinalStates must have a name" );
			Trace.Assert( parent != null, "FinalStates must have a parent Region" );
		}

		// override State's implementation of IsComplete
		internal override Boolean IsComplete( TContext context ) {
			return true;
		}

		// do not allow FinalState's to become composite
		internal override void Add( Region<TContext> region ) {
			throw new NotSupportedException( "A FinalState may not be the parent of a Region" );
		}

		/// <summary>
		/// Creates a new transition from this Vertex.
		/// </summary>
		/// <param name="target">The Vertex to transition to.</param>
		/// <returns>An intance of the Transition class.</returns>
		/// <remarks>
		/// FinalState's are not permitted to have outgoing transitions; this method will therefore throw an exception.
		/// </remarks>
		public override Transition<TContext> To( Vertex<TContext> target ) {
			throw new NotSupportedException( "Transitions my not originate from a FinalState" );
		}
	}
}
