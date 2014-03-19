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