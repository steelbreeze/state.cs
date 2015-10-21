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
	public static class Brice {
		public static void Run () {

			var model = new StateMachine<Instance>("model");
			var initial1 = new PseudoState<Instance>("initial", model, PseudoStateKind.Initial);
			var myComposite1 = new State<Instance>("composite1", model);
			var state3 = new State<Instance>("state3", model);
			var initial2 = new PseudoState<Instance>("initial", myComposite1, PseudoStateKind.Initial);
			var state1 = new State<Instance>("state1", myComposite1);
			var state2 = new State<Instance>("state2", myComposite1);

			initial1.To(myComposite1);
			initial2.To(state1);
			myComposite1.To(state3).When<string>(c => c == "a");
			state1.To(state2).When<string>(c => c == "a");

			model.Validate();

			var instance = new Instance("brice");
			model.Initialise(instance);

			model.Evaluate(instance, "a");

			Trace.Assert(instance.GetCurrent(myComposite1.DefaultRegion) == state2);
		}
	}
}