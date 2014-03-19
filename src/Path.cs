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
	// the path between any pair of elements within a state machine hierarchy
	internal sealed class Path<TState> where TState : IState<TState>
	{
		internal readonly Action<TState> exit;
		internal readonly Action<TState> beginEntry;
		internal readonly Action<TState, Boolean> endEntry;

		internal Path( Element<TState> source, Element<TState> target )
		{
			var sourceAncestors = Ancestors( source.Owner );
			var targetAncestors = Ancestors( target.Owner );
			var ignoreAncestors = 0;

			while( sourceAncestors.Count > ignoreAncestors && targetAncestors.Count > ignoreAncestors && sourceAncestors[ ignoreAncestors ].Equals( targetAncestors[ ignoreAncestors ] ) )
				ignoreAncestors++;

			this.exit = source.BeginExit;
			this.exit += source.EndExit;

			foreach( var sourceAncestor in sourceAncestors.Skip( ignoreAncestors ).Reverse() )
				this.exit += sourceAncestor.EndExit;

			foreach( var targetAncestor in targetAncestors.Skip( ignoreAncestors ) )
				this.beginEntry += targetAncestor.BeginEntry;

			this.beginEntry += target.BeginEntry;
			this.endEntry = target.EndEntry;
		}

		private static IList<Element<TState>> Ancestors( Element<TState> element )
		{
			var ancestors = element.Owner != null ? Ancestors( element.Owner ) : new List<Element<TState>>();

			ancestors.Add( element );

			return ancestors;
		}
	}
}