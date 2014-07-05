// The MIT License (MIT)
//
// Copyright (c) 2014 Steelbreeze Limited
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
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
	public class Transition<TState, TMessage> : TransitionBase<TState>, ITransition<TState> where TState : IState<TState> where TMessage : class
	{
		private readonly Func<TState, TMessage, Boolean> guard;

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
		public Transition( SimpleState<TState> source, PseudoState<TState> target, Func<TState, TMessage, Boolean> guard = null )
			: base (source, target )
		{
			this.guard = guard;

			source.Add( this );
		}

		/// <summary>
		/// Creates a transition between states.
		/// </summary>
		/// <param name="source">The source state.</param>
		/// <param name="target">The target state.</param>
		/// <param name="guard">The guard condition to be tested in order to follow the transition.</param>
		public Transition( SimpleState<TState> source, SimpleState<TState> target, Func<TState, TMessage, Boolean> guard = null )
			: base( source, target )
		{
			this.guard = guard;

			source.Add( this );
		}

		/// <summary>
		/// Creates an internal transition.
		/// </summary>
		/// <param name="source">The state to create the internal transition for.</param>
		/// <param name="guard">The guard condition to be tested in order to call the effect action.</param>
		/// <remarks>Internal transitions perform an action in response to an event, but do not leave the state therefore no entry or exit actions are performed.</remarks>
		public Transition( SimpleState<TState> source, Func<TState, TMessage, Boolean> guard = null )
			: base( source, null )
		{
			this.guard = guard;

			source.Add( this );
		}

		Boolean ITransition<TState>.Guard( TState state, Object message )
		{
			var typed = message as TMessage; // NOTE: do not attempt to remove case as this performs the message type check

			if( typed == null )
				return false;

			return guard == null || guard( state, typed );
		}

		void ITransition<TState>.Traverse( TState state, Object message )
		{
			if( this.Exit != null )
				this.Exit( state );

			OnEffect( state, message );

			if( this.BeginEntry != null )
			{
				this.BeginEntry( state );
				this.EndEntry( state, false );
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
	public partial class Transition<TState> : TransitionBase<TState> where TState : IState<TState>
	{
		internal static Func<TState, Boolean> True = state => true;
		internal static Func<TState, Boolean> False = state => false;
		internal readonly Func<TState, Boolean> guard;
		internal virtual Boolean isElse { get { return false; } }

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
		public Transition( PseudoState<TState> source, PseudoState<TState> target, Func<TState, Boolean> guard = null )
			: base( source, target )

		{
			this.guard = guard ?? True;

			source.Add( this );
		}

		/// <summary>
		/// Creates a continuation transition from a pseudo state to a state.
		/// </summary>
		/// <param name="source">The source pseudo state.</param>
		/// <param name="target">The target state.</param>
		/// <param name="guard">The guard condition to be tested in order to follow the transition.</param>
		/// <remarks>This type of transition completes a compound transition.</remarks>
		public Transition( PseudoState<TState> source, SimpleState<TState> target, Func<TState, Boolean> guard = null )
			: base( source, target )
		{
			this.guard = guard ?? True;

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
		public Transition( SimpleState<TState> source, PseudoState<TState> target, Func<TState, Boolean> guard = null )
			: base( source, target )
		{
			this.guard = guard ?? True;

			source.Add( this );
		}

		/// <summary>
		/// Creates a continuation transition between states.
		/// </summary>
		/// <param name="source">The source state.</param>
		/// <param name="target">The target state.</param>
		/// <param name="guard">The guard condition to be tested in order to follow the transition.</param>
		/// <remarks>Continuation transitions are tested for after a state has been entered if the state is deemed to be completed.</remarks>
		public Transition( SimpleState<TState> source, SimpleState<TState> target, Func<TState, Boolean> guard = null )
			: base( source, target )
		{
			this.guard = guard ?? True;

			source.Add( this );
		}

		internal void Traverse( TState state, Boolean deepHistory )
		{
			this.Exit( state );

			OnEffect( state );

			this.BeginEntry( state );
			this.EndEntry( state, deepHistory );
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