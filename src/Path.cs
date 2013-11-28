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
		private readonly Element source;
		private readonly Action<IState> elementsToExit;
		private readonly Action<IState> elementsToEnter;
		private readonly Element target;

		internal Path( Element source, Element target )
		{
			var sourceAncestors = source.Owner.Ancestors;
			var targetAncestors = target.Owner.Ancestors;
			var lca = LCA( sourceAncestors, targetAncestors );

			this.source = source;
			this.elementsToExit += source.EndExit;

			foreach( var sourceAncestor in sourceAncestors.Skip( lca + 1 ).Reverse() )
				this.elementsToExit += sourceAncestor.EndExit;

			foreach( var targetAncestor in targetAncestors.Skip( lca + 1 ) )
				this.elementsToEnter += targetAncestor.BeginEnter;
	
			this.elementsToEnter += target.BeginEnter;
			this.target = target;
		}

		internal void Exit( IState context )
		{
			this.source.BeginExit( context );
			this.elementsToExit( context );
		}

		internal void Enter( IState context, Boolean deepHistory )
		{
			this.elementsToEnter( context );
			this.target.EndEnter( context, deepHistory );
		}

		private static int LCA( IList<Element> sourceAncestry, IList<Element> targetAncestry ) 
		{
			int common = 0;

			while( sourceAncestry.Count > common && targetAncestry.Count > common && sourceAncestry[ common ].Equals( targetAncestry[ common ] ) )
				common ++;

			return common - 1;
		}
	}
}
