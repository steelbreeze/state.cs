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
	/// An event-based transition from a state.
	/// </summary>
	/// <typeparam name="TMessage">The type of the message that can trigger the transition.</typeparam>
	public class Transition<TMessage> : ITransition where TMessage : class
	{
		private readonly Path path;
		private readonly Func<TMessage, Boolean> guard;

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
			this.guard = guard;
			this.path = new Path( source, target );

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
			this.guard = guard;
			this.path = new Path( source, target );

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
			if( path != null )
				path.Exit( context );

			OnEffect( message );

			if( path != null )
				path.Enter( context, false );
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

	/// <summary>
	/// A continuation transition between states or pseudo states within a state machine.
	/// </summary>
	/// <remarks>
	/// Continuation transitions are tested for after sucessful entry to pseudo states or completed states.
	/// </remarks>
	public partial class Transition
	{
		private readonly Path path;
		private readonly Func<Boolean> guard;

		internal virtual Boolean IsElse { get { return false; } }

		/// <summary>
		/// The action(s) to perform while traversing the transition.
		/// </summary>
		public event Action Effect;

		/// <summary>
		/// Creates a continuation transition between pseudo states.
		/// </summary>
		/// <param name="source">The source pseudo state.</param>
		/// <param name="target">The target pseudo state.</param>
		/// <param name="guard">The guard condition to be tested in order to follow the transition.</param>
		/// <remarks>For initial pseudo states, this type of tranision initiates a compound transition, for others, it is a particiapnt in a compound transition.</remarks>
		public Transition( PseudoState source, PseudoState target, Func<Boolean> guard = null )
		{
			this.guard = guard;
			this.path = new Path( source, target );

			if( source.Kind.IsInitial() )
				Trace.Assert( source.completions == null, "initial pseudo states can have at most one outbound completion transition" );

			( source.completions ?? ( source.completions = new HashSet<Transition>() ) ).Add( this );
		}

		/// <summary>
		/// Creates a continuation transition from a pseudo state to a state.
		/// </summary>
		/// <param name="source">The source pseudo state.</param>
		/// <param name="target">The target state.</param>
		/// <param name="guard">The guard condition to be tested in order to follow the transition.</param>
		/// <remarks>This type of transition completes a compound transition.</remarks>
		public Transition( PseudoState source, SimpleState target, Func<Boolean> guard = null )
		{
			this.guard = guard;
			this.path = new Path( source, target );

			if( source.Kind.IsInitial() )
				Trace.Assert( source.completions == null, "initial pseudo states can have at most one outbound completion transition" );

			( source.completions ?? ( source.completions = new HashSet<Transition>() ) ).Add( this );
		}

		/// <summary>
		/// Creates a continuation transition from a state to a pseudo state.
		/// </summary>
		/// <param name="source">The source state.</param>
		/// <param name="target">The target pseudo state.</param>
		/// <param name="guard">The guard condition to be tested in order to follow the transition.</param>
		/// <remarks>Continuation transitions are tested for after a state has been entered if the state is deemed to be completed.</remarks>
		/// <remarks>This type of transition initiates a compound transition.</remarks>
		public Transition( SimpleState source, PseudoState target, Func<Boolean> guard = null )
		{
			this.guard = guard;
			this.path = new Path( source, target );

			( source.completions ?? ( source.completions = new HashSet<Transition>() ) ).Add( this );
		}

		/// <summary>
		/// Creates a continuation transition between states.
		/// </summary>
		/// <param name="source">The source state.</param>
		/// <param name="target">The target state.</param>
		/// <param name="guard">The guard condition to be tested in order to follow the transition.</param>
		/// <remarks>Continuation transitions are tested for after a state has been entered if the state is deemed to be completed.</remarks>
		public Transition( SimpleState source, SimpleState target, Func<Boolean> guard = null )
		{
			this.guard = guard;
			this.path = new Path( source, target );

			( source.completions ?? ( source.completions = new HashSet<Transition>() ) ).Add( this );
		}

		internal Boolean Guard()
		{
			return guard == null || guard();
		}

		internal void Traverse( IState context, Boolean deepHistory )
		{
			path.Exit( context );

			OnEffect();

			path.Enter( context, deepHistory );
		}

		/// <summary>
		/// Invokes the transition effect action.
		/// </summary>
		/// <remarks>Override this method to create custom transition behaviour.</remarks>
		protected virtual void OnEffect()
		{
			if( Effect != null )
				Effect();
		}
	}
}
