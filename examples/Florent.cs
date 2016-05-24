using System;
using Steelbreeze.StateMachines.Model;
using Steelbreeze.StateMachines.Runtime;

namespace Steelbreeze.Behavior.StateMachines.Examples {
	/// <summary>
	/// Example machine showing history modelling.
	/// </summary>
	public static class Florent {
		/// <summary>
		/// Entry point
		/// </summary>
		public static void Main() {
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

			var instance = new StateMachineInstance("instance");

			model.Initialise(instance);

			model.Evaluate(instance, "ReleaseInput");
			model.Evaluate(instance, "Disable");
			model.Evaluate(instance, "Enable");

			Console.WriteLine("Press any key...");
			Console.ReadKey();
		}
	}
}
