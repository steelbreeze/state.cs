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
	/// A state with a state machine model that can contain child regions.
	/// </summary>
	public class CompositeState : SimpleState
	{
		/// <summary>
		/// Returns the default Region within a CompositeState .
		/// </summary>
		/// <param name="state">The CompositeState to find the default Region for.</param>
		/// <returns>The default Region.</returns>
		public static implicit operator Region( CompositeState state ) { return state.regions.SingleOrDefault( r => r.Name.Equals( "default" ) ) ?? new Region( "default", state ); }

		/// <summary>
		/// The child regions of the composite state
		/// </summary>
		internal readonly HashSet<Region> regions = new HashSet<Region>();

		/// <summary>
		/// Creates a Compsite State.
		/// </summary>
		/// <param name="name">The name of the composite state.</param>
		/// <param name="owner">The optional parent region.</param>
		/// <remarks>
		/// A composite state may be used as the root of a state machine model; in this case, the parent region is not provided.
		/// </remarks>
		public CompositeState( String name, Region owner = null ) : base( name, owner ) { }

		/// <summary>
		/// Tests the composite state for completeness
		/// </summary>
		/// <param name="state">The state machine state to test completeness against.</param>
		/// <returns>True if the state machine state is complete for this composite state.</returns>
		/// <remarks>
		/// A composite state is deemed to be complete if all its child regions are complete.
		/// </remarks>
		public override Boolean IsComplete( IState state )
		{
			return regions.All( region => region.IsComplete( state ) );
		}

		/// <summary>
		/// Exits the composite state
		/// </summary>
		/// <param name="state">The state model state to operate on.</param>
		internal override void OnExit( IState state )
		{
			foreach( var region in regions )
				if( state.GetActive( region ) )
					region.OnExit( state );

			base.OnExit( state );
		}

		/// <summary>
		/// Completes the entry of the composite state.
		/// </summary>
		/// <param name="state">The state model state to operate on.</param>
		/// <param name="deepHistory">Cascade of deep history.</param>
		internal override void Complete( IState state, Boolean deepHistory )
		{
			foreach( var region in regions )
				region.Initialise( state, deepHistory );

			base.Complete( state, deepHistory );
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
		public override bool Process( IState state, Object message )
		{
			if( state.IsTerminated )
				return false;

			return base.Process( state, message ) || regions.Aggregate( false, ( result, region ) => result || region.Process( state, message ) );
		}
	}
}