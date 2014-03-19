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
using System.Collections.Generic;
using System.Linq;

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// An orthogonal state is a state that contains regions.
	/// </summary>
	/// <remarks>
	/// Orthogonal states allow seperation of mutually exclusive child states allowing them to independantly respond to messages. 
	/// </remarks>
	public class OrthogonalState<TState> : SimpleState<TState> where TState : IState<TState>
	{
		internal readonly ICollection<Region<TState>> regions = new HashSet<Region<TState>>();

		/// <summary>
		/// Creates an orthogonal state within an owning (parent) region.
		/// </summary>
		/// <param name="name">The name of the orthogonal state.</param>
		/// <param name="owner">The owning (parent) region.</param>
		/// <remarks>
		/// An orthogonal state is a container of regions within a state machine model; it can be used as a root state machine.
		/// </remarks>
		public OrthogonalState( String name, Region<TState> owner ) : base( name, owner ) { }

		/// <summary>
		/// Creates an orthogonal state within an owning (parent) composite state.
		/// </summary>
		/// <param name="name">The name of the orthogonal state.</param>
		/// <param name="owner">The owning (parent) composite state.</param>
		/// <remarks>
		/// An orthogonal state is a container of regions within a state machine model; it can be used as a root state machine.
		/// </remarks>
		public OrthogonalState( String name, CompositeState<TState> owner ) : base( name, owner ) { }

		/// <summary>
		/// Creates a new child Region within the StateMachine
		/// </summary>
		/// <param name="name">The name of the Region</param>
		/// <returns>The new child Region</returns>
		public Region<TState> CreateRegion( String name )
		{
			return new Region<TState>( name, this );
		}

		internal override bool IsComplete( TState state )
		{
			if( !state.IsTerminated )
				foreach( var region in this.regions )
					if( !region.IsComplete( state ) )
						return false;

			return true;
		}

		internal override void BeginExit( TState state )
		{
			foreach( var region in this.regions )
			{
				if( state.GetActive( region ) )
				{
					region.BeginExit( state );
					region.EndExit( state );
				}
			}
		}

		internal override void EndEntry( TState state, bool deepHistory )
		{
			foreach( var region in this.regions )
			{
				region.BeginEntry( state );
				region.EndEntry( state, deepHistory );
			}

			base.EndEntry( state, deepHistory );
		}

		internal override bool Process( TState state, object message )
		{
			if( state.IsTerminated )
				return false;

			return base.Process( state, message ) || regions.Aggregate( false, ( result, region ) => region.Process( state, message ) || result );
		}
	}
}
