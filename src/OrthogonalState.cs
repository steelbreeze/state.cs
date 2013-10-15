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
	/// An orthogonal state is a state that contains regions.
	/// </summary>
	/// <remarks>
	/// Orthogonal states allow seperation of mutually exclusive child states allowing them to independantly respond to messages. 
	/// </remarks>
	public class OrthogonalState : SimpleState
	{
		/// <summary>
		/// The set of child regions.
		/// </summary>
		internal ICollection<Region> regions { get; set; }

		/// <summary>
		/// Creates an orthogonal state within an owning (parent) region.
		/// </summary>
		/// <param name="name">The name of the orthogonal state.</param>
		/// <param name="owner">The owning (parent) region.</param>
		/// <remarks>
		/// An orthogonal state is a container of regions within a state machine model; it can be used as a root state machine.
		/// </remarks>
		public OrthogonalState( String name, Region owner ) : base( name, owner ) { }

		/// <summary>
		/// Creates an orthogonal state within an owning (parent) composite state.
		/// </summary>
		/// <param name="name">The name of the orthogonal state.</param>
		/// <param name="owner">The owning (parent) composite state.</param>
		/// <remarks>
		/// An orthogonal state is a container of regions within a state machine model; it can be used as a root state machine.
		/// </remarks>
		public OrthogonalState( String name, CompositeState owner ) : base( name, owner ) { }

		/// <summary>
		/// Tests the orthogonal state for completeness.
		/// </summary>
		/// <param name="context">The state machine state to test.</param>
		/// <returns>True if the all the child regions are complete.</returns>
		public override bool IsComplete( IState context )
		{
			return context.IsTerminated || regions.All( region => region.IsComplete( context ) );
		}

		internal override void OnExit( IState context )
		{
			foreach( IRegion region in regions )
				if( context.GetActive( region ) )
					region.Exit( context );

			base.OnExit( context );
		}

		internal override void OnComplete( IState context, bool deepHistory )
		{
			foreach( var region in regions )
			{
				( region as IRegion ).Enter( context );
				region.Complete( context, deepHistory );
			}

			base.OnComplete( context, deepHistory );
		}

		/// <summary>
		/// Attempts to process a message against an orthogonal state.
		/// </summary>
		/// <param name="context">The state machine state.</param>
		/// <param name="message">The message to evaluate.</param>
		/// <returns>A boolean indicating if the message caused a state change.</returns>
		public override bool Process( IState context, object message )
		{
			if( context.IsTerminated )
				return false;

			return base.Process( context, message ) || regions.Aggregate( false, ( result, region ) => region.Process( context, message ) || result );
		}
	}
}
