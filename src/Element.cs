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
	/// Represents an element within a state machine heirarchy
	/// </summary>
	public abstract class Element
	{
		/// <summary>
		/// The name of the element.
		/// </summary>
		public String Name { get; private set; }

		/// <summary>
		/// The owning parent element of the element
		/// </summary>
		public Element Owner { get; private set; }

		/// <summary>
		/// Returns the fully qualified name of the element
		/// </summary>
		public String QualifiedName { get { return this.Ancestors.Select( element => element.Name ).Aggregate( ( left, right ) => left + "." + right ); } }

		internal Element( String name, Element owner )
		{
			this.Name = name;
			this.Owner = owner;
		}

		internal IList<Element> Ancestors
		{
			get
			{
				var ancestors = this.Owner != null ? this.Owner.Ancestors : new List<Element>();

				ancestors.Add( this );

				return ancestors;
			}
		}

		internal virtual void BeginExit( IState context ) { }

		internal virtual void EndExit( IState context )
		{
			Debug.WriteLine( this, "Leave" );

			context.SetActive( this, false );
		}

		internal virtual void BeginEnter( IState context )
		{
			if( context.GetActive( this ) )
				EndExit( context );

			Debug.WriteLine( this, "Enter" );

			context.SetActive( this, true );

		}

		internal virtual void EndEnter( IState context, bool deepHistory ) { }

		/// <summary>
		/// Returns the fully qualified name of the element.
		/// </summary>
		/// <returns>The fully qualified name of the element.</returns>
		public override string ToString()
		{
			return this.QualifiedName;
		}
	}
}
