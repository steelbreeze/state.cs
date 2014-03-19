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
