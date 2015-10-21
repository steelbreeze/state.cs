/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
using Steelbreeze.StateMachines.Model;

namespace Steelbreeze.StateMachines.Tools {
	/// <summary>
	/// Extensions to the core model to perform non-core operations, such as model validation.
	/// </summary>
	public static class Extensions {
		/// <summary>
		/// Validates a state machine model for correctness.
		/// </summary>
		/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
		/// <param name="model">The state machine model.</param>
		/// <remarks>The validation criteria are largely drawn from the UML 2 Superstructure Specification.</remarks>
		public static void Validate<TInstance>(this StateMachine<TInstance> model) where TInstance : IInstance<TInstance> {
			model.Accept(new Validator<TInstance>());
		}
	}
}