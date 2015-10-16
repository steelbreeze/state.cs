/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
using Steelbreeze.StateMachines.Model;

namespace Steelbreeze.StateMachines.Tests {
	public class Instance : StateMachineInstance<Instance> {
		public readonly string Name;

		public int Int1 { get; set; }
		public int Int2 { get; set; }
		public int Int3 { get; set; }

		public Instance (string name) {
			this.Name = name;
		}

		public override string ToString () {
			return this.Name;
		}
	}
}
