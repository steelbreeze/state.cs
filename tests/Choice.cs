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
	public static class Choice {
		private static int nextRand = 0;

		private static int randRobin (int max) {
			var result = nextRand;

			if (++nextRand == max) {
				nextRand = 0;
			}

			return result;
		}

		public static void Run () {

			var model = new StateMachine<Instance>("model");
			var initial = new PseudoState<Instance>("initial", model, PseudoStateKind.Initial);
			var stateA = new State<Instance>("stateA", model);
			var choice = new PseudoState<Instance>("choice", model, PseudoStateKind.Choice);

			initial.To(stateA);

			stateA.To(choice).When<string>(message => message == "choose");

			choice.To(stateA).Effect(instance => instance.Int1++);
			choice.To(stateA).Effect(instance => instance.Int2++);
			choice.To(stateA).Effect(instance => instance.Int3++);

			model.Validate();

			var instance1 = new Instance("instance1");

			model.Initialise(instance1);

			for (var i = 0; i < 99; i++) {
				model.Evaluate(instance1, "choose");
			}

			Trace.Assert(99 == (instance1.Int1 + instance1.Int2 + instance1.Int3));

			Runtime.Extensions.RandomSelector = randRobin;

			var instance2 = new Instance("instance2");

			model.Initialise(instance2);

			for (var i = 0; i < 99; i++) {
				model.Evaluate(instance2, "choose");
			}

			model.Evaluate(instance2, "end");

			Trace.Assert(33 == instance2.Int1);
			Trace.Assert(33 == instance2.Int2);
			Trace.Assert(33 == instance2.Int3);
		}
	}
}