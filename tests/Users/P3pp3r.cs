/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Diagnostics;

namespace Steelbreeze.Behavior.StateMachines.Tests.Users {
	/// <summary>
	/// Test relating to external transitions and orthogonal regions; a transition in one region renders a potential transition in another region obsolete
	/// </summary>
	public class P3pp3r {
		public static void Test() {
			var main = new StateMachine<DictionaryContext>( "p3pp3r" );
			var initial = new PseudoState<DictionaryContext>( "initial", main, PseudoStateKind.Initial );
			var state1 = new State<DictionaryContext>( "state1", main );
			var state2 = new State<DictionaryContext>( "state2", main );

			var regionA = new Region<DictionaryContext>( "regionA", state1 );
			var initialA = new PseudoState<DictionaryContext>( "initialA", regionA, PseudoStateKind.Initial );
			var state3 = new State<DictionaryContext>( "state3", regionA );
			var state8 = new State<DictionaryContext>( "state8", regionA );

			var regionB = new Region<DictionaryContext>( "regionB", state1 );
			var initialB = new PseudoState<DictionaryContext>( "initialB", regionB, PseudoStateKind.Initial );
			var state4 = new State<DictionaryContext>( "state4", regionB );
			var state5 = new State<DictionaryContext>( "state5", regionB );

			var regionBa = new Region<DictionaryContext>( "regionBa", state4 );
			var initialBa = new PseudoState<DictionaryContext>( "initialBa", regionBa, PseudoStateKind.Initial );
			var state6 = new State<DictionaryContext>( "state6", regionBa );

			var regionBb = new Region<DictionaryContext>( "regionBb", state4 );
			var initialBb = new PseudoState<DictionaryContext>( "initialBb", regionBb, PseudoStateKind.Initial );
			var state7 = new State<DictionaryContext>( "state7", regionBb );

			initial.To( state1 );
			initialA.To( state3 );
			initialB.To( state4 );
			initialBa.To( state6 );
			initialBb.To( state7 );

			state3.To( state2 ).When<String>( c => c == "event2" );
			state3.To( state8 ).When<String>( c => c == "event1" );

			state7.To( state5 ).When<String>( c => c == "event2" ).Effect( () => Trace.TraceError("p3pp3r test failed") );
			state7.To( state5 ).When<String>( c => c == "event1" );

			var sms = new DictionaryContext();
			main.Initialise( sms );

			main.Evaluate( "event2", sms );
		}
	}
}