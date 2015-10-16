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
	public static class Internal {
		public static void Run () {
			var model = new StateMachine<Instance>("model");
			var initial = new PseudoState<Instance>("initial", model, PseudoStateKind.Initial);
			var target = new State<Instance>("state", model).Entry(i => i.Int1++).Exit(i => i.Int2++);

			initial.To(target);

			target.To().When<string>(m => m == "internal").Effect(i => i.Int3++);
			target.To(target).When<string>(m => m == "external").Effect(i => i.Int3++);

			var instance = new Instance("internal");

			model.Validate();

			model.Initialise(instance);

			model.Evaluate(instance, "internal");

			Trace.Assert(target == instance.GetCurrent(model.DefaultRegion));
			Trace.Assert(1 == instance.Int1);
			Trace.Assert(0 == instance.Int2);
			Trace.Assert(1 == instance.Int3);

			model.Evaluate(instance, "external");

			Trace.Assert(target == instance.GetCurrent(model.DefaultRegion));
			Trace.Assert(2 == instance.Int1);
			Trace.Assert(1 == instance.Int2);
			Trace.Assert(2 == instance.Int3);
		}
	}
}