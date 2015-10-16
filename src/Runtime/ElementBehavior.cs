/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
 using Steelbreeze.StateMachines.Model;

namespace Steelbreeze.StateMachines.Runtime {
	internal class ElementBehavior<TInstance> where TInstance : IInstance<TInstance> {
		public Behavior<TInstance> leave;
		public Behavior<TInstance> beginEnter;
		public Behavior<TInstance> endEnter;

		public Behavior<TInstance> Enter {
			get {
				return this.beginEnter + this.endEnter;
			}
		}
	}
}
