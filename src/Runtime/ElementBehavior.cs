/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
 using Steelbreeze.StateMachines.Model;

namespace Steelbreeze.StateMachines.Runtime {
	// Internal type used while initialising a state machine model; used to hold the pieces of bahavior required on an interim basis
	internal class ElementBehavior<TInstance> where TInstance : IInstance<TInstance> {
		// The behavior to execute when leaving an element
		public Behavior<TInstance> leave;

		// The behavior to execute when entering an element
		public Behavior<TInstance> beginEnter;

		// The behavior to execute to complete the entry of an element (used for the last element in the target ancestry)
		public Behavior<TInstance> endEnter;

		// The behavior to execute when entering an element
		public Behavior<TInstance> Enter {
			get {
				return this.beginEnter + this.endEnter;
			}
		}
	}
}
