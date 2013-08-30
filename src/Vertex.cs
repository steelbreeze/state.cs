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
	/// A Vertex is a node within a state machine model that can be the source or target of a transition.
	/// </summary>
	public abstract class Vertex : StateMachineElement
	{
		private readonly Func<IEnumerable<Completion>, Completion> getCompletion;
		internal HashSet<Completion> completions = null;

		internal Vertex( String name, Region owner, Func<IEnumerable<Completion>, Completion> getCompletion )
			: base( name, owner )
		{
			this.getCompletion = getCompletion;
		}

		internal Vertex( String name, CompositeState owner, Func<IEnumerable<Completion>, Completion> getCompletion )
			: base( name, owner )
		{
			this.getCompletion = getCompletion;
		}

		internal virtual void OnEndEnter( IState state, Boolean deepHistory )
		{
			if( completions != null )
			{
				if( IsComplete( state ) )
				{
					var completion = getCompletion( completions );

					if( completion != null )
						completion.Traverse( state, deepHistory );
				}
			}
		}
	}
}