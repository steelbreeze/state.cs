/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Diagnostics;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// A special kind of State signifying that its parent Region is completed.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	/// <remarks>To be complete, final states cannot have any child model structure beneath them (Region's) or outgoing transitions.</remarks>
	public sealed class FinalState<TInstance> : State<TInstance> where TInstance : IActiveStateConfiguration<TInstance> {
		/// <summary>
		/// The name of the type without generic considerations
		/// </summary>
		public override string Type { get { return "finalState"; } }

		/// <summary>
		/// Initializes a new instance of a FinalState within the parent Region.
		/// </summary>
		/// <param name="name">The name of the FinalState.</param>
		/// <param name="parent">The parent Region of the FinalState.</param>
		public FinalState (String name, Region<TInstance> parent)
			: base (name, parent) {
			Trace.Assert (name != null, "FinalStates must have a name");
			Trace.Assert (parent != null, "FinalStates must have a parent Region");
		}

		// override State's implementation of IsComplete
		public override Boolean IsComplete (TInstance instance) {
			return true;
		}

		// do not allow FinalState's to become composite
		internal override void Add (Region<TInstance> region) {
			throw new NotSupportedException ("A FinalState may not be the parent of a Region");
		}

		/// <summary>
		/// Creates a new transition from this Vertex.
		/// </summary>
		/// <param name="target">The Vertex to transition to.</param>
		/// <returns>An intance of the Transition class.</returns>
		/// <remarks>
		/// FinalState's are not permitted to have outgoing transitions; this method will therefore throw an exception.
		/// </remarks>
		public override Transition<TInstance> To (Vertex<TInstance> target) {
			throw new NotSupportedException ("Transitions my not originate from a FinalState");
		}
	}
}