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
using System.Collections.Generic;

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// General purpose implementation of IState
	/// </summary>
	/// <typeparam name="TState">The type of the subclass that inherits from this abstract class</typeparam>
	/// <remarks>Subclass this as a simple, opaque implementation of IState</remarks>
	public abstract class StateBase<TState> : IState<TState> where TState : IState<TState>
	{
		private class ElementState
		{
			internal Boolean Active = false;
			internal SimpleState<TState> Current = null;
		}

		private Dictionary<Element<TState>, ElementState> state = new Dictionary<Element<TState>, ElementState>();

		private ElementState this[ Element<TState> key ]
		{
			get
			{
				ElementState value = null;

				if( !state.TryGetValue( key, out value ) )
					state.Add( key, value = new ElementState() );

				return value;
			}
		}

		Boolean IState<TState>.IsTerminated { get; set; }

		void IState<TState>.SetActive( Element<TState> element, bool value )
		{
			this[ element ].Active = value;
		}

		Boolean IState<TState>.GetActive( Element<TState> element )
		{
			return this[ element ].Active;
		}

		void IState<TState>.SetCurrent( Element<TState> element, SimpleState<TState> value )
		{
			this[ element ].Current = value;
		}

		SimpleState<TState> IState<TState>.GetCurrent( Element<TState> element )
		{
			return this[ element ].Current;
		}
	}
}
