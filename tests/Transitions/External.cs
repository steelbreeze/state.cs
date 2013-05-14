// Alamo Project
// Copyright © 2008-13 Steelbreeze, all rights reserved
// All code contained herein is provided to you 'AS IS' without warantees of any kind.
using System;
using System.Diagnostics;
using Steelbreeze.Behavior;

namespace Steelbreeze.Behavior.Test.Transitions
{
	public class External
	{
		public static void Test()
		{
			var stateMachine = new State( "external" ); 

			var initial = new PseudoState( PseudoStateKind.Initial, stateMachine );
			var composite = new State( "composite", stateMachine );
			var orthogonal = new State( "orthogonal", stateMachine );
			var final = new FinalState( "final", stateMachine);

			var c1 = new State( "c1", composite );
			var c2 = new State( "c2", composite );
		
			var region1 = new Region( "region1", orthogonal );
			var region2 = new Region( "region2", orthogonal );

			var o1 = new State( "o1", region1 );
			var o2 = new State( "o2", region2 );

			var j1 = new PseudoState( PseudoStateKind.Junction, region2 );

			new Completion( initial, composite );
			new Completion( new PseudoState( PseudoStateKind.Initial, composite ), c1 );
			new Transition<String>( c2, c1, command => command == "1" );
			new Transition<String>( c1, j1, command => command == "2" );
			new Completion( j1, o1 );
			new Transition<String>( o1, o2, command => command == "3" );
			new Transition<String>( o2, c2, command => command == "4" );
			new Transition<String>( composite, orthogonal, command => command == "5" );
			new Transition<String>( composite, final, command => command == "x" );

			new Completion( new PseudoState( PseudoStateKind.Initial, region1 ), o1 );
			new Completion( new PseudoState( PseudoStateKind.Initial, region2 ), o2 );

			stateMachine.Initialise();

			Trace.Assert( !stateMachine.Process( "1" ) );
			Trace.Assert(  stateMachine.Process( "2" ) );
			Trace.Assert( !stateMachine.Process( "4" ) );
			Trace.Assert(  stateMachine.Process( "3" ) );
			Trace.Assert(  stateMachine.Process( "4" ) );
			Trace.Assert(  stateMachine.Process( "1" ) );
			Trace.Assert( !stateMachine.Process( "z" ) );
			Trace.Assert(  stateMachine.Process( "x" ) );
			Trace.Assert( stateMachine.IsComplete );
		}
	}
}