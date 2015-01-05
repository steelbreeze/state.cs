/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Diagnostics;

namespace Steelbreeze.Behavior.StateMachines.Tests.Users {
	public class Brice1 {
		public static void Test() {
			var model = new StateMachine<DictionaryContext>( "brice1" );

			var initial1 = new PseudoState<DictionaryContext>( "initial", model );
			var myComposite1 = new State<DictionaryContext>( "myComposite1", model );
			var myComposite2 = new State<DictionaryContext>( "myComposite2", model );

			var initial2 = new PseudoState<DictionaryContext>( "initial", myComposite1 );
			var myState1 = new State<DictionaryContext>( "myState1", myComposite1 );
			var myState2 = new State<DictionaryContext>( "myState2", myComposite1 );

			initial1.To( myComposite1 );
			initial2.To( myState1 );

			myComposite1.To( myComposite2 ).When<String>( message => message == "A" ).Effect( () => System.Diagnostics.Trace.Fail( "Outer transition fired" ) );
			myState1.To( myState2 ).When<String>( message => message == "A" );

			var instance = new DictionaryContext();

			model.Initialise( instance );

			model.Evaluate( "A", instance );
		}
	}
}