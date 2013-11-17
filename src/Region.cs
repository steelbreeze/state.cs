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

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// A region within a state machine model.
	/// </summary>
	/// <remarks>
	/// A region is a container for states and pseudo states.
	/// </remarks>
	public class Region : Element
	{
		internal PseudoState Initial { private get; set; }

		/// <summary>
		/// Creates a new region within a state machine.
		/// </summary>
		/// <param name="name">The name of the region.</param>
		/// <param name="owner">The parent state machine.</param>
		/// <remarks>
		/// A state machine is a container of states and pseudo states within a state machine model; it can be used as a root state machine.
		/// </remarks>
		public Region( String name, StateMachine owner )
			: base( name, owner )
		{
			if( owner != null )
				owner.regions.Add( this );
		}

		/// <summary>
		/// Creates a new region within an orthogonal state.
		/// </summary>
		/// <param name="name">The name of the region.</param>
		/// <param name="owner">The optional parent orthogonal state.</param>
		/// <remarks>
		/// A region is a container of states and pseudo states within a state machine model; it can be used as a root state machine.
		/// </remarks>
		public Region( String name, OrthogonalState owner = null )
			: base( name, owner )
		{
			if( owner != null )
				owner.regions.Add( this );
		}

		/// <summary>
		/// Determines if a region is completed.
		/// </summary>
		/// <param name="context">The state machine state to test completeness for.</param>
		/// <returns>A boolean value indicating that the region is completed.</returns>
		/// <remarks>A region is deemed to be completed when its current child state is a final state.</remarks>
		public Boolean IsComplete( IState context )
		{
			return context.IsTerminated || context.GetCurrent( this ) is FinalState;
		}

		/// <summary>
		/// Initialises the state machine state context with its initial state.
		/// </summary>
		/// <param name="context">The state machine state context to initialise.</param>
		public void Initialise( IState context )
		{
			BeginEnter( context );
			EndEnter( context, false );
		}


		internal override void BeginExit( IState context )
		{
			var current = context.GetCurrent( this );

			if( current != null )
			{
				current.BeginExit( context );
				current.EndExit( context );
			}
		}

		internal override void EndEnter( IState context, Boolean deepHistory )
		{
			var current = deepHistory || this.Initial.Kind.IsHistory() ? context.GetCurrent( this ) as Element ?? this.Initial : this.Initial;

			current.BeginEnter( context );
			current.EndEnter( context, deepHistory || this.Initial.Kind == PseudoStateKind.DeepHistory );
		}

		/// <summary>
		/// Attempts to process a message against a region.
		/// </summary>
		/// <param name="context">The state machine state.</param>
		/// <param name="message">The message to evaluate.</param>
		/// <returns>A boolean indicating if the message caused a state change.</returns>
		public Boolean Process( IState context, Object message )
		{
			if( context.IsTerminated )
				return false;

			return context.GetActive( this ) && context.GetCurrent( this ).Process( context, message );
		}
	}
}
