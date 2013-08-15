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
	public class SimpleState : Vertex
	{
		private static Func<IEnumerable<Completion>, Completion> GetCompletion = completions => completions.SingleOrDefault( c => c.guard() ); 
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
		/// <param name="owner">The parent Region or the State.</param>
		public SimpleState( String name, Region owner ) : base( name, owner, GetCompletion ) { }

		/// <summary>
		/// Initialises a node to its initial state.
		/// </summary>
		/// <param name="state">An optional transaction that the process operation will participate in.</param>
		public void Initialise( IState state )
		{
			OnEnter( state );
			Complete( state, false );
		}

		override internal void OnExit( IState state )
		{
			OnExit();

			state.SetActive( this, false );

			base.OnExit( state );
		}

		override internal void OnEnter( IState state )
		{
			if( state.GetActive( this ) )
				OnExit( state );

			base.OnEnter( state );

			state.SetActive( this, true );

			if( this.Owner != null )
				state.SetCurrent( this.Owner, this );

			OnEnter();
		}

		/// <summary>
		/// Calls the state's entry behaviour
		/// </summary>
		/// <remarks>
		/// Override this method to implement more complex state entry behaviour
		/// </remarks>
		public virtual void OnExit()
		{
			if( Exit != null )
				Exit();
		}

		/// <summary>
		/// Calls the state's entry behaviour
		/// </summary>
		/// <remarks>
		/// Override this method to implement more complex state entry behaviour
		/// </remarks>
		public virtual void OnEnter()
		{
			if( Entry != null )
				Entry();
		}

		/// <summary>
		/// Attempts to process a message.
		/// </summary>
		/// <param name="message">The message to process.</param>
		/// <param name="state">An optional transaction that the process operation will participate in.</param>
		/// <returns>A Boolean indicating if the message was processed.</returns>
		override public Boolean Process( IState state, Object message )
		{
			if( state.IsTerminated )
				return false;

			if( this.transitions == null )
				return false;

			var transition = this.transitions.SingleOrDefault( t => t.Guard( message ) );

			if( transition == null )
				return false;

			transition.Traverse( state, message );

			return true;
		}
	}
}