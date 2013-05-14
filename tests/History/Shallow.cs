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

			var initial = new PseudoState( PseudoStateKind.Initial, machine );
			var shallow = new State( "shallow", machine );
			var deep = new State( "deep", machine );
			var final = new FinalState( "final", machine );

			var s1 = new State( "s1", shallow );
			var s2 = new State( "s2", shallow );

			new Completion( initial, shallow );
			new Completion( new PseudoState( PseudoStateKind.ShallowHistory, shallow ), s1 );
			new Transition<String>( s1, s2, c => c.Equals( "move" ) );
			new Transition<String>( shallow, deep, c => c.Equals( "go deep" ) );
			new Transition<String>( deep, shallow, c => c.Equals( "go shallow" ) );
			new Transition<String>( s2, final, c => c.Equals( "end" ) );

			machine.Initialise();

			Trace.Assert(  machine.Process( "move" ) );
			Trace.Assert(  machine.Process( "go deep" ) );
			Trace.Assert(  machine.Process( "go shallow" ) );
			Trace.Assert( !machine.Process( "go shallow" ) );
			Trace.Assert(  shallow.Default.Current.Equals( s2 ) );
			Trace.Assert(  machine.Process( "end" ) );
			Trace.Assert(  machine.IsComplete );
		}
	}
}