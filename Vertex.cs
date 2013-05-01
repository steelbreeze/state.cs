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
	/// A Vertex is a node within a state machine that can be the source or target of a transition.
	/// </summary>
	public abstract class Vertex : StateMachineBase
	{
		/// <summary>
		/// The Vertex's parent Region.
		/// </summary>
		public Region Parent { get; private set; }

		internal Vertex( Region parent )
		{
			if( ( this.Parent = parent ) != null )
				parent.vertices.Add( this );
		}
		
		internal void Initialise( TransactionBase transaction, Boolean deepHistory )
		{
			BeginEnter( transaction );
			EndEnter( transaction, deepHistory );
		}

		virtual internal void EndEnter( TransactionBase transaction, Boolean deepHistory ) { }

		/// <summary>
		/// Accepts a Visitor object and visits all child Regions.
		/// </summary>
		/// <typeparam name="TContext">The type of the context to pass while visiting the CompositeState.</typeparam>
		/// <param name="visitor">The Visitor object.</param>
		/// <param name="context">The context to pass while visiting the CompositeState.</param>
		/// <returns>Context to pass on to sibling Vertics within the parent Region.</returns>
		override public TContext Accept<TContext>( Visitor<TContext> visitor, TContext context = default( TContext ) )
		{
			return visitor.Visit( this, base.Accept( visitor, context ) );
		}
	}
}