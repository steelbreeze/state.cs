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

namespace Steelbreeze.Behavior.Tests.History
{
	public class Shallow
	{
		public static void Test()
		{
			var machine = new Region<State>( "history" );

			var initial = new PseudoState<State>( "initial", PseudoStateKind.Initial, machine );
			var shallow = new CompositeState<State>( "shallow", machine );
			var deep = new SimpleState<State>( "deep", machine );
			var final = new FinalState<State>( "final", machine );

			var s1 = new SimpleState<State>( "s1", shallow );
			var s2 = new SimpleState<State>( "s2", shallow );

			new Transition<State>( initial, shallow );
			new Transition<State>( new PseudoState<State>( "shallow", PseudoStateKind.ShallowHistory, shallow ), s1 );
			new Transition<State, String>( s1, s2, c => c.Equals( "move" ) );
			new Transition<State, String>( shallow, deep, c => c.Equals( "go deep" ) );
			new Transition<State, String>( deep, shallow, c => c.Equals( "go shallow" ) );
			new Transition<State, String>( s2, final, c => c.Equals( "end" ) );

			var state = new State();

			machine.Initialise( state );

			Trace.Assert( machine.Process( state, "move" ) );
			Trace.Assert( machine.Process( state, "go deep" ) );
			Trace.Assert( machine.Process( state, "go shallow" ) );
			Trace.Assert( !machine.Process( state, "go shallow" ) );
			Trace.Assert( machine.Process( state, "end" ) );
			Trace.Assert( machine.IsComplete( state ) );
		}
	}
}