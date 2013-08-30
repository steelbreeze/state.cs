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
using System.Diagnostics;
using System.Linq;

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// Common base class for all transition types
	/// </summary>
	public abstract class TransitionBase
	{
		readonly internal Action<IState> onExit;
		readonly internal Action<IState> onBeginEnter;
		readonly internal Action<IState, Boolean> onEndEnter;

		internal TransitionBase( Vertex source, Vertex target )
		{
			if( target != null )
			{
				var sourceAncestors = source.Ancestors.Reverse().GetEnumerator();
				var targetAncestors = target.Ancestors.Reverse().GetEnumerator();

				while( sourceAncestors.MoveNext() && targetAncestors.MoveNext() && sourceAncestors.Current.Equals( targetAncestors.Current ) ) { }

				if( source is PseudoState && !sourceAncestors.Current.Equals( source ) )
					onExit += source.OnExit;

				onExit += sourceAncestors.Current.OnExit;

				do { onBeginEnter += targetAncestors.Current.OnBeginEnter; } while( targetAncestors.MoveNext() );

				onEndEnter += target.OnEndEnter;
			}
		}
	}
}