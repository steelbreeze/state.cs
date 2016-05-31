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
	public static class Florent {
		public static void Run () {
			var model = new StateMachine<StateMachineInstance>("Model");
			var initial = model.CreatePseudoState("Initial", PseudoStateKind.Initial);
			var on = model.CreateState("On");
			var off = model.CreateState("Off");
			var clean = model.CreateState("Clean");
			var final = model.CreateFinalState("Final");

			var history = on.CreatePseudoState("History", PseudoStateKind.ShallowHistory);
			var idle = on.CreateState("Idle");
			var moveItem = on.CreateState("MoveItem");
			var showItemMovePattern = on.CreateState("ShowItemMovePattern");
			var hideItemMovePattern = on.CreateState("HideItemMovePattern");

			initial.To(idle);
			on.To(clean).When<string>(s => s == "DestroyInput");
			off.To(clean).When<string>(s => s == "DestroyInput");
			on.To(off).When<string>(s => s == "Disable");
			off.To(history).When<string>(s => s == "Enable");
			clean.To(final);
			idle.To(moveItem).When<string>(s => s == "TransformInput");
			moveItem.To(idle).When<string>(s => s == "ReleaseInput");
			idle.To(showItemMovePattern).When<string>(s => s == "ReleaseInput");
			showItemMovePattern.To(hideItemMovePattern).When<string>(s => s == "ReleaseInput");
			hideItemMovePattern.To(idle);

			model.Validate();

			var instance = new StateMachineInstance("instance");

			model.Initialise(instance);

			model.Evaluate(instance, "ReleaseInput");
			model.Evaluate(instance, "Disable");
			model.Evaluate(instance, "Enable");

			Trace.Assert(instance.GetCurrent(on.DefaultRegion) == showItemMovePattern, "History semantics should set current state to " + showItemMovePattern.Name);

			model.Evaluate(instance, "ReleaseInput");
			model.Evaluate(instance, "Disable");
			model.Evaluate(instance, "Enable");

			Trace.Assert(instance.GetCurrent(on.DefaultRegion) == idle, "History semantics should set current state to " + idle.Name);
		}
	}
}
