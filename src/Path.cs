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
using System.Linq;

namespace Steelbreeze.Behavior
{
	// the path between any pair of elements within a state machine hierarchy
	internal sealed class Path
	{
		internal readonly Action<IState> exit;
		internal readonly Action<IState> beginEnter;
		internal readonly Action<IState, Boolean> endEnter;

		internal Path( Element source, Element target )
		{
			var sourceAncestors = source.Owner.Ancestors;
			var targetAncestors = target.Owner.Ancestors;
			var ignoreAncestors = 0;

			while( sourceAncestors.Count > ignoreAncestors && targetAncestors.Count > ignoreAncestors && sourceAncestors[ ignoreAncestors ].Equals( targetAncestors[ ignoreAncestors ] ) )
				ignoreAncestors++;

			this.exit = source.BeginExit;
			this.exit += source.EndExit;

			foreach( var sourceAncestor in sourceAncestors.Skip( ignoreAncestors ).Reverse() )
				this.exit += sourceAncestor.EndExit;

			foreach( var targetAncestor in targetAncestors.Skip( ignoreAncestors ) )
				this.beginEnter += targetAncestor.BeginEnter;
	
			this.beginEnter += target.BeginEnter;
			this.endEnter = target.EndEnter;
		}
	}
}
