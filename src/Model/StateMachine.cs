/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
namespace Steelbreeze.StateMachines.Model {
	public class StateMachine<TInstance> : State<TInstance> where TInstance : IInstance<TInstance> {
		internal bool Clean;
		internal Behavior<TInstance> OnInitialise;

		public StateMachine (string name) : base(name, null) { }

		/// <summary>
		/// Returns the state machine that this state machine is a part of.
		/// </summary>
		/// <remarks>A state machine may be embedded within another state machine; in this circumstance it behaves as any other state.</remarks>
		public override StateMachine<TInstance> Root {
			get {
				return this.Region != null ? this.Region.Root : this;
			}
		}

		/// <summary>
		/// Accepts a visitor.
		/// </summary>
		/// <param name="visitor">The visitor to accept.</param>
		public override void Accept(Visitor<TInstance> visitor)
        {
            visitor.VisitStateMachine(this);
        }

		/// <summary>
		/// Accepts a visitor.
		/// </summary>
		/// <typeparam name="TArg">The type of the argument passed into the visitor.</typeparam>
		/// <param name="visitor">The visitor to accept.</param>
		/// <param name="arg">The argument to pass to each element visited.</param>
		public override void Accept<TArg>(Visitor<TInstance, TArg> visitor, TArg arg)
        {
            visitor.VisitStateMachine(this, arg);
        }
    }
}