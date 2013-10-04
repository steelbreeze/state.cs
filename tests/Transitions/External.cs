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
			var stateMachine = new Region( "external" ); 

			var initial = new PseudoState( "initial", PseudoStateKind.Initial, stateMachine );
			var composite = new CompositeState( "composite", stateMachine );
			var orthogonal = new OrthogonalState( "orthogonal", stateMachine );
			var final = new FinalState( "final", stateMachine);

			var c1 = new SimpleState( "c1", composite );
			var c2 = new SimpleState( "c2", composite );
		
			var region1 = new Region( "region1", orthogonal );
			var region2 = new Region( "region2", orthogonal );

			var o1 = new SimpleState( "o1", region1 );
			var o2 = new SimpleState( "o2", region2 );

			var j1 = new PseudoState( "junction", PseudoStateKind.Junction, region2 );

			new Completion( initial, composite );
			new Completion( new PseudoState( "initial", PseudoStateKind.Initial, composite ), c1 );
			new Transition<String>( c2, c1, command => command == "1" );
			new Transition<String>( c1, j1, command => command == "2" );
			new Completion.Else( j1, o1 );
			new Transition<String>( o1, o2, command => command == "3" );
			new Transition<String>( o2, c2, command => command == "4" );
			new Transition<String>( composite, orthogonal, command => command == "5" );
			new Transition<String>( composite, final, command => command == "x" );

			new Completion( new PseudoState( "initial", PseudoStateKind.Initial, region1 ), o1 );
			new Completion( new PseudoState( "initial", PseudoStateKind.Initial, region2 ), o2 );

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