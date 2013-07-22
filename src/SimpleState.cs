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
using System.Linq;

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// A condition or situation during the life of an object during which it satisfies some condition, performs some activity, or waits for some event.
	/// </summary>
	public class SimpleState : StateBase
	{
		internal HashSet<Completion> completions = null;
		internal HashSet<TypedTransition> transitions = null;

		/// <summary>
		/// The action or actions performed when entering a State.
		/// </summary>
		public event Action Entry;

		/// <summary>
		/// The action or actions performed when leaving a State.
		/// </summary>
		public event Action Exit;

		/// <summary>
		/// Creates a State.
		/// </summary>
		/// <param name="name">The name of the State.</param>
		/// <param name="parent">The parent Region or the State.</param>
		public SimpleState( String name, Region parent = null ) : base( name, parent ) { }

		/// <summary>
		/// Tests to see if a state is complete
		/// </summary>
		/// <param name="state">The state machine state</param>
		/// <returns>True if the state is complete</returns>
		/// <remarks>
		/// A SimpleState is always deemed to be complete.
		/// </remarks>
		virtual public Boolean IsComplete( IState state )
		{
			return true;
		}

		/// <summary>
		/// Initialises a node to its initial state.
		/// </summary>
		/// <param name="state">An optional transaction that the process operation will participate in.</param>
		public void Initialise( IState state )
		{
			Initialise( state, false );
		}

		override internal void OnExit( IState state )
		{
			if( Exit != null )
				Exit();

			base.OnExit( state );
		}

		override internal void OnEnter( IState state )
		{
			base.OnEnter( state );

			if( Entry != null )
				Entry();
		}

		internal override void Complete( IState state, bool deepHistory )
		{
			if( completions != null )
			{
				if( IsComplete( state ) )
				{
					var completion = completions.SingleOrDefault( c => c.guard() );

					if( completion != null )
						completion.Traverse( state, deepHistory );
				}
			}
		}

		/// <summary>
		/// Attempts to process a message.
		/// </summary>
		/// <param name="message">The message to process.</param>
		/// <param name="state">An optional transaction that the process operation will participate in.</param>
		/// <returns>A Boolean indicating if the message was processed.</returns>
		override public Boolean Process( IState state, Object message )
		{
			TypedTransition transition;

			return this.transitions != null && ( transition = this.transitions.SingleOrDefault( t => t.Guard( message ) ) ) != null && transition.Traverse( state, message );
		}
	}
}