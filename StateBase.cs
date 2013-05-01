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
	abstract public class StateBase : Vertex
	{
		/// <summary>
		/// The name of the State
		/// </summary>
		public String Name { get; private set; }

		/// <summary>
		/// A flag indicating that the State is active (entered, but not yet exited)
		/// </summary>
		public Boolean IsActive { get; internal set; }

		internal StateBase( String name, Region parent ) 
			: base( parent ) 
		{
			Trace.Assert( name != null, "State/FinalState must have name provided" );

			Name = name;

			if( parent != null )
				Trace.Assert( parent.vertices.OfType<StateBase>().Where( v => v.Name.Equals( name ) ).Count() == 1, "State/FinalState names must be unique within a Region." );
		}

		internal override void OnExit( ITransaction transaction )
		{
			transaction.SetActive( this, false );

			base.OnExit( transaction );
		}

		internal override void BeginEnter( ITransaction transaction )
		{
			if( transaction.GetActive( this ) )
				OnExit( transaction );

			base.BeginEnter( transaction );

			transaction.SetActive( this, true );

			if( Parent != null )
				transaction.SetCurrent( Parent, this );
		}

		/// <summary>
		/// Attempts to process a message.
		/// </summary>
		/// <param name="message">The message to process.</param>
		/// <param name="transaction">An optional transaction that the process operation will participate in.</param>
		/// <returns>A Boolean indicating if the message was processed.</returns>
		virtual public Boolean Process( Object message, ITransaction transaction = null ) { return false; }

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

		/// <summary>
		/// Displays the fully qualified name of the Region or Vertex
		/// </summary>
		/// <returns></returns>
		override public String ToString()
		{
			return Parent == null ? Name : Parent + "." + Name;
		}
	}
}