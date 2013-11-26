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
using System.ComponentModel;

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// A final state is a state that denotes its parent region or composite state is complete.
	/// </summary>
	public sealed class FinalState : SimpleState
	{
		/// <summary>
		/// The final state's entry action (do not set this)
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete( "Entry actions are not permitted for FinalState", true )]
		new public Action Entry { get { return null; } set { throw new Exception( "FinalState cannot have an entry action" ); } }

		/// <summary>
		/// The final state's exit action (do not set this)
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		[Obsolete( "Exit actions are not permitted for FinalState", true )]
		new public Action Exit { get { return null; } set { throw new Exception( "FinalState cannot have an exit action" ); } }

		/// <summary>
		/// Creates a final state within an owning (parent) region.
		/// </summary>
		/// <param name="name">The name of the final state.</param>
		/// <param name="owner">The owning (parent) region.</param>
		public FinalState( String name, Region owner ) : base( name, owner ) { }

		/// <summary>
		/// Creates a final state within an owning (parent) composite state.
		/// </summary>
		/// <param name="name">The name of the final state.</param>
		/// <param name="owner">The owning (parent) composite state.</param>
		public FinalState( String name, CompositeState owner ) : base( name, owner ) { }
	}
}
