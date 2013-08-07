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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// A node within a state machine that can be entered and exited.
	/// </summary>
	/// <remarks>
	/// This draws a little from the UML 2 NamedElement class, but adds some state machine characteristics
	/// </remarks>
	public abstract class StateMachineElement 
	{
		/// <summary>
		/// The name of the element
		/// </summary>
		public String Name { get; private set; }

		/// <summary>
		/// The fully qualified name of the element
		/// </summary>
		public String QualifiedName { get { return Ancestors.Select( ancestor => ancestor.Name ).Aggregate( ( right, left ) => left + "." + right ); } }

		/// <summary>
		/// The parent of the element
		/// </summary>
		public abstract StateMachineElement Parent { get; }

		// The ancestors of the element
		internal IEnumerable<StateMachineElement> Ancestors { get { for( StateMachineElement element = this; element != null; element = element.Parent ) yield return element; } }

		internal StateMachineElement( String name )
		{
			Trace.Assert( name != null, "All state machine elements must have name provided" );

			this.Name = name;
		}

		virtual internal void OnExit( IState state )
		{
			Debug.WriteLine( this.QualifiedName, "Leave" );
		}

		virtual internal void OnEnter( IState state )
		{
			Debug.WriteLine( this.QualifiedName, "Enter" );
		}

		/// <summary>
		/// Returns the QualifiedName of the element
		/// </summary>
		/// <returns>The QualifiedName of the element</returns>
		public override String ToString()
		{
			return this.QualifiedName;
		}
	}
}