// The MIT License (MIT)
//
// Copyright (c) 2014 Steelbreeze Limited
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Diagnostics;
using Steelbreeze.Behavior;

namespace Steelbreeze.Behavior.Tests.Transitions
{
	public class Completions
	{
		public class Activity : CompositeState<State>
		{
			public Activity( String name, Region<State> region )
				: base(  name, region )
			{
				new Transition<State>( new PseudoState<State>( "initial", PseudoStateKind.Initial, this ), new FinalState<State>( "final", this ) );
			}
		}

		public static void Test()
		{
			try
			{
				var stateMachine = new Region<State>( "continuation" );

				var initial = new PseudoState<State>( "initial", PseudoStateKind.Initial, stateMachine );
				var activity1 = new SimpleState<State>( "activity1", stateMachine );
				var activity2 = new Activity( "activity2", stateMachine );
				var junction1 = new PseudoState<State>( "junction1", PseudoStateKind.Junction, stateMachine );
				var junction2 = new PseudoState<State>( "junction2", PseudoStateKind.Junction, stateMachine );
				var activity3 = new SimpleState<State>( "activity3", stateMachine );
				var final = new FinalState<State>( "final", stateMachine );

				new Transition<State>( initial, activity1 );
				new Transition<State>( activity1, activity2 );
				new Transition<State>( activity2, junction1 );
				new Transition<State>( junction1, junction2 );
				new Transition<State>( junction2, activity3 );
				new Transition<State>( activity3, final );

				var state = new State();

				stateMachine.Initialise( state );

				Trace.Assert( stateMachine.IsComplete( state ) );
			}
			catch( Exception x )
			{
				Trace.Fail( x.Message );
			}
		}
	}
}