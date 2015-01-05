/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Diagnostics;

namespace Steelbreeze.Behavior.StateMachines.Tests.Transitions
{
	public class Completions
	{
		public class Activity<TState> : State<TState> where TState : class, IContext<TState>
		{
			public Activity( String name, Region<TState> region )
				: base(  name, region )
			{
				new PseudoState<TState>( "initial", this ).To( new FinalState<TState>( "final", this ) );
			}
		}

		public static void Test()
		{
			try
			{
				var model = new StateMachine<DictionaryContext>( "continuation" );
				var initial = new PseudoState<DictionaryContext>( "initial", model );
				var activity1 = new State<DictionaryContext>( "activity1", model );
				var activity2 = new Activity<DictionaryContext>( "activity2", model );
				var junction1 = new PseudoState<DictionaryContext>( "junction1", model, PseudoStateKind.Junction );
				var junction2 = new PseudoState<DictionaryContext>( "junction2", model, PseudoStateKind.Junction );
				var activity3 = new State<DictionaryContext>( "activity3", model );
				var final = new FinalState<DictionaryContext>( "final", model );

				initial.To( activity1 );
				activity1.To( activity2 );
				activity2.To( junction1 );
				junction1.To( junction2 );
				junction2.To( activity3 );
				activity3.To( final );

				var instance = new DictionaryContext();

				model.Initialise( instance );

				Trace.Assert( model.IsComplete( instance ) );
			}
			catch( Exception x )
			{
				Trace.Fail( x.Message );
			}
		}
	}
}