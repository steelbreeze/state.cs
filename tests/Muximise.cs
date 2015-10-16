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
	public static class Muximise {
		public static void Run () {
			var model = new StateMachine<Instance>("model");

			var initial = new PseudoState<Instance>("initial", model, PseudoStateKind.Initial);
			var ortho = new State<Instance>("ortho", model);
			var simple = new State<Instance>("simple", model);
			var final = new FinalState<Instance>("final", model);

			var r1 = new Region<Instance>("r1", ortho);
			var r2 = new Region<Instance>("r2", ortho);

			var i1 = new PseudoState<Instance>("initial", r1, PseudoStateKind.ShallowHistory);
			var i2 = new PseudoState<Instance>("initial", r2, PseudoStateKind.ShallowHistory);

			var s1 = new State<Instance>("s1", r1);
			var s2 = new State<Instance>("s2", r2);

			var f1 = new FinalState<Instance>("f1", r1);
			var f2 = new FinalState<Instance>("f2", r2);

			initial.To(ortho);

			i1.To(s1);
			i2.To(s2);

			ortho.To(final); // This should happen once all regions in ortho are complete?

			s1.To(f1).When<string>(c => c == "complete1");
			s2.To(f2).When<string>(c => c == "complete2");

			ortho.To(simple).When<string>(c => c == "jump");
			simple.To(ortho).When<string>(c => c == "back");

			model.Validate();

			var instance = new Instance("muximise");
			model.Initialise(instance);
			/*
console.log("simple.isSimple = " + simple.isSimple());
console.log("simple.isComposite = " + simple.isComposite());
console.log("simple.isOrthogonal = " + simple.isOrthogonal());

console.log("model.isSimple = " + model.isSimple());
console.log("model.isComposite = " + model.isComposite());
console.log("model.isOrthogonal = " + model.isOrthogonal());

console.log("ortho.isSimple = " + ortho.isSimple());
console.log("ortho.isComposite = " + ortho.isComposite());
console.log("ortho.isOrthogonal = " + ortho.isOrthogonal());
*/

			Trace.Assert(simple.IsSimple);
			Trace.Assert(!model.IsSimple);
			Trace.Assert(!ortho.IsSimple);

			Trace.Assert(!simple.IsComposite);
			Trace.Assert(model.IsComposite);
			Trace.Assert(ortho.IsComposite);

			Trace.Assert(!simple.IsOrthogonal);
			Trace.Assert(!model.IsOrthogonal);
			Trace.Assert(ortho.IsOrthogonal);

			model.Evaluate(instance, "complete1");
			model.Evaluate(instance, "complete2");

			Trace.Assert(model.IsComplete(instance));
		}
	}
}