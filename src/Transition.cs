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
	/// An event-based transition from a state.
	/// </summary>
	/// <typeparam name="TMessage">The type of the message that can trigger the transition.</typeparam>
	public class Transition<TMessage> : ITransition where TMessage : class
	{
		private Action<IState> onExit;
		private Action<IState> onBeginEnter;
		private Func<TMessage, Boolean> guard;
		private IVertex target;

		/// <summary>
		/// The action(s) to perform while traversing the transition.
		/// </summary>
		public event Action<TMessage> Effect;

		/// <summary>
		/// Creates a transition from a state to a pseudo state.
		/// </summary>
		/// <param name="source">The source state.</param>
		/// <param name="target">The target pseudo state.</param>
		/// <param name="guard">The guard condition to be tested in order to follow the transition.</param>
		/// <remarks>This type of transition initiates a compound transition.</remarks>
		public Transition( SimpleState source, PseudoState target, Func<TMessage, Boolean> guard = null )
		{
			this.target = target;
			this.guard = guard;

			Completion.Path( source, target, ref onExit, ref onBeginEnter );

			( source.transitions ?? ( source.transitions = new HashSet<ITransition>() ) ).Add( this );
		}

		/// <summary>
		/// Creates a transition between states.
		/// </summary>
		/// <param name="source">The source state.</param>
		/// <param name="target">The target state.</param>
		/// <param name="guard">The guard condition to be tested in order to follow the transition.</param>
		public Transition( SimpleState source, SimpleState target, Func<TMessage, Boolean> guard = null )
		{
			this.target = target;
			this.guard = guard;

			Completion.Path( source, target, ref onExit, ref onBeginEnter );

			( source.transitions ?? ( source.transitions = new HashSet<ITransition>() ) ).Add( this );
		}

		/// <summary>
		/// Creates an internal transition.
		/// </summary>
		/// <param name="state">The state to create the internal transition for.</param>
		/// <param name="guard">The guard condition to be tested in order to call the effect action.</param>
		/// <remarks>Internal transitions perform an action in response to an event, but do not leave the state therefore no entry or exit actions are performed.</remarks>
		public Transition( SimpleState state, Func<TMessage, Boolean> guard = null )
		{
			this.guard = guard;

			( state.transitions ?? ( state.transitions = new HashSet<ITransition>() ) ).Add( this );
		}

		Boolean ITransition.Guard( Object message )
		{
			if( guard == null )
				return true;

			var typed = message as TMessage;

			return typed != null && guard( typed );
		}

		void ITransition.Traverse( IState context, Object message )
		{
			if( onExit != null )
				onExit( context );

			OnEffect( message );

			if( onBeginEnter != null )
				onBeginEnter( context );

			if( this.target != null )
				this.target.OnEndEnter( context, false );
		}

		/// <summary>
		/// Invokes the transition effect action.
		/// </summary>
		/// <param name="message">The message that caused the transition.</param>
		/// <remarks>Override this method to create custom transition behaviour.</remarks>
		protected virtual void OnEffect( Object message )
		{
			if( Effect != null )
				Effect( message as TMessage );
		}
	}
}