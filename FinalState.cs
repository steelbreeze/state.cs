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
	public sealed class FinalState : StateBase
	{
		/// <summary>
		/// Creates a FinalState.
		/// </summary>
		/// <param name="name">The name of the FinalState.</param>
		/// <param name="parent">The paret Region of the FinalState.</param>
		public FinalState( String name, Region parent )
			: base( name, parent )
		{
			Trace.Assert( parent != null, "FinalState must have parent provided" );
		}

		/// <summary>
		/// Attempts to process a message.
		/// </summary>
		/// <param name="message">The message to process.</param>
		/// <returns>A Boolean indicating if the message was processed.</returns>
		/// <remarks>
		/// Final states will never process a message as they have no outbound transitions or child regions.
		/// </remarks>
		override public Boolean Process( Object message ) { return false; }

		/// <summary>
		/// Accepts a Visitor object.
		/// </summary>
		/// <typeparam name="TContext">The type of the context to pass to the visitor.</typeparam>
		/// <param name="visitor">The visitor object.</param>
		/// <param name="context">The context to pass while visiting the CompositeState.</param>
		/// <returns>Context to pass on to sibling Vertices within the parent Region.</returns>
		override public TContext Accept<TContext>( Visitor<TContext> visitor, TContext context )
		{
			return visitor.Visit( this, base.Accept( visitor, context ) );
		}
	}
}