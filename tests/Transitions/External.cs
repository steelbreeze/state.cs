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
using System.Diagnostics;
using Steelbreeze.Behavior;

namespace Steelbreeze.Behavior.Tests.Transitions
{
	public class External
	{
		public static void Test()
		{
			var stateMachine = new StateMachine<State>( "external" );
			var region = stateMachine.CreateRegion( "external" );

			var initial = region.CreatePseudoState( "initial", PseudoStateKind.Initial );
			var composite = region.CreateCompositeState( "composite" );
			var orthogonal = region.CreateOrthogonalState( "orthogonal" );
			var final = region.CreateFinalState( "final" );

			var c1 = composite.CreateSimpleState( "c1" );
			var c2 = composite.CreateSimpleState( "c2" );
		
			var region1 = orthogonal.CreateRegion( "region1" );
			var region2 = orthogonal.CreateRegion( "region2" );

			var o1 = region1.CreateSimpleState( "o1" );
			var o2 = region2.CreateSimpleState( "o2" );

			var j1 = region2.CreatePseudoState( "junction", PseudoStateKind.Junction );

			stateMachine.CreateTransition( initial, composite );
			stateMachine.CreateTransition( composite.CreatePseudoState( "initial", PseudoStateKind.Initial ), c1 );
			stateMachine.CreateTransition<String>( c2, c1, command => command == "1" );
			stateMachine.CreateTransition<String>( c1, j1, command => command == "2" );
			stateMachine.CreateElse( j1, o1 );
			stateMachine.CreateTransition<String>( o1, o2, command => command == "3" );
			stateMachine.CreateTransition<String>( o2, c2, command => command == "4" );
			stateMachine.CreateTransition<String>( composite, orthogonal, command => command == "5" );
			stateMachine.CreateTransition<String>( composite, final, command => command == "x" );

			stateMachine.CreateTransition( region1.CreatePseudoState( "initial", PseudoStateKind.Initial ), o1 );
			stateMachine.CreateTransition( region2.CreatePseudoState( "initial", PseudoStateKind.Initial ), o2 );

			var state = new State();

			stateMachine.Initialise( state );

			Trace.Assert( !stateMachine.Process( state, "1" ) );
			Trace.Assert( stateMachine.Process( state, "2" ) );
			Trace.Assert( !stateMachine.Process( state, "4" ) );
			Trace.Assert( stateMachine.Process( state, "3" ) );
			Trace.Assert( stateMachine.Process( state, "4" ) );
			Trace.Assert( stateMachine.Process( state, "1" ) );
			Trace.Assert( !stateMachine.Process( state, "z" ) );
			Trace.Assert( stateMachine.Process( state, "x" ) );
			Trace.Assert( stateMachine.IsComplete( state ) );
		}
	}
}