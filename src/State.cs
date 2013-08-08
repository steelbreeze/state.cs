// Copyright © 2013 Steelbreeze Limited.
// This file is part of state.cs.
//
// state.cs is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published
// by the Free Software Foundation, either version 3 of the License,
// or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// Basic example of state machine state implementation
	/// </summary>
	public sealed class State : IState
	{
		private Dictionary<StateMachineElement, Boolean> active = new Dictionary<StateMachineElement, Boolean>();
		private Dictionary<StateMachineElement, Vertex> current = new Dictionary<StateMachineElement, Vertex>();

		/// <summary>
		/// Returns the current state of a region
		/// </summary>
		/// <param name="element">The region to get the state for</param>
		/// <returns>The uncommitted state of the region</returns>
		public Vertex GetCurrent( StateMachineElement element )
		{
			Vertex current = null;

			this.current.TryGetValue( element, out current );

			return current;
		}

		/// <summary>
		/// Returns the active status of a state
		/// </summary>
		/// <param name="element">The state to get the active status for</param>
		/// <returns>The uncommitted active status</returns>
		public Boolean GetActive( StateMachineElement element )
		{
			Boolean active = false;

			this.active.TryGetValue( element, out active );

			return active;
		}

		/// <summary>
		/// Sets the active status of a state
		/// </summary>
		/// <param name="element">The state to set the active status for</param>
		/// <param name="value">The valuse to set the active status to</param>
		public void SetActive( StateMachineElement element, Boolean value )
		{
			this.active[ element ] = value;
		}

		/// <summary>
		/// Sets the current state of a region
		/// </summary>
		/// <param name="element">The region to set the current state for</param>
		/// <param name="value">The value to set the current state to</param>
		public void SetCurrent( StateMachineElement element, Vertex value )
		{
			this.current[ element ] = value;
		}
	}
}