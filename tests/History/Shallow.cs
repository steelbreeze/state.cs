// Alamo Project
// Copyright © 2008-13 Steelbreeze, all rights reserved
// All code contained herein is provided to you 'AS IS' without warantees of any kind.
using System;
using System.Diagnostics;
using Steelbreeze.Behavior;

namespace Steelbreeze.Behavior.Test.History
{
	public class Shallow
	{
		public static void Test()
		{
			var machine = new Region( "history" );

			var initial = new PseudoState( "initial", PseudoStateKind.Initial, machine );
			var shallow = new CompositeState( "shallow", machine );
			var deep = new SimpleState( "deep", machine );
			var final = new FinalState( "final", machine );

			var s1 = new SimpleState( "s1", shallow );
			var s2 = new SimpleState( "s2", shallow );

			new Completion( initial, shallow );
			new Completion( new PseudoState( "shallow", PseudoStateKind.ShallowHistory, shallow ), s1 );
			new Transition<String>( s1, s2, c => c.Equals( "move" ) );
			new Transition<String>( shallow, deep, c => c.Equals( "go deep" ) );
			new Transition<String>( deep, shallow, c => c.Equals( "go shallow" ) );
			new Transition<String>( s2, final, c => c.Equals( "end" ) );

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