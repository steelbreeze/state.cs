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
using System.Linq;

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// A state (invariant condition) within a state machine model.
	/// </summary>
	public class SimpleState<TState> : Element<TState> where TState : IState<TState>
	{
		private ICollection<Transition<TState>> completions;
		private ICollection<ITransition<TState>> transitions;

		/// <summary>
		/// Optional action(s) that can be called when the state is entered.
		/// </summary>
		public event Action<TState> Entry;

		/// <summary>
		/// Optional action(s) that can be calle when the state is exited.
		/// </summary>
		public event Action<TState> Exit;

		/// <summary>
		/// Creates a state within an owning (parent) region.
		/// </summary>
		/// <param name="name">The name of the state.</param>
		/// <param name="owner">The owning (parent) region.</param>
		public SimpleState( String name, Region<TState> owner ) : base( name, owner ) { }

		/// <summary>
		/// Creates a state within an owning (parent) composite state.
		/// </summary>
		/// <param name="name">The name of the state.</param>
		/// <param name="owner">The owning (parent) composite state.</param>
		public SimpleState( String name, CompositeState<TState> owner ) : base( name, owner ) { }

		internal void Add( Transition<TState> completion )
		{
			if( completions == null )
				completions = new HashSet<Transition<TState>>();

			completions.Add( completion );
		}

		internal void Add( ITransition<TState> transition )
		{
			if( transitions == null )
				transitions = new HashSet<ITransition<TState>>();

			transitions.Add( transition );
		}

		internal virtual Boolean IsComplete( TState state )
		{
			return true;
		}

		/// <summary>
		/// Invokes the state exit action.
		/// </summary>
		/// <remarks>Override this method to create custom exit behaviour.</remarks>
		protected virtual void OnExit( TState state )
		{
			if( Exit != null )
				Exit( state );
		}

		/// <summary>
		/// Invokes the state entry action.
		/// </summary>
		/// <remarks>Override this method to create custom entry behaviour.</remarks>
		protected virtual void OnEntry( TState state )
		{
			if( Entry != null )
				Entry( state );
		}

		internal override void EndExit( TState state )
		{
			this.OnExit( state );
			base.EndExit( state );
		}

		internal override void BeginEntry( TState state )
		{
			base.BeginEntry( state );

			if( this.Owner != null )
				state.SetCurrent( this.Owner, this );

			this.OnEntry( state );
		}

		internal override void EndEntry( TState state, Boolean deepHistory )
		{
			if( completions == null )
				return;

			if( !IsComplete( state ) )
				return;

			var completion = completions.SingleOrDefault( t => t.guard( state ) );

			if( completion != null )
				completion.Traverse( state, deepHistory );
		}

		internal virtual Boolean Process( TState state, Object message )
		{
			if( this.transitions == null )
				return false;

			var transition = this.transitions.SingleOrDefault( t => t.Guard( state, message ) );

			if( transition == null )
				return false;

			transition.Traverse( state, message );

			return true;
		}
	}
}