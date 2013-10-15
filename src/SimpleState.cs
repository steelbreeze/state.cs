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
	/// A state (invariant condition) within a state machine model.
	/// </summary>
	public class SimpleState : IVertex
	{
		IElement IElement.Owner { get { return owner; } }

		private readonly IRegion owner;

		internal ICollection<Completion> completions { get; set; }
		internal ICollection<ITransition> transitions { get; set; }
		internal virtual Boolean IsFinalState { get { return false; } }

		/// <summary>
		/// The name of the state.
		/// </summary>
		public String Name { get; private set; }
	
		/// <summary>
		/// Optional action(s) that can be called when the state is entered.
		/// </summary>
		public Action Entry { get; set; }

		/// <summary>
		/// Optional action(s) that can be calle when the state is exited.
		/// </summary>
		public Action Exit { get; set; }

		/// <summary>
		/// Creates a state within an owning (parent) region.
		/// </summary>
		/// <param name="name">The name of the state.</param>
		/// <param name="owner">The owning (parent) region.</param>
		public SimpleState( String name, Region owner )
		{
			this.Name = name;
			this.owner = owner;
		}

		/// <summary>
		/// Creates a state within an owning (parent) composite state.
		/// </summary>
		/// <param name="name">The name of the state.</param>
		/// <param name="owner">The owning (parent) composite state.</param>
		public SimpleState( String name, CompositeState owner )
		{
			this.Name = name;
			this.owner = owner;
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

		internal virtual void OnExit( IState context )
		{
			this.OnExit();

			Debug.WriteLine( this, "Leave" );

			context.SetActive( this, false );
		}

		void IElement.Exit( IState context )
		{
			OnExit( context );
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

		internal virtual void OnEnter( IState context )
		{
			IVertex vertex = this;

			if( context.GetActive( this ) )
				vertex.Exit( context );

			Debug.WriteLine( this, "Enter" );

			context.SetActive( this, true );

			if( this.owner != null )
				context.SetCurrent( owner, this );

			this.OnEnter();
		}

		void IElement.Enter( IState context )
		{
			OnEnter( context );
		}

		internal virtual void OnComplete( IState context, Boolean deepHistory )
		{
			if( completions != null )
			{
				if( IsComplete( context ) )
				{
					var completion = completions.SingleOrDefault( t => t.Guard() );

					if( completion != null )
						completion.Traverse( context, deepHistory );
				}
			}
		}

		void IVertex.Complete( IState context, Boolean deepHistory )
		{
			OnComplete( context, deepHistory );
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

			if( transition == null )
				return false;

			transition.Traverse( context, message );

			return true;
		}

		/// <summary>
		/// Returns the fully qualified name of the state.
		/// </summary>
		/// <returns>The fully qualified name of the state.</returns>
		public override String ToString()
		{
			return this.Ancestors().Select( ancestor => ancestor.Name ).Aggregate( ( right, left ) => left + "." + right );
		}
	}
}
