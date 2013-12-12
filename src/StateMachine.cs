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
using System.Text;

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// The root of a state machine; container for Regions
	/// </summary>
	public class StateMachine : Element
	{
		/// <summary>
		/// Function to name default regions for state machines.
		/// </summary>
		public static Func<StateMachine, String> DefaultRegionName = sm => sm.Name;

		/// <summary>
		/// Returns (and creates if necessary) the default region for a state machine.
		/// </summary>
		/// <param name="stateMachine">The state machine to get the default region for.</param>
		/// <returns></returns>
		public static implicit operator Region( StateMachine stateMachine )
		{
			return stateMachine.regions.SingleOrDefault( region => region.Name.Equals( DefaultRegionName( stateMachine ) ) ) ?? new Region( DefaultRegionName( stateMachine ), stateMachine );
		}

		internal readonly ICollection<Region> regions = new HashSet<Region>();

		/// <summary>
		/// Creates a new instance of the StateMachine class
		/// </summary>
		/// <param name="name">The name of the state macchine</param>
		public StateMachine( String name ) : base( name, null ) { }

		/// <summary>
		/// Tests the state machine for completeness.
		/// </summary>
		/// <param name="context">The state machine state to test.</param>
		/// <returns>True if the all the child regions are complete.</returns>
		public bool IsComplete( IState context )
		{
			if( !context.IsTerminated )
				foreach( var region in this.regions )
					if( !region.IsComplete( context ) )
						return false;

			return true;
		}

		/// <summary>
		/// Initialises a state machine to its initial state
		/// </summary>
		/// <param name="context">The state machine state.</param>
		public void Initialise( IState context )
		{
			this.BeginEnter( context );
			this.EndEnter( context, false );
		}

		internal override void BeginExit( IState context )
		{
			foreach( var region in regions )
			{
				if( context.GetActive( region ) )
				{
					region.BeginExit( context );
					region.EndExit( context );
				}
			}
		}

		internal override void EndEnter( IState context, bool deepHistory )
		{
			foreach( var region in regions )
			{
				region.BeginEnter( context );
				region.EndEnter( context, deepHistory );
			}

			base.EndEnter( context, deepHistory );
		}

		/// <summary>
		/// Attempts to process a message against an orthogonal state.
		/// </summary>
		/// <param name="context">The state machine state.</param>
		/// <param name="message">The message to evaluate.</param>
		/// <returns>A boolean indicating if the message caused a state change.</returns>
		public bool Process( IState context, object message )
		{
			if( context.IsTerminated )
				return false;

			return regions.Aggregate( false, ( result, region ) => region.Process( context, message ) || result );
		}
	}
}