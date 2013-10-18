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
	/// A region within a state machine model.
	/// </summary>
	/// <remarks>
	/// A region is a container for states and pseudo states.
	/// </remarks>
	public class Region : IRegion
	{
		IElement IElement.Owner { get { return owner; } }

		/// <summary>
		/// Holds the initial pseudo state for the composote state upon initial entry or subsiquent entry without history
		/// </summary>
		PseudoState IRegion.Initial { get; set; }

		private readonly OrthogonalState owner;

		/// <summary>
		/// The name of the region.
		/// </summary>
		public String Name { get; private set; }

		/// <summary>
		/// Creates a new region within a state machine.
		/// </summary>
		/// <param name="name">The name of the region.</param>
		/// <param name="owner">The optional parent orthogonal state.</param>
		/// <remarks>
		/// A region is a container of states and pseudo states within a state machine model; it can be used as a root state machine.
		/// </remarks>
		public Region( String name, OrthogonalState owner = null )
		{
			this.Name = name;
			this.owner = owner;

			if( this.owner != null )
				( this.owner.regions ?? ( this.owner.regions = new HashSet<Region>() ) ).Add( this );
		}

		/// <summary>
		/// Determines if a region is completed.
		/// </summary>
		/// <param name="context">The state machine state to test completeness for.</param>
		/// <returns>A boolean value indicating that the region is completed.</returns>
		/// <remarks>A region is deemed to be completed when its current child state is a final state.</remarks>
		public Boolean IsComplete( IState context )
		{
			return context.IsTerminated || context.GetCurrent( this ).IsFinalState;
		}

		/// <summary>
		/// Initialises the state machine state context with its initial state.
		/// </summary>
		/// <param name="context">The state machine state context to initialise.</param>
		public void Initialise( IState context )
		{
			IRegion region = this;

			region.Enter( context );
			Complete( context, false );
		}

		void IElement.Exit( IState context )
		{
			var current = context.GetCurrent( this ) as IVertex;

			if( current != null )
				current.Exit( context );

			Debug.WriteLine( this, "Leave" );

			context.SetActive( this, false );
		}

		void IElement.Enter( IState context )
		{
			IRegion region = this;

			if( context.GetActive( region ) )
				region.Exit( context );

			Debug.WriteLine( this, "Enter" );

			context.SetActive( region, true );
		}

		internal void Complete( IState context, Boolean deepHistory )
		{
			IRegion region = this;
			IVertex current = deepHistory || region.Initial.Kind.IsHistory() ? context.GetCurrent( this ) as IVertex ?? region.Initial : region.Initial;

			current.Enter( context );
			current.Complete( context, deepHistory || region.Initial.Kind == PseudoStateKind.DeepHistory );
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

		/// <summary>
		/// Returns the fully qualified name of the region.
		/// </summary>
		/// <returns>The fully qualified name of the region.</returns>
		public override string ToString()
		{
			return this.owner != null ? this.owner + "." + this.Name : this.Name;
		}
	}
}
