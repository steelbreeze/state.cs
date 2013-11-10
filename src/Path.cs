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
		private readonly Element[] exit;
		private readonly Element[] enter;

		internal Path( Element source, Element target )
		{
			var sourceAncestors = source.Ancestors;
			var targetAncestors = target.Ancestors;
			var uncommonAncestor = source.Owner.Equals( target.Owner ) ? sourceAncestors.Count - 1 : Uncommon( sourceAncestors, targetAncestors );

			this.exit = sourceAncestors.Skip( uncommonAncestor ).Reverse().ToArray();
			this.enter = targetAncestors.Skip( uncommonAncestor ).ToArray();
		}

		internal void Exit( IState context )
		{
			this.exit[ 0 ].BeginExit( context );

			foreach( var element in this.exit )
				element.EndExit( context );
		}

		internal void Enter( IState context, Boolean deepHistory )
		{
			foreach( var element in this.enter )
				element.BeginEnter( context );

			this.enter[ this.enter.Length - 1 ].EndEnter( context, deepHistory );
		}

		private static int Uncommon( IList<Element> sourceAncestors, IList<Element> targetAncestors, int index = 0 )
		{
			return sourceAncestors[ index ].Equals( targetAncestors[ index ] ) ? Uncommon( sourceAncestors, targetAncestors, ++index ) : index;
		}
	}
}
