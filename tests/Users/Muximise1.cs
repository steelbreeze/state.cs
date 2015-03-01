/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Diagnostics;

namespace Steelbreeze.Behavior.StateMachines.Tests.Users {
	public class Muximise1 {
		public static void Test() {
			var model = new StateMachine<DictionaryContext>( "muximise1" );

			var initial = new PseudoState<DictionaryContext>( "initial", model );
			var ortho = new State<DictionaryContext>( "ortho", model );
			var simple = new State<DictionaryContext>( "simple", model );
			var final = new FinalState<DictionaryContext>( "final", model );

			var r1 = new Region<DictionaryContext>( "r1", ortho );
			var r2 = new Region<DictionaryContext>( "r2", ortho );

			var i1 = new PseudoState<DictionaryContext>( "initial", r1, PseudoStateKind.ShallowHistory );
			var i2 = new PseudoState<DictionaryContext>( "initial", r2, PseudoStateKind.ShallowHistory );

			var s1 = new State<DictionaryContext>( "s1", r1 );
			var s2 = new State<DictionaryContext>( "s2", r2 );

			var f1 = new FinalState<DictionaryContext>( "f1", r1 );
			var f2 = new FinalState<DictionaryContext>( "f2", r2 );

			initial.To( ortho );

			i1.To( s1 );
			i2.To( s2 );

			ortho.To( final ); // This should happen once all regions in ortho are complete?

			s1.To( f1 ).When<String>( c => c == "complete1" );
			s2.To( f2 ).When<String>( c => c == "complete2" );

			ortho.To( simple ).When<String>( c => c == "jump" );
			simple.To( ortho ).When<String>( c => c == "back" );

			var instance = new DictionaryContext();

			model.Initialise( instance );

			model.Evaluate( "complete1", instance );
			model.Evaluate( "complete2", instance );

			Trace.Assert( model.IsComplete( instance ) );
		}
	}
}