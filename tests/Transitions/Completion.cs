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
	public class Completions
	{
		public class Activity : CompositeState<State>
		{
			public Activity( String name, Region<State> region )
				: base(  name, region )
			{
				new Transition<State>( new PseudoState<State>( "initial", PseudoStateKind.Initial, this ), new FinalState<State>( "final", this ) );
			}
		}

		public static void Test()
		{
			try
			{
				var stateMachine = new Region<State>( "continuation" );

				var initial = new PseudoState<State>( "initial", PseudoStateKind.Initial, stateMachine );
				var activity1 = new SimpleState<State>( "activity1", stateMachine );
				var activity2 = new Activity( "activity2", stateMachine );
				var junction1 = new PseudoState<State>( "junction1", PseudoStateKind.Junction, stateMachine );
				var junction2 = new PseudoState<State>( "junction2", PseudoStateKind.Junction, stateMachine );
				var activity3 = new SimpleState<State>( "activity3", stateMachine );
				var final = new FinalState<State>( "final", stateMachine );

				new Transition<State>( initial, activity1 );
				new Transition<State>( activity1, activity2 );
				new Transition<State>( activity2, junction1 );
				new Transition<State>( junction1, junction2 );
				new Transition<State>( junction2, activity3 );
				new Transition<State>( activity3, final );

				var state = new State();

				stateMachine.Initialise( state );

				Trace.Assert( stateMachine.IsComplete( state ) );
			}
			catch( Exception x )
			{
				Trace.Fail( x.Message );
			}
		}
	}
}