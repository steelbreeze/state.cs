/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Diagnostics;

namespace Steelbreeze.Behavior.StateMachines.Tests.Users {
	/// <summary>
	/// Test relating to external transitions and orthogonal regions; a transition in one region renders a potential transition in another region obsolete
	/// </summary>
	public class P3pp3r {
		public static void Test () {
			var model = new StateMachine<StateMachineInstance> ("model");
			var initial = model.CreatePseudoState ("initial", PseudoStateKind.Initial);
			var state1 = model.CreateState ("state1");
			var state2 = model.CreateState ("state2");

			var regionA = state1.CreateRegion ("regionA");
			var initialA = regionA.CreatePseudoState ("initialA", PseudoStateKind.Initial);
			var state3 = regionA.CreateState ("state3");
			var state8 = regionA.CreateState ("state8");

			var regionB = state1.CreateRegion ("regionB");
			var initialB = regionB.CreatePseudoState ("initialB", PseudoStateKind.Initial);
			var state4 = regionB.CreateState ("state4");
			var state5 = regionB.CreateState ("state5");

			var regionBa = state4.CreateRegion ("regionBa");
			var initialBa = regionBa.CreatePseudoState ("initialBa", PseudoStateKind.Initial);
			var state6 = regionBa.CreateState ("state6");

			var regionBb = state4.CreateRegion ("regionBb");
			var initialBb = regionBb.CreatePseudoState ("initialBb", PseudoStateKind.Initial);
			var state7 = regionBb.CreateState ("state7");

			initial.To (state1);
			initialA.To (state3);
			initialB.To (state4);
			initialBa.To (state6);
			initialBb.To (state7);

			state3.To (state2).When<String> (c => c == "event2");
			state3.To (state8).When<String> (c => c == "event1");

			state7.To (state5).When<String> (c => c == "event2").Effect (() => Trace.TraceError ("p3pp3r test failed"));
			state7.To (state5).When<String> (c => c == "event1");

			var instance = new StateMachineInstance ("p3pp3r");
			model.Initialise (instance);

			model.Evaluate ("event2", instance);
		}
	}
}