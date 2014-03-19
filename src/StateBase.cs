// The MIT License (MIT)
//
// Copyright (c) 2014 Steelbreeze Limited
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
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
