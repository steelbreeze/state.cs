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
	/// Common base class for State and FinalState
	/// </summary>
	/// <remarks>
	/// A StateBase can be the current state of a Region.
	/// </remarks>
	abstract public class StateBase : Vertex
	{
		//		readonly private Region parent;
		//		readonly private String name;

		/// <summary>
		/// The name of the State
		/// </summary>
		public String Name { get; private set; }

		internal StateBase( String name, Region parent )
			: base( parent )
		{
			Trace.Assert( name != null, "State/FinalState must have name provided" );

			this.Name = name;

			if( parent != null )
				Trace.Assert( parent.vertices.OfType<StateBase>().Where( v => v.Name.Equals( name ) ).Count() == 1, "State/FinalState names must be unique within a Region." );
		}

		internal override void OnExit( IState state = null )
		{
			state.SetActive( this, false );

			base.OnExit( state );
		}

		internal override void OnEnter( IState state )
		{
			if( state.GetActive( this ) )
				OnExit( state );

			base.OnEnter( state );

			state.SetActive( this, true );

			if( this.Parent != null )
				state.SetCurrent( this.Parent, this );
		}

		/// <summary>
		/// Attempts to process a message.
		/// </summary>
		/// <param name="message">The message to process.</param>
		/// <param name="state">An optional transaction that the process operation will participate in.</param>
		/// <returns>A Boolean indicating if the message was processed.</returns>
		virtual public Boolean Process( IState state, Object message ) { return false; }

		/// <summary>
		/// Displays the fully qualified name of the Region or Vertex
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Parent == null ? Name : Parent + "." + Name;
		}
	}
}