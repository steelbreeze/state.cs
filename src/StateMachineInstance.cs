/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Collections.Generic;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// A simple class to extend as a base for state machine intsances.
	/// </summary>
	public class StateMachineInstance : StateMachineInstanceBase<StateMachineInstance> {
		/// <summary>
		/// The name of the state machine instance
		/// </summary>
		public readonly String Name;

		/// <summary>
		/// Create a new instance of hte StateMachineInstance class.
		/// </summary>
		/// <param name="name">The name of the state machin instance</param>
		public StateMachineInstance (String name) {
			this.Name = name;
		}

		/// <summary>
		/// Returns the name of the state machine instance
		/// </summary>
		/// <returns>The name of the state machine instance</returns>
		public override string ToString () {
			return this.Name;
		}
	}
}