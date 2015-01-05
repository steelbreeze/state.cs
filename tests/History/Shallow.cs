/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Diagnostics;

namespace Steelbreeze.Behavior.StateMachines.Tests.History {
	public class Shallow {
		public static void Test() {
			var model = new StateMachine<DictionaryContext>( "history" );

			var initial = new PseudoState<DictionaryContext>( "initial", model );
			var shallow = new State<DictionaryContext>( "shallow", model );
			var deep = new State<DictionaryContext>( "deep", model );
			var final = new FinalState<DictionaryContext>( "final", model );

			var s1 = new State<DictionaryContext>( "s1", shallow );
			var s2 = new State<DictionaryContext>( "s2", shallow );

			initial.To( shallow );
			new PseudoState<DictionaryContext>( "shallow", shallow, PseudoStateKind.ShallowHistory ).To( s1 );
			s1.To( s2 ).When<String>( c => c == "move" );
			shallow.To( deep ).When<String>( c => c == "go deep" );
			deep.To( shallow ).When<String>( c => c == "go shallow" );
			s2.To( final ).When<String>( c => c == "end" );

			var instance = new DictionaryContext();

			model.Initialise( instance );

			Trace.Assert( model.Evaluate( "move", instance ) );
			Trace.Assert( model.Evaluate( "go deep", instance ) );
			Trace.Assert( model.Evaluate( "go shallow", instance ) );
			Trace.Assert( !model.Evaluate( "go shallow", instance ) );
			Trace.Assert( model.Evaluate( "end", instance ) );
			Trace.Assert( model.IsComplete( instance ) );
		}
	}
}
