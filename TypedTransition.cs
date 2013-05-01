﻿// Copyright © 2013 Steelbreeze Limited.
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
	/// <summary>
	/// Common base for templatised transitions
	/// </summary>
	public abstract class TypedTransition : TransitionBase
	{
		internal TypedTransition( State source, Vertex target ) : base( source, target ) { }

		abstract internal Boolean Guard( Object message );
		abstract internal void Traverse( TransactionBase transaction, Object message );
	}
}