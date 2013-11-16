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
	/// A state (invariant condition) within a state machine model.
	/// </summary>
	public class SimpleState : Element
	{
		private ICollection<Transition> completions;
		private ICollection<ITransition> transitions;

		/// <summary>
		/// Optional action(s) that can be called when the state is entered.
		/// </summary>
		public event Action Entry;

		/// <summary>
		/// Optional action(s) that can be calle when the state is exited.
		/// </summary>
		public event Action Exit;

		/// <summary>
		/// Creates a state within an owning (parent) region.
		/// </summary>
		/// <param name="name">The name of the state.</param>
		/// <param name="owner">The owning (parent) region.</param>
		public SimpleState( String name, Region owner ) : base( name, owner ) { }

		/// <summary>
		/// Creates a state within an owning (parent) composite state.
		/// </summary>
		/// <param name="name">The name of the state.</param>
		/// <param name="owner">The owning (parent) composite state.</param>
		public SimpleState( String name, CompositeState owner ) : base( name, owner ) { }

		internal void Add( Transition completion )
		{
			if( completions == null )
				completions = new HashSet<Transition>();
			
			completions.Add( completion );
		}

		internal void Add( ITransition transition )
		{
			if( transitions == null )
				transitions = new HashSet<ITransition>();
			
			transitions.Add( transition );
		}

		/// <summary>
		/// Tests the state for completeness.
		/// </summary>
		/// <param name="context">The state machine state to test.</param>
		public virtual Boolean IsComplete( IState context )
		{
			return true;
		}

		/// <summary>
		/// Invokes the state exit action.
		/// </summary>
		/// <remarks>Override this method to create custom exit behaviour.</remarks>
		protected virtual void OnExit()
		{
			if( Exit != null )
				Exit();
		}

		/// <summary>
		/// Invokes the state entry action.
		/// </summary>
		/// <remarks>Override this method to create custom entry behaviour.</remarks>
		protected virtual void OnEnter()
		{
			if( Entry != null )
				Entry();
		}

		internal override void EndExit( IState context )
		{
			this.OnExit();
			base.EndExit( context );
		}

		internal override void BeginEnter( IState context )
		{
			base.BeginEnter( context );

			if( this.Owner != null )
				context.SetCurrent( this.Owner, this );

			this.OnEnter();
		}

		internal override void EndEnter( IState context, Boolean deepHistory )
		{
			if( completions == null )
				return;

			if( !IsComplete( context ) )
				return;

			var completion = completions.SingleOrDefault( t => t.Guard() );

			if( completion != null )
				completion.Traverse( context, deepHistory );
		}


		/// <summary>
		/// Attempts to process a message against a state.
		/// </summary>
		/// <param name="context">The state machine state.</param>
		/// <param name="message">The message to evaluate.</param>
		/// <returns>A boolean indicating if the message caused a state change.</returns>
		public virtual Boolean Process( IState context, Object message )
		{
			if( context.IsTerminated )
				return false;

			if( this.transitions == null )
				return false;

			var transition = this.transitions.SingleOrDefault( t => t.Guard( message ) );

			if( transition != null )
				transition.Traverse( context, message );

			return transition != null;
		}
	}
}
