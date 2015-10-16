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
	public static class History {
		public static void Run () {
			var model = new StateMachine<Instance>("history");

			var initial = new PseudoState<Instance>("initial", model, PseudoStateKind.Initial);
			var shallow = new State<Instance>("shallow", model);
			var deep = new State<Instance>("deep", model);
			var end = new FinalState<Instance>("final", model);

			var s1 = new State<Instance>("s1", shallow);
			var s2 = new State<Instance>("s2", shallow);

			initial.To(shallow);
			new PseudoState<Instance>("shallow", shallow, PseudoStateKind.ShallowHistory).To(s1);
			s1.To(s2).When<string>(c => c == "move");
			shallow.To(deep).When<string>(c => c == "go deep");
			deep.To(shallow).When<string>(c => c == "go shallow");
			s2.To(end).When<string>(c => c == "end");

			model.Validate();

			var instance = new Instance("history");

			model.Initialise(instance);

			model.Evaluate(instance, "move");
			model.Evaluate(instance, "go deep");
			model.Evaluate(instance, "go shallow");
			model.Evaluate(instance, "end");

			Trace.Assert(model.IsComplete(instance));
		}
	}
}