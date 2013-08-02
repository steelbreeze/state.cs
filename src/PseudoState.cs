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

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// A transient Vertex within a Region.
	/// </summary>
	public sealed class PseudoState : Vertex
	{
		// the completion transitions from the pseudostate
		internal HashSet<Completion> completions = null;

		/// <summary>
		/// The kind of pseudostate that determines its behaviour.
		/// </summary>
		public PseudoStateKind Kind { get; private set; }

		/// <summary>
		/// Creates a PseudoState.
		/// </summary>
		/// <param name="kind">The kind of the PseudoState.</param>
		/// <param name="parent">The parent Region of the PseudoState.</param>
		public PseudoState( PseudoStateKind kind, Region parent )
			: base( parent )
		{
			Trace.Assert( kind != null, "PseudoStateKind must be provided" );
			Trace.Assert( parent != null, "PseudoState must have a parent" );

			if( ( this.Kind = kind ).IsInitial )
				parent.initial = this;
		}

		internal override void Complete( IState state, bool deepHistory )
		{
			Kind.GetCompletion( completions ).Traverse( state, deepHistory );
		}

		/// <summary>
		/// Displays the fully qualified name of the Region or Vertex
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Parent == null ? Kind.Name : Parent + "." + Kind.Name;
		}
	}
}