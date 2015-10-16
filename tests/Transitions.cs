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
	public static class Transitions {
		public static void Run () {
			var model = new StateMachine<Instance>("compTest");
			var initial = new PseudoState<Instance>("initial", model, PseudoStateKind.Initial);

			var activity1 = new State<Instance>("activity1", model);
			var activity2 = new State<Instance>("activity2", model);
			var activity3 = new State<Instance>("activity3", model);
			var junction1 = new PseudoState<Instance>("junction1", model, PseudoStateKind.Junction);
			var junction2 = new PseudoState<Instance>("junction2", model, PseudoStateKind.Junction);
			var end = new FinalState<Instance>("end", model);

			var subInitial = new PseudoState<Instance>("subInitial", activity2, PseudoStateKind.Initial);
			var subEnd = new FinalState<Instance>("subEnd", activity2);

			subInitial.To(subEnd);
			initial.To(activity1);
			activity1.To(activity2);
			activity2.To(junction1);
			junction1.To(junction2).Else();
			junction2.To(activity3).Else();
			activity3.To(end);

			model.Validate();

			var instance = new Instance("transitions");
			model.Initialise(instance);

			Trace.Assert(model.IsComplete(instance));
		}
	}
}