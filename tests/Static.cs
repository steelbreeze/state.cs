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
	public static class Static {
		public static void Run () {

			var model = new StateMachine<Instance>("model");
			var initial = new PseudoState<Instance>("initial", model, PseudoStateKind.Initial);
			var junction1 = new PseudoState<Instance>("junction1", model, PseudoStateKind.Junction);
			var junction2 = new PseudoState<Instance>("junction2", model, PseudoStateKind.Junction);

			var pass = new State<Instance>("success", model);
			var fail = new State<Instance>("error", model);

			initial.To(junction1);

			junction1.To(junction2).When(i => i.Int1 == 0).Effect(i => i.Int1++);
			junction1.To(fail).Else();
			junction2.To(pass).When(i => i.Int1 == 0).Effect(i => i.Int1++);
			junction2.To(fail).Else();

			model.Validate();

			var instance = new Instance("static");

			model.Initialise(instance);

			Trace.Assert(pass == instance.GetCurrent(model.DefaultRegion));

			Trace.Assert(2 == instance.Int1);
		}
	}
}