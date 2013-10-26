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

namespace Steelbreeze.Behavior
{
	internal class Path
	{
		internal Action<IState> exit;
		internal Action<IState> enter;

		internal Path( IVertex source, IVertex target )
		{
			var sourceAncestors = Ancestors( source ).GetEnumerator();
			var targetAncestors = Ancestors( target ).GetEnumerator();

			while( sourceAncestors.MoveNext() && targetAncestors.MoveNext() && sourceAncestors.Current.Equals( targetAncestors.Current ) ) { }

			if( source is PseudoState && !sourceAncestors.Current.Equals( source ) )
				exit += source.Exit;

			exit += sourceAncestors.Current.Exit; // TODO: add all exits like enter

			do { enter += targetAncestors.Current.Enter; } while( targetAncestors.MoveNext() );
		}

		internal static List<IElement> Ancestors( IElement element )
		{
			var ancestors = element.Owner != null ? Ancestors( element.Owner ) : new List<IElement>();

			ancestors.Add( element );

			return ancestors;
		}
	}
}
