/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
using Steelbreeze.StateMachines.Model;

namespace Steelbreeze.StateMachines.Tools {
	public static class Extensions {
		public static void Validate<TInstance> (this StateMachine<TInstance> model) where TInstance : IInstance<TInstance> {
			model.Accept(new Validator<TInstance>());
		}
	}
}
