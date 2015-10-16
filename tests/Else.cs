/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
using System.Diagnostics;
using Steelbreeze.StateMachines.Model;
using Steelbreeze.StateMachines.Tools;
using Steelbreeze.StateMachines.Runtime;

namespace Steelbreeze.StateMachines.Tests {
	public static class Else {
		public static void Run () {
			var model = new StateMachine<Instance>("model");

			var initial = new PseudoState<Instance>("initial", model);

			var choice = new PseudoState<Instance>("choice", model, PseudoStateKind.Choice);
			var junction = new PseudoState<Instance>("junction", model, PseudoStateKind.Junction);

			var finalState = new FinalState<Instance>("final", model);

			initial.To(choice);
			choice.To(junction).When(i => i.Int1 == 0).Effect(i => i.Int1 = 1);
			choice.To(finalState).Else();
			junction.To(choice).When(i => i.Int2 == 0).Effect(i => i.Int2 = 2);

			model.Validate();

			var instance = new Instance("else");

			model.Initialise(instance);

			Trace.Assert(model.IsComplete(instance));
			Trace.Assert(instance.Int1 == 1);
			Trace.Assert(instance.Int2 == 2);
		}
	}
}