/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
namespace Steelbreeze.StateMachines.Model {
	/// <summary>
	/// Represents a state within a region that cannot have outgoing transitions.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	/// <remarks>When reached, a final state will cause its parent region to be deemed complete.</remarks>
	public class FinalState<TInstance> : State<TInstance> where TInstance : IInstance<TInstance> {
		/// <summary>
		/// Creates a new final state as a child of a region.
		/// </summary>
		/// <param name="region">The parent region.</param>
		/// <param name="name">The name of the new final state.</param>
		/// <returns>The newly created final state.</returns>
		public FinalState (string name, Region<TInstance> region) : base(name, region) { }

		/// <summary>
		/// Accepts a visitor.
		/// </summary>
		/// <param name="visitor">The visitor to accept.</param>
        public override void Accept(Visitor<TInstance> visitor)
        {
            visitor.VisitFinalState(this);
        }

		/// <summary>
		/// Accepts a visitor.
		/// </summary>
		/// <typeparam name="TArg">The type of the argument passed into the visitor.</typeparam>
		/// <param name="visitor">The visitor to accept.</param>
		/// <param name="arg">The argument to pass to each element visited.</param>
        public override void Accept<TArg>(Visitor<TInstance, TArg> visitor, TArg arg)
        {
            visitor.VisitFinalState(this, arg);
        }
    }
}