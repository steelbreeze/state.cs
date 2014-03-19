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

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// A composite state is a state that contains states and pseudo states.
	/// </summary>
	public class CompositeState<TState> : SimpleState<TState> where TState : IState<TState>
	{
		// TODO: add a collection of elements (needed for serialisation)
		internal PseudoState<TState> initial;

		/// <summary>
		/// Creates a composite state within an owning (parent) region.
		/// </summary>
		/// <param name="name">The name of the composite state.</param>
		/// <param name="owner">The owning (parent) region.</param>
		/// <remarks>
		/// A composite state is a container of states and pseudo states within a state machine model; it can be used as a root state machine.
		/// </remarks>
		public CompositeState( String name, Region<TState> owner ) : base( name, owner ) { }

		/// <summary>
		/// Creates a composite state within an owning (parent) composite state.
		/// </summary>
		/// <param name="name">The name of the composite state.</param>
		/// <param name="owner">The owning (parent) composite state.</param>
		/// <remarks>
		/// A composite state is a container of states and pseudo states within a state machine model; it can be used as a root state machine.
		/// </remarks>
		public CompositeState( String name, CompositeState<TState> owner ) : base( name, owner ) { }

		/// <summary>
		/// Creates a new child PseudoState within the CompositeState
		/// </summary>
		/// <param name="name">The name of the PseudoState</param>
		/// <param name="kind">The kind of the PseudoState</param>
		/// <returns>The new child PseudoState</returns>
		public PseudoState<TState> CreatePseudoState( String name, PseudoStateKind kind )
		{
			return new PseudoState<TState>( name, kind, this );
		}

		/// <summary>
		/// Creates a new child SimpleState within the CompositeState
		/// </summary>
		/// <param name="name">The name of the SimpleState</param>
		/// <returns>The new child SimpleState</returns>
		public SimpleState<TState> CreateSimpleState( String name )
		{
			return new SimpleState<TState>( name, this );
		}

		/// <summary>
		/// Creates a new child CompositeState within the CompositeState
		/// </summary>
		/// <param name="name">The name of the CompositeState</param>
		/// <returns>The new child CompositeState</returns>
		public CompositeState<TState> CreateCompositeState( String name )
		{
			return new CompositeState<TState>( name, this );
		}

		/// <summary>
		/// Creates a new child OrthogonalState within the CompositeState
		/// </summary>
		/// <param name="name">The name of the OrthogonalState</param>
		/// <returns>The new child Orthogonaltate</returns>
		public OrthogonalState<TState> CreateOrthogonalState( String name )
		{
			return new OrthogonalState<TState>( name, this );
		}

		/// <summary>
		/// Creates a new child FinalState within the CompositeState
		/// </summary>
		/// <param name="name">The name of the FinalState</param>
		/// <returns>The new child FinalState</returns>
		public FinalState<TState> CreateFinalState( String name )
		{
			return new FinalState<TState>( name, this );
		}

		internal override bool IsComplete( TState state )
		{
			var current = state.GetCurrent( this );

			return state.IsTerminated || current == null || current is FinalState<TState> || state.GetActive( current ) == false;
		}

		internal override void BeginExit( TState state )
		{
			var current = state.GetCurrent( this );

			if( current != null )
			{
				current.BeginExit( state );
				current.EndExit( state );
			}
		}

		internal override void EndEntry( TState state, bool deepHistory )
		{
			Element<TState> current = null;

			if( deepHistory || this.initial.Kind.IsHistory() )
				current = state.GetCurrent( this );

			if( current == null )
				current = initial;

			current.BeginEntry( state );
			current.EndEntry( state, deepHistory || this.initial.Kind == PseudoStateKind.DeepHistory );

			base.EndEntry( state, deepHistory );
		}

		internal override Boolean Process( TState state, Object message )
		{
			if( state.IsTerminated )
				return false;

			return base.Process( state, message ) || state.GetCurrent( this ).Process( state, message );
		}
	}
}
