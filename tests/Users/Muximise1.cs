/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Diagnostics;

namespace Steelbreeze.Behavior.StateMachines.Tests.Users {
	public class Muximise1 {
		public static StateMachine<StateMachineInstance> Model { get; private set; }

		static Muximise1 () {
			Model = new StateMachine<StateMachineInstance> ("model");

			var initial = Model.CreatePseudoState ("initial");
			var ortho = Model.CreateState ("ortho");
			var simple = Model.CreateState ("simple");
			var final = Model.CreateFinalState ("final");

			var r1 = ortho.CreateRegion ("r1");
			var r2 = ortho.CreateRegion ("r2");


			var i1 = r1.CreatePseudoState ("initial", PseudoStateKind.ShallowHistory);
			var i2 = r2.CreatePseudoState ("initial", PseudoStateKind.ShallowHistory);

			var s1 = r1.CreateState ("s1");
			var s2 = r2.CreateState ("s2");

			var f1 = r1.CreateFinalState ("f1");
			var f2 = r2.CreateFinalState ("f2");

			initial.To (ortho);

			i1.To (s1);
			i2.To (s2);

			ortho.To (final); // This should happen once all regions in ortho are complete?

			s1.To (f1).When<String> (c => c == "complete1");
			s2.To (f2).When<String> (c => c == "complete2");

			ortho.To (simple).When<String> (c => c == "jump");
			simple.To (ortho).When<String> (c => c == "back");
		}

		public static void Test () {
			var instance = new StateMachineInstance ("muximise1");

			Model.Initialise (instance);

			Model.Evaluate ("complete1", instance);
			Model.Evaluate ("complete2", instance);

			Trace.Assert (Model.IsComplete (instance));
		}
	}
}