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
	/// A node within a state machine that can be entered and exited.
	/// </summary>
	public abstract class StateMachineBase
	{
		virtual internal void OnExit( TransactionBase transaction )
		{
			Debug.WriteLine( this, "Leave" );
		}

		virtual internal void BeginEnter( TransactionBase transaction )
		{
			Debug.WriteLine( this, "Enter" );
		}

		/// <summary>
		/// Accepts a Visitor object and visits all child nodes.
		/// </summary>
		/// <typeparam name="TContext">The type of the context to pass while visiting the node.</typeparam>
		/// <param name="visitor">The Visitor object.</param>
		/// <param name="context">The context to pass while visiting the node.</param>
		/// <returns>Context to pass on to sibling nodes within the parent node.</returns>
		virtual public TContext Accept<TContext>( Visitor<TContext> visitor, TContext context = default( TContext ) )
		{
			return visitor.Visit( this, context );
		}
	}
}