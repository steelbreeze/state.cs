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
	/// An event-based Transition between Vertices.
	/// </summary>
	/// <typeparam name="TMessage">The type of the message that may cause the transition to be traversed.</typeparam>
	public class Transition<TMessage> : TransitionBase, ITransition where TMessage : class
	{
		// the guard condition
		private readonly Func<TMessage, Boolean> guard;

		/// <summary>
		/// The optional action that is called while traversing the transition.
		/// </summary>
		public event Action<TMessage> Effect;

		/// <summary>
		/// Creates an event-based Transition.
		/// </summary>
		/// <param name="source">The source Vertex of the Transition.</param>
		/// <param name="target">The target Vertex of the Transition.</param>
		/// <param name="guard">An optional guard condition to restrict traversal of the transition.</param>
		public Transition( SimpleState source, Vertex target, Func<TMessage, Boolean> guard = null )
			: base( source, target )
		{
			Trace.Assert( source != null, "Source vertex for transition must be specified." );

			this.guard = guard ?? ( message => true );

			( source.transitions ?? ( source.transitions = new HashSet<ITransition>() ) ).Add( this );
		}

		Boolean ITransition.EvaluateGuard( Object message )
		{
			return Guard( message );
		}

		/// <summary>
		/// Logic required to evaluate the completion guard condition
		/// </summary>
		/// <param name="message">The message to test the guard condition against</param>
		/// <returns>True if the guard evaluates true</returns>
		protected virtual Boolean Guard( Object message )
		{
			var typed = message as TMessage;

			if( typed == null )
				return false;
			else
				return guard( typed );
		}

		void ITransition.Traverse( IState state, Object message )
		{
			if( onExit != null )
				onExit( state );

			OnEffect( message );
	
			if( onBeginEnter != null )
				onBeginEnter( state );
	
			if( onEndEnter != null )
				onEndEnter( state, false );
		}

		/// <summary>
		/// The transitions behaviour
		/// </summary>
		/// <param name="message">The message that caused the transition.</param>
		/// <remarks>
		/// Override this method to implement more complex event-based transition behaviour
		/// </remarks>
		protected virtual void OnEffect( Object message )
		{
			if( Effect != null )
				Effect( message as TMessage );
		}
	}
}