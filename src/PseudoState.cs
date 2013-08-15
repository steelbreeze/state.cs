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
		/// <summary>
		/// The kind of pseudostate that determines its behaviour.
		/// </summary>
		public PseudoStateKind Kind { get; private set; }

		/// <summary>
		/// Creates a PseudoState.
		/// </summary>
		/// <param name="name">The name of the pseudo state.</param>
		/// <param name="kind">The kind of the PseudoState.</param>
		/// <param name="owner">The parent Region of the PseudoState.</param>
		public PseudoState( String name, PseudoStateKind kind, Region owner )
			: base( name, owner, kind.GetCompletion )
		{
			Trace.Assert( kind != null, "PseudoStateKind must be provided" );
			Trace.Assert( owner != null, "PseudoState must have an owner" );

			if( ( this.Kind = kind ).IsInitial )
				owner.initial = this;
		}

		internal override void OnEnter( IState state )
		{
			base.OnEnter( state );

			if( this.Kind == PseudoStateKind.Terminated )
				state.IsTerminated = true;
		}

		/// <summary>
		/// Attempts to process a message.
		/// </summary>
		/// <param name="message">The message to process.</param>
		/// <param name="state">An optional transaction that the process operation will participate in.</param>
		/// <returns>A Boolean indicating if the message was processed.</returns>
		/// <remarks>Note that pseudo states are transient so will therefore never be able to process a message itself.</remarks>
		public override bool Process( IState state, Object message )
		{
			return false;
		}
	}
}