// Copyright © 2014 Steelbreeze Limited.
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
	/// An event-based transition from a state.
	/// </summary>
	/// <typeparam name="TState">The type of the state machine state under state management through this state machine model</typeparam>
	/// <typeparam name="TMessage">The type of the message that can trigger the transition.</typeparam>
	public class Transition<TState, TMessage> : ITransition<TState> where TState : IState<TState> where TMessage : class
	{
		private readonly Path<TState> path;
		private readonly Func<TMessage, Boolean> guard;

		/// <summary>
		/// The action(s) to perform while traversing the transition.
		/// </summary>
		public event Action<TState, TMessage> Effect;

		/// <summary>
		/// Creates a transition from a state to a pseudo state.
		/// </summary>
		/// <param name="source">The source state.</param>
		/// <param name="target">The target pseudo state.</param>
		/// <param name="guard">The guard condition to be tested in order to follow the transition.</param>
		/// <remarks>This type of transition initiates a compound transition.</remarks>
		public Transition( SimpleState<TState> source, PseudoState<TState> target, Func<TMessage, Boolean> guard = null )
		{
			this.guard = guard;
			this.path = new Path<TState>( source, target );

			source.Add( this );
		}

		/// <summary>
		/// Creates a transition between states.
		/// </summary>
		/// <param name="source">The source state.</param>
		/// <param name="target">The target state.</param>
		/// <param name="guard">The guard condition to be tested in order to follow the transition.</param>
		public Transition( SimpleState<TState> source, SimpleState<TState> target, Func<TMessage, Boolean> guard = null )
		{
			this.guard = guard;
			this.path = new Path<TState>( source, target );

			source.Add( this );
		}

		/// <summary>
		/// Creates an internal transition.
		/// </summary>
		/// <param name="source">The state to create the internal transition for.</param>
		/// <param name="guard">The guard condition to be tested in order to call the effect action.</param>
		/// <remarks>Internal transitions perform an action in response to an event, but do not leave the state therefore no entry or exit actions are performed.</remarks>
		public Transition( SimpleState<TState> source, Func<TMessage, Boolean> guard = null )
		{
			this.guard = guard;

			source.Add( this );
		}

		Boolean ITransition<TState>.Guard( Object message )
		{
			var typed = message as TMessage; // NOTE: do not attempt to remove case as this performs the message type check

			if( typed == null )
				return false;

			return guard == null || guard( typed );
		}

		void ITransition<TState>.Traverse( TState state, Object message )
		{
			if( path != null )
				path.exit( state );

			OnEffect( state, message );

			if( path != null )
			{
				path.beginEntry( state );
				path.endEntry( state, false );
			}
		}

		/// <summary>
		/// Invokes the transition effect action.
		/// </summary>
		/// <param name="state">The state machine instnace</param>
		/// <param name="message">The message that caused the transition.</param>
		/// <remarks>Override this method to create custom transition behaviour.</remarks>
		protected virtual void OnEffect( TState state, Object message )
		{
			if( Effect != null )
				Effect( state, message as TMessage ); // NOTE: cast is ok as this won't be called unless the guard passed
		}
	}

	/// <summary>
	/// A continuation transition between states or pseudo states within a state machine.
	/// </summary>
	/// <remarks>
	/// Continuation transitions are tested for after sucessful entry to pseudo states or completed states.
	/// </remarks>
	public partial class Transition<TState> where TState : IState<TState>
	{
		internal static Func<Boolean> True = () => true;
		internal static Func<Boolean> False = () => false;
		internal readonly Func<Boolean> guard;
		private readonly Path<TState> path;

		/// <summary>
		/// The action(s) to perform while traversing the transition.
		/// </summary>
		public event Action<TState> Effect;

		/// <summary>
		/// Creates a continuation transition between pseudo states.
		/// </summary>
		/// <param name="source">The source pseudo state.</param>
		/// <param name="target">The target pseudo state.</param>
		/// <param name="guard">The guard condition to be tested in order to follow the transition.</param>
		/// <remarks>For initial pseudo states, this type of tranision initiates a compound transition, for others, it is a particiapnt in a compound transition.</remarks>
		public Transition( PseudoState<TState> source, PseudoState<TState> target, Func<Boolean> guard = null )
		{
			this.guard = guard ?? True;
			this.path = new Path<TState>( source, target );

			source.Add( this );
		}

		/// <summary>
		/// Creates a continuation transition from a pseudo state to a state.
		/// </summary>
		/// <param name="source">The source pseudo state.</param>
		/// <param name="target">The target state.</param>
		/// <param name="guard">The guard condition to be tested in order to follow the transition.</param>
		/// <remarks>This type of transition completes a compound transition.</remarks>
		public Transition( PseudoState<TState> source, SimpleState<TState> target, Func<Boolean> guard = null )
		{
			this.guard = guard ?? True;
			this.path = new Path<TState>( source, target );

			source.Add( this );
		}

		/// <summary>
		/// Creates a continuation transition from a state to a pseudo state.
		/// </summary>
		/// <param name="source">The source state.</param>
		/// <param name="target">The target pseudo state.</param>
		/// <param name="guard">The guard condition to be tested in order to follow the transition.</param>
		/// <remarks>Continuation transitions are tested for after a state has been entered if the state is deemed to be completed.</remarks>
		/// <remarks>This type of transition initiates a compound transition.</remarks>
		public Transition( SimpleState<TState> source, PseudoState<TState> target, Func<Boolean> guard = null )
		{
			this.guard = guard ?? True;
			this.path = new Path<TState>( source, target );

			source.Add( this );
		}

		/// <summary>
		/// Creates a continuation transition between states.
		/// </summary>
		/// <param name="source">The source state.</param>
		/// <param name="target">The target state.</param>
		/// <param name="guard">The guard condition to be tested in order to follow the transition.</param>
		/// <remarks>Continuation transitions are tested for after a state has been entered if the state is deemed to be completed.</remarks>
		public Transition( SimpleState<TState> source, SimpleState<TState> target, Func<Boolean> guard = null )
		{
			this.guard = guard ?? True;
			this.path = new Path<TState>( source, target );

			source.Add( this );
		}

		internal void Traverse( TState state, Boolean deepHistory )
		{
			path.exit( state );

			OnEffect( state );

			path.beginEntry( state );
			path.endEntry( state, deepHistory );
		}

		/// <summary>
		/// Invokes the transition effect action.
		/// </summary>
		/// <remarks>Override this method to create custom transition behaviour.</remarks>
		protected virtual void OnEffect( TState state )
		{
			if( Effect != null )
				Effect( state );
		}
	}
}