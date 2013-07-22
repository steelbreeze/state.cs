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
		internal HashSet<Vertex> vertices = new HashSet<Vertex>();

		/// <summary>
		/// The Region's parent State
		/// </summary>
		public CompositeState Parent { get; private set; }

		/// <summary>
		/// The name of Region
		/// </summary>
		public String Name { get; private set; }

		/// <summary>
		/// Creates a Region.
		/// </summary>
		/// <param name="name">The name of the Region.</param>
		/// <param name="parent">The parent CompositeState.</param>
		public Region( String name, CompositeState parent = null )
		{
			Trace.Assert( name != null, "Region name must be provided" );

			this.Name = name;

			if( ( this.Parent = parent ) != null )
				( parent.regions ?? ( parent.regions = new HashSet<Region>() ) ).Add( this );
		}

		/// <summary>
		/// Tests to see if a region is complete
		/// </summary>
		/// <param name="state">The state machine state</param>
		/// <returns>True if the region is complete</returns>
		/// <remarks>
		/// A Region is deemed to be complete when it's current state is a FinalState.
		/// </remarks>
		public Boolean IsComplete( IState state )
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
			OnEnter( state );

			var initial = this.vertices.OfType<PseudoState>().SingleOrDefault( pseudoState => pseudoState.Kind.IsInitial ); // NOTE: linq is deferred so this will only evaluate if the logic below requires it

			var vertex = deepHistory || initial.Kind.IsHistory ? state.GetCurrent( this ) as Vertex ?? initial : initial;

			vertex.Initialise( state, deepHistory || ( initial.Kind == PseudoStateKind.DeepHistory ) );
		}

		override internal void OnExit( IState state )
		{
			if( state.GetCurrent( this ) != null )
				state.GetCurrent( this ).OnExit( state );

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
		public Boolean Process( IState state, Object message )
		{
			return state.GetActive( this ) && state.GetCurrent( this ).Process( state, message );
		}

		/// <summary>
		/// Displays the fully qualified name of the Region or Vertex
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Parent == null ? Name : Parent + "." + Name;
		}
	}
}