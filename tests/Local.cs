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
	public static class Local {
		public static void Run () {
			// create the state machine model elements
			var model = new StateMachine<Instance>("model");
			var initial = new PseudoState<Instance>("initial", model, PseudoStateKind.Initial);
			var stateA = new State<Instance>("stateA", model);
			var stateB = new State<Instance>("stateB", model).Exit(i => i.Int1++);

			var bInitial = new PseudoState<Instance>("bInitial", stateB);
			var bStateI = new State<Instance>("bStateI", stateB);
			var bStateII = new State<Instance>("bStateII", stateB);

			// create the state machine model transitions
			initial.To(stateA);
			stateA.To(stateB).When<string>(message => message == "move");

			bInitial.To(bStateI);

			stateB.To(bStateII, TransitionKind.Local).When<string>(message => message == "local");
			stateB.To(bStateII, TransitionKind.External).When<string>(message => message == "external");
			model.Validate();

			// create a state machine instance
			var instance = new Instance("local");

			// initialise the model and instance
			model.Initialise(instance);

			// send the machine instance a message for evaluation, this will trigger the transition from stateA to stateB
			model.Evaluate(instance, "move");

			model.Evaluate(instance, "local");

			Trace.Assert(0 == instance.Int1);

			model.Evaluate(instance, "external");

			Trace.Assert(1 == instance.Int1);
		}
	}
}