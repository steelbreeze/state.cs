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
	/// A Composite State that contains child vertices.
	/// </summary>
	public class CompositeState : SimpleState
	{
		internal PseudoState initial;

		/// <summary>
		/// Creates a composite state.
		/// </summary>
		/// <param name="name">The name of the composite state</param>
		/// <param name="parent">The parent region of the composite state.</param>
		public CompositeState( String name, Region parent ) : base( name, parent ) { }

		/// <summary>
		/// Creates a composite state
		/// </summary>
		/// <param name="name">The name of the composite state</param>
		/// <param name="parent">The parent composite state of the composite state</param>
		public CompositeState( String name, CompositeState parent ) : base( name, parent ) { }

		/// <summary>
		/// Tests the orthogonal state for completeness
		/// </summary>
		/// <param name="state">The state machine state to test completeness against.</param>
		/// <returns>True if the state machine state is complete for this composite state.</returns>
		/// <remarks>
		/// A composite state is deemed to be complete if its currently active child state is a final state.
		/// </remarks>
		public override bool IsComplete( IState state )
		{
			return state.GetCurrent( this ) is FinalState;
		}

		internal override void OnExit( IState state )
		{
			var current = state.GetCurrent( this );

			if( current != null )
				current.OnExit( state );

			base.OnExit( state );
		}

		internal override void OnEndEnter( IState state, bool deepHistory )
		{
			var current = ( deepHistory || initial.Kind.IsHistory ) ? ( state.GetCurrent( this ) ?? initial ) : initial;

			current.OnBeginEnter( state );
			current.OnEndEnter( state, deepHistory || initial.Kind == PseudoStateKind.DeepHistory );

			base.OnEndEnter( state, deepHistory );
		}

		/// <summary>
		/// Attempts to process a message against a state machine state.
		/// </summary>
		/// <param name="state">The state machine state to process the message against.</param>
		/// <param name="message">The message.</param>
		/// <returns>True if the message caused a state transition.</returns>
		/// <remarks>
		/// Note that a state transition may leave the state machine state unchanged (both internal transitions and self-transitions). 
		/// </remarks>
		public override Boolean Process( IState state, Object message )
		{
			if( state.IsTerminated )
				return false;

			return base.Process( state, message ) || state.GetCurrent( this ).Process( state, message );
		}
	}
}
