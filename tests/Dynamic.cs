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
	public static class Dynamic {
		public static void Run () {

			var model = new StateMachine<Instance>("model");
			var initial = new PseudoState<Instance>("initial", model, PseudoStateKind.Initial);
			var stateA = new State<Instance>("stateA", model).Exit(i => i.Int1 += 1);

			initial.To(stateA);

			var instance = new Instance("dynamic");

			model.Validate();

			model.Initialise(instance);

			Trace.Assert(!model.Evaluate(instance, "move"));

			var stateB = new State<Instance>("stateB", model).Entry(i => i.Int1 += 2);

			stateA.To(stateB).When<string>(message => message == "move").Effect(i => i.Int1 += 4);

			Trace.Assert(model.Evaluate(instance, "move"));
		}
	}
}