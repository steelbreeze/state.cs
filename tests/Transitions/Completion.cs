// Alamo Project
// Copyright © 2013 Steelbreeze, all rights reserved
// All code contained herein is provided to you 'AS IS' without warrantees of any kind.
using System;
using System.Diagnostics;
using Steelbreeze.Behavior;

namespace Steelbreeze.Behavior.Test.Transitions
{
	public class Completions
	{
		public class Activity : State
		{
			public Activity( String name, Region region )
				: base(  name, region )
			{
				new Transition( new PseudoState( PseudoStateKind.Initial, this ), new FinalState( "final", this ) );
			}
		}

		public static void Test()
		{
			try
			{
				var stateMachine = new Region( "continuation" );

				var initial = new PseudoState( PseudoStateKind.Initial, stateMachine );
				var activity1 = new State( "activity1", stateMachine );
				var activity2 = new Activity( "activity2", stateMachine );
				var junction1 = new PseudoState( PseudoStateKind.Junction, stateMachine );
				var junction2 = new PseudoState( PseudoStateKind.Junction, stateMachine );
				var activity3 = new State( "activity3", stateMachine );
				var final = new FinalState( "final", stateMachine );

				new Transition( initial, activity1 );
				new Transition( activity1, activity2 );
				new Transition( activity2, junction1 );
				new Transition( junction1, junction2 );
				new Transition( junction2, activity3 );
				new Transition( activity3, final );

				stateMachine.Initialise();

				Trace.Assert( stateMachine.IsComplete );
			}
			catch( Exception x )
			{
				Trace.Fail( x.Message );
			}
		}
	}
}