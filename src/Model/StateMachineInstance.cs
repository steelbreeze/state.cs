/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
using System.Collections.Generic;

namespace Steelbreeze.StateMachines.Model {
	/// <summary>
	/// Represents a state machine instance at runtime.
	/// </summary>
	/// <typeparam name="TInstance">The derived type of the state machine instance.</typeparam>
	public abstract class StateMachineInstance<TInstance> : IInstance<TInstance> where TInstance : IInstance<TInstance> {
		private bool terminated = false;
		private Dictionary<Region<TInstance>, State<TInstance>> current = new Dictionary<Region<TInstance>, State<TInstance>>();

		/// <summary>
		/// Flag indicating if the state machine instance has been terminated.
		/// </summary>
		/// <remarks>A state machine instance is terminated as soon as it reaches a terminate pseudo state.</remarks>
		public bool IsTerminated { get { return this.terminated; } }

		/// <summary>
		/// sets the state machine instance to a terminated state; use inheritance method to hide this interface from derived classes.
		/// </summary>
		public void Terminate() {
			this.terminated = true;
		}

		/// <summary>
		/// sets or updates the child state for a region; use inheritance method to hide this interface from derived classes.
		/// </summary>
		public void SetCurrent (Region<TInstance> region, State<TInstance> state) {
			current[ region ] = state;
		}

		/// <summary>
		/// Retreives the last known active child state of a region.
		/// </summary>
		/// <param name="region">The region to get the current active child state for.</param>
		/// <returns>The last known active child state of the region or null if the region has never been entered.</returns>
		/// <remarks>The last known child state of the region is maintained in order to correctly manage history semantics.</remarks>
		public State<TInstance> GetCurrent (Region<TInstance> region) {
			var value = default(State<TInstance>);

			this.current.TryGetValue(region, out value);

			return value;
		}
	}

	/// <summary>
	/// Represents a state machine instance at runtime.
	/// </summary>
	public sealed class StateMachineInstance : StateMachineInstance<StateMachineInstance> {
		/// <summary>
		/// The name of the state machine instance.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// Creates a new instance of the StateMachine Instance class.
		/// </summary>
		/// <param name="name">The name of the new instance.</param>
		public StateMachineInstance (string name) {
			this.Name = name;
		}

		/// <summary>
		/// Returns the name of the instance.
		/// </summary>
		/// <returns>The name of the instance.</returns>
		public override string ToString () {
			return this.Name;
		}
	}
}