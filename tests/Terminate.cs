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
	public static class Terminate {

		public static void Run () {
			var model = new StateMachine<Instance>("model");
			var initial = new PseudoState<Instance>("initial", model);
			var stateA = new State<Instance>("stateA", model);
			var terminate = new PseudoState<Instance>("terminate", model, PseudoStateKind.Terminate);

			initial.To(stateA);
			stateA.To(terminate).When<string>(message => message == "1");

			model.Validate();

			var instance = new Instance("terminate");

			model.Initialise(instance);

			Trace.Assert(!model.Evaluate(instance, "2"));
			Trace.Assert(model.Evaluate(instance, "1"));
			Trace.Assert(!model.Evaluate(instance, "1"));
			Trace.Assert(instance.IsTerminated);
		}
	}
}