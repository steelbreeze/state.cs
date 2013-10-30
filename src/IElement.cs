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

namespace Steelbreeze.Behavior
{
	// Represents an element within a state machine hierarchy
	internal interface IElement
	{
		// The owning (parent) element of the element
		IElement Owner { get; }

		// recursive exiting of an element
		void BeginExit( IState context );

		// non-recursive exit of an element
		void EndExit( IState context );

		// non-recursive entry to an element
		void BeginEnter( IState context );

		// recursive exiting of an element
		void EndEnter( IState context, Boolean deepHistory );
	}
}
