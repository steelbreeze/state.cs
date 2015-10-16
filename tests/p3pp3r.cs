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

	public static class p3pp3r {
		public static void Run () {
			var model = new StateMachine<Instance>("model");
			var initial = new PseudoState<Instance>("initial", model, PseudoStateKind.Initial);
			var state1 = new State<Instance>("state1", model);
			var state2 = new State<Instance>("state2", model);

			var regionA = new Region<Instance>("regionA", state1);
			var initialA = new PseudoState<Instance>("initialA", regionA, PseudoStateKind.Initial);
			var state3 = new State<Instance>("state3", regionA);
			var state8 = new State<Instance>("state8", regionA);

			var regionB = new Region<Instance>("regionB", state1);
			var initialB = new PseudoState<Instance>("initialB", regionB, PseudoStateKind.Initial);
			var state4 = new State<Instance>("state4", regionB);
			var state5 = new State<Instance>("state5", regionB);

			var regionBa = new Region<Instance>("regionBa", state4);
			var initialBa = new PseudoState<Instance>("initialBa", regionBa, PseudoStateKind.Initial);
			var state6 = new State<Instance>("state6", regionBa);

			var regionBb = new Region<Instance>("regionBb", state4);
			var initialBb = new PseudoState<Instance>("initialBb", regionBb, PseudoStateKind.Initial);
			var state7 = new State<Instance>("state7", regionBb);

			initial.To(state1);
			initialA.To(state3);
			initialB.To(state4);
			initialBa.To(state6);
			initialBb.To(state7);

			state3.To(state2).When<string>(c => c == "event2");
			state3.To(state8).When<string>(c => c == "event1");
			state7.To(state5).When<string>(c => c == "event2");
			state7.To(state5).When<string>(c => c == "event1");

			model.Validate();

			var instance = new Instance("p3pp3r");
			model.Initialise(instance);

			model.Evaluate(instance, "event2");

			Trace.Assert(state2 == instance.GetCurrent(model.DefaultRegion), instance.GetCurrent(model.DefaultRegion).ToString() );
			Trace.Assert(state4 == instance.GetCurrent(regionB));
		}
	}
}