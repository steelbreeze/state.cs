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
using System.Diagnostics;

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// A State that has no outgoing transitions.
	/// </summary>
	public sealed class FinalState : Vertex
	{
		/// <summary>
		/// Creates a FinalState.
		/// </summary>
		/// <param name="name">The name of the FinalState.</param>
		/// <param name="owner">The paret Region of the FinalState.</param>
		public FinalState( String name, Region owner ) : base( name, owner, null ) { }

		override internal void OnExit( IState state )
		{
			state.SetActive( this, false );

			base.OnExit( state );
		}

		override internal void OnEnter( IState state )
		{
			if( state.GetActive( this ) )
				OnExit( state );

			base.OnEnter( state );

			state.SetActive( this, true );

			if( this.Owner != null )
				state.SetCurrent( this.Owner, this );
		}

		/// <summary>
		/// Attempts to process a message.
		/// </summary>
		/// <param name="message">The message to process.</param>
		/// <param name="state">An optional transaction that the process operation will participate in.</param>
		/// <returns>A Boolean indicating if the message was processed.</returns>
		/// <remarks>Note that a final state has no outbound transitions so will therefore never be able to process a message itself.</remarks>
		public override bool Process( IState state, Object message )
		{
			return false;
		}
	}
}