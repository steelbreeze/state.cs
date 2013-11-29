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
	public partial class Transition
	{
		/// <summary>
		/// An else continuation transition; used as the default path from choice or junction pseudo states
		/// </summary>
		public sealed class Else : Transition
		{
			/// <summary>
			/// Creates an else completion transition between pseudo states.
			/// </summary>
			/// <param name="source">The source pseudo state.</param>
			/// <param name="target">The target pseudo state.</param>
			public Else( PseudoState source, PseudoState target )
				: base( source, target, False )
			{
				Trace.Assert( source.Kind == PseudoStateKind.Choice || source.Kind == PseudoStateKind.Junction, "Else can only originate from choice or junction pseudo states" );
			}

			/// <summary>
			/// Creates an else completion transition from a pseudo state to a state.
			/// </summary>
			/// <param name="source">The source pseudo state.</param>
			/// <param name="target">The target state.</param>
			public Else( PseudoState source, SimpleState target )
				: base( source, target, False )
			{
				Trace.Assert( source.Kind == PseudoStateKind.Choice || source.Kind == PseudoStateKind.Junction, "Else can only originate from choice or junction pseudo states" );
			}
		}
	}
}
