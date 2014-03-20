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
using System.Linq;

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// Common base class for completion and event-based transitions
	/// </summary>
	/// <typeparam name="TState">The type of the state machine state under management</typeparam>
	public abstract class TransitionBase<TState> where TState : IState<TState>
	{
		/// <summary>
		/// The the elements to exit as as required by the transation
		/// </summary>
		internal protected readonly Action<TState> Exit;

		/// <summary>
		/// The elements to enter as required by the transition
		/// </summary>
		internal protected readonly Action<TState> BeginEntry;

		/// <summary>
		/// The element to cascade the entry operation to after the transtion completes
		/// </summary>
		internal protected readonly Action<TState, Boolean> EndEntry;

		/// <summary>
		/// Creates a new transtion
		/// </summary>
		/// <param name="source">The source element to transition from</param>
		/// <param name="target">The target element to transition to</param>
		/// <remarks>The TransitionBase calculates the path from source to target elements using a Least Common Ancestor method</remarks>
		internal TransitionBase( Element<TState> source, Element<TState> target )
		{
			if( target != null )
			{
				var sourceAncestors = Ancestors( source.Owner );
				var targetAncestors = Ancestors( target.Owner );
				var ignoreAncestors = 0;

				while( sourceAncestors.Count > ignoreAncestors && targetAncestors.Count > ignoreAncestors && sourceAncestors[ ignoreAncestors ].Equals( targetAncestors[ ignoreAncestors ] ) )
					ignoreAncestors++;

				this.Exit = source.BeginExit;
				this.Exit += source.EndExit;

				foreach( var sourceAncestor in sourceAncestors.Skip( ignoreAncestors ).Reverse() )
					this.Exit += sourceAncestor.EndExit;

				foreach( var targetAncestor in targetAncestors.Skip( ignoreAncestors ) )
					this.BeginEntry += targetAncestor.BeginEntry;

				this.BeginEntry += target.BeginEntry;
				this.EndEntry = target.EndEntry;
			}
		}

		/// <summary>
		/// Returns the ancestors of an element
		/// </summary>
		/// <param name="element">The element to get the ancesstors of</param>
		/// <returns>The ancestors of the element</returns>
		private static IList<Element<TState>> Ancestors( Element<TState> element )
		{
			var ancestors = element.Owner != null ? Ancestors( element.Owner ) : new List<Element<TState>>();

			ancestors.Add( element );

			return ancestors;
		}
	}
}