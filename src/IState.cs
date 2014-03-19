// Copyright © 2014 Steelbreeze Limited.
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

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// Abstraction of a state machines state.
	/// </summary>
	public interface IState<TState> where TState : IState<TState>
	{
		/// <summary>
		/// Boolean indicating that the state machine is terminated.
		/// </summary>
		Boolean IsTerminated { get; set; }

		/// <summary>
		/// Sets the active flag for an element within the state machine model hierarchy.
		/// </summary>
		/// <param name="element">The element to set the active flag for.</param>
		/// <param name="value">The value of the active flag.</param>
		void SetActive( Element<TState> element, Boolean value );

		/// <summary>
		/// Returns the active flag for an element within the state machine model hierarchy.
		/// </summary>
		/// <param name="element">The element to get the active flag for.</param>
		/// <returns>The active flag for the given element.</returns>
		Boolean GetActive( Element<TState> element );

		/// <summary>
		/// Sets the current state for a given region or composite state.
		/// </summary>
		/// <param name="element">The region or composite state to set the current state for.</param>
		/// <param name="value">The current state.</param>
		void SetCurrent( Element<TState> element, SimpleState<TState> value );

		/// <summary>
		/// Gets the current state for a given region or composite state.
		/// </summary>
		/// <param name="element">The region or composite state to get the current state for.</param>
		/// <returns>The current state of the region or composite state.</returns>
		SimpleState<TState> GetCurrent( Element<TState> element );
	}
}
