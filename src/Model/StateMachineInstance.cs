/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
using System.Collections.Generic;

namespace Steelbreeze.StateMachines.Model {
	public abstract class StateMachineInstance<TInstance> : IInstance<TInstance> where TInstance : IInstance<TInstance> {
		private Dictionary<Region<TInstance>, State<TInstance>> current = new Dictionary<Region<TInstance>, State<TInstance>>();

		public bool IsTerminated { get; set; }

		public void SetCurrent (Region<TInstance> region, State<TInstance> state) {
			current[ region ] = state;
		}

		public State<TInstance> GetCurrent (Region<TInstance> region) {
			var value = default(State<TInstance>);

			this.current.TryGetValue(region, out value);

			return value;
		}
	}

	public class StateMachineInstance : StateMachineInstance<StateMachineInstance> {
		public readonly string Name;

		public StateMachineInstance (string name) {
			this.Name = name;
		}

		public override string ToString () {
			return this.Name ;
		}
	}
}