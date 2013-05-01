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
	/// /// The base class and factory for transactions.
	/// </summary>
	public interface TransactionBase
	{
		/// <summary>
		/// Returns the uncommitted current state of a region
		/// </summary>
		/// <param name="region">The region to get the uncommitted state for</param>
		/// <returns>The uncommitted state of the region</returns>
		StateBase GetCurrent( Region region );

		/// <summary>
		/// Returns the uncommitted active status of a region
		/// </summary>
		/// <param name="region">The region to get the uncommitted active status for</param>
		/// <returns>The uncommitted active status</returns>
		Boolean GetActive( Region region );

		/// <summary>
		/// Returns the uncommitted active status of a state
		/// </summary>
		/// <param name="state">The state to get the uncommitted active status for</param>
		/// <returns>The uncommitted active status</returns>
		Boolean GetActive( StateBase state );

		/// <summary>
		/// Sets the uncommitted active status of a region
		/// </summary>
		/// <param name="region">The region to set the uncommitted active status for</param>
		/// <param name="value">The valuse to set the uncommitted active status to</param>
		void SetActive( Region region, Boolean value );

		/// <summary>
		/// Sets the uncommitted active status of a state
		/// </summary>
		/// <param name="state">The state to set the uncommitted active status for</param>
		/// <param name="value">The valuse to set the uncommitted active status to</param>
		void SetActive( StateBase state, Boolean value );

		/// <summary>
		/// Sets the uncommitted current state of a region
		/// </summary>
		/// <param name="region">The region to set the uncommitted current state for</param>
		/// <param name="value">The value to set the uncommitted current state to</param>
		void SetCurrent( Region region, StateBase value );

		/// <summary>
		/// Commits all uncommitted values
		/// </summary>
		void Commit();

		/// <summary>
		/// Rolls back all uncommitted values
		/// </summary>
		void Rollback();
	}
}