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
	public sealed class FinalState<TInstance> : State<TInstance> where TInstance : class, IActiveStateConfiguration<TInstance> {
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

		/// <summary>
		/// Tests the final state to determine if it is deemed to be complete.
		/// </summary>
		/// <param name="instance">The state machine instance.</param>
		/// <returns>True if the final state is complete</returns>
		/// <remarks>FinalStates are always deemed to be complete.</remarks>
		public override Boolean IsComplete (TInstance instance) {
			return true;
		}

		/// <summary>
		/// Adds a region to the state.
		/// </summary>
		/// <param name="region">The region to add to the state.</param>
		/// <exception cref="System.NotSupportedException">Final states cannot be composite states so Add will always throw this exception.</exception>
		internal override void Add (Region<TInstance> region) {
			throw new NotSupportedException ("A FinalState may not be the parent of a Region");
		}

		/// <summary>
		/// Creates a new transition from this Vertex.
		/// </summary>
		/// <param name="target">The Vertex to transition to.</param>
		/// <returns>This implementation of To will not return a value.</returns>
		/// <exception cref="System.NotSupportedException"></exception>
		/// <remarks>
		/// As final states may not have outbound transitions, this implementation will always throw an exception.
		/// </remarks>
		public override Transition<TInstance> To (Vertex<TInstance> target) {
			throw new NotSupportedException ("A FinalState may not have outbound transitions");
		}

		/// <summary>
		/// Selects a transition for a given message and state machine instance for the final state.
		/// </summary>
		/// <param name="message">The message that may trigger a transition.</param>
		/// <param name="instance">The state machine instance.</param>
		/// <returns>Always returns null.</returns>
		/// <remarks>As a final state cannot have transitions, this implementation of Select will always return null.</remarks>
		protected internal override Transition<TInstance> Select (object message, TInstance instance) {
			return null;
		}

		/// <summary>
		/// Accepts a visitor
		/// </summary>
		/// <param name="visitor">The visitor to visit.</param>
		/// <param name="param">A parameter passed to the visitor when visiting elements.</param>
		/// <remarks>
		/// A visitor will walk the state machine model from this element to all child elements including transitions calling the approritate visit method on the visitor.
		/// </remarks>
		public override void Accept<TParam> (Visitor<TInstance, TParam> visitor, TParam param) {
			visitor.VisitFinalState (this, param);
		}
	}
}