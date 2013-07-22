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
		private Dictionary<Region, StateBase> current = new Dictionary<Region, StateBase>();

		/// <summary>
		/// Returns the current state of a region
		/// </summary>
		/// <param name="region">The region to get the state for</param>
		/// <returns>The uncommitted state of the region</returns>
		public StateBase GetCurrent( Region region )
		{
			StateBase current = null;

			this.current.TryGetValue( region, out current );

			return current;
		}

		/// <summary>
		/// Returns the active status of a region
		/// </summary>
		/// <param name="region">The region to get the active status for</param>
		/// <returns>The uncommitted active status</returns>
		public Boolean GetActive( Region region )
		{
			Boolean active = false;

			this.active.TryGetValue( region, out active );

			return active;
		}

		/// <summary>
		/// Returns the active status of a state
		/// </summary>
		/// <param name="state">The state to get the active status for</param>
		/// <returns>The uncommitted active status</returns>
		public Boolean GetActive( StateBase state )
		{
			Boolean active = false;

			this.active.TryGetValue( state, out active );

			return active;
		}

		/// <summary>
		/// Sets the active status of a region
		/// </summary>
		/// <param name="region">The region to set the active status for</param>
		/// <param name="value">The valuse to set the active status to</param>
		public void SetActive( Region region, Boolean value )
		{
			this.active[ region ] = value;
		}

		/// <summary>
		/// Sets the active status of a state
		/// </summary>
		/// <param name="state">The state to set the active status for</param>
		/// <param name="value">The valuse to set the active status to</param>
		public void SetActive( StateBase state, Boolean value )
		{
			this.active[ state ] = value;
		}

		/// <summary>
		/// Sets the current state of a region
		/// </summary>
		/// <param name="region">The region to set the current state for</param>
		/// <param name="value">The value to set the current state to</param>
		public void SetCurrent( Region region, StateBase value )
		{
			this.current[ region ] = value;
		}
	}
}