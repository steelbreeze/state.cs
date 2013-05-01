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
		internal HashSet<Transition> completions = null;

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

			if( ( Kind = kind ).IsInitial )
			{
				Trace.Assert( parent.initial == null, "Region may have only one initial PseudoState (Initial, EntryPoint, DeepHistory, ShallowHistory)" );

				parent.initial = this;
			}
		}

		internal override void EndEnter( TransactionBase transaction, bool deepHistory )
		{
			Kind.GetCompletion( completions ).Traverse( transaction, deepHistory );
		}

		/// <summary>
		/// Accepts a Visitor object.
		/// </summary>
		/// <typeparam name="TContext">The type of the context to pass while visiting the CompositeState.</typeparam>
		/// <param name="visitor">The Visitor object.</param>
		/// <param name="context">The context to pass while visiting the CompositeState.</param>
		/// <returns>Context to pass on to sibling Vertices within the parent Region.</returns>
		override public TContext Accept<TContext>( Visitor<TContext> visitor, TContext context )
		{
			return visitor.Visit( this, base.Accept( visitor, context ) );
		}

		/// <summary>
		/// Displays the fully qualified name of the Region or Vertex
		/// </summary>
		/// <returns></returns>
		override public String ToString()
		{
			return Parent == null ? Kind.Name : Parent + "." + Kind.Name;
		}
	}
}