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
	/// A Region is an orthogonal part of either a CompositeState or a StateMachine. It contains states and transitions.
	/// </summary>
	public class Region : StateMachineElement
	{
		private readonly CompositeState owner;
		internal readonly HashSet<Vertex> vertices = new HashSet<Vertex>();
		internal PseudoState initial;

		/// <summary>
		/// The parent element of this element
		/// </summary>
		public override StateMachineElement Owner { get { return this.owner; } }
	
		/// <summary>
		/// Creates a Region.
		/// </summary>
		/// <param name="name">The name of the Region.</param>
		/// <param name="owner">The parent CompositeState.</param>
		public Region( String name, CompositeState owner = null )
			: base( name )
		{
			if( ( this.owner = owner ) != null )
				this.owner.regions.Add( this );
		}

		/// <summary>
		/// Tests to see if a region is complete
		/// </summary>
		/// <param name="state">The state machine state</param>
		/// <returns>True if the region is complete</returns>
		/// <remarks>
		/// A Region is deemed to be complete when it's current state is a FinalState.
		/// </remarks>
		public override Boolean IsComplete( IState state )
		{
			return state.GetCurrent( this ) is FinalState;
		}

		/// <summary>
		/// Initialises a node to its initial state.
		/// <param name="state">The state machine state to initialise.</param>
		/// </summary>
		public void Initialise( IState state )
		{
			Initialise( state, false );
		}

		internal void Initialise( IState state, Boolean deepHistory )
		{
			this.OnEnter( state );

			var current = ( deepHistory || initial.Kind.IsHistory ) ? ( state.GetCurrent( this ) ?? initial ) : initial;

			current.OnEnter( state );
			current.Complete( state, deepHistory || initial.Kind == PseudoStateKind.DeepHistory );
		}

		override internal void OnExit( IState state )
		{
			var current = state.GetCurrent( this );

			if( current != null )
				current.OnExit( state );

			state.SetActive( this, false );

			base.OnExit( state );
		}

		internal override void OnEnter( IState state )
		{
			if( state.GetActive( this ) )
				OnExit( state );

			base.OnEnter( state );

			state.SetActive( this, true );
		}

		/// <summary>
		/// Attempts to process a message to facilitate state transitions
		/// </summary>
		/// <param name="state">The state machine state to pass the message to.</param>
		/// <param name="message">The message to process.</param>
		/// <returns>A Boolean indicating if the message was processed.</returns>
		public override Boolean Process( IState state, Object message )
		{
			return state.GetActive( this ) && state.GetCurrent( this ).Process( state, message );
		}
	}
}