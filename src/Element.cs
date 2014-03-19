// Copyright © 2014 Steelbreeze Limited.
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
	/// Represents an element within a state machine heirarchy
	/// </summary>
	public abstract class Element<TState> where TState : IState<TState>
	{
		/// <summary>
		/// The name of the element.
		/// </summary>
		public readonly String Name;

		/// <summary>
		/// The owning parent element of the element
		/// </summary>
		public readonly Element<TState> Owner;

		/// <summary>
		/// Returns the fully qualified name of the element
		/// </summary>
		public String QualifiedName { get { return this.Owner != null ? this.Owner.QualifiedName + "." + this.Name : this.Name; } }

		internal Element( String name, Element<TState> owner )
		{
			this.Name = name;
			this.Owner = owner;
		}

		internal virtual void BeginExit( TState state ) { }

		internal virtual void EndExit( TState state )
		{
			Debug.WriteLine( this.QualifiedName, "Leave" );

			state.SetActive( this, false );
		}

		internal virtual void BeginEntry( TState state )
		{
			if( state.GetActive( this ) ) // NOTE: this check is required to cater for edge cases involving external transitions between orthogonal regions; we like to keep the entry/exit count for elements the same, so we exit an element that active before reentering it
			{
				BeginExit( state );
				EndExit( state );
			}

			Debug.WriteLine( this.QualifiedName, "Enter" );

			state.SetActive( this, true );
		}

		internal virtual void EndEntry( TState state, bool deepHistory ) { }

		/// <summary>
		/// Returns the name of the element.
		/// </summary>
		/// <returns>The fully qualified name of the element.</returns>
		public override string ToString()
		{
			return this.Name;
		}
	}
}
