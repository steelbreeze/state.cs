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

namespace Steelbreeze.Behavior.Tests.History
{
	public class Shallow
	{
		public static void Test()
		{
			var machine = new Region<State>( "history" );

			var initial = new PseudoState<State>( "initial", PseudoStateKind.Initial, machine );
			var shallow = new CompositeState<State>( "shallow", machine );
			var deep = new SimpleState<State>( "deep", machine );
			var final = new FinalState<State>( "final", machine );

			var s1 = new SimpleState<State>( "s1", shallow );
			var s2 = new SimpleState<State>( "s2", shallow );

			new Transition<State>( initial, shallow );
			new Transition<State>( new PseudoState<State>( "shallow", PseudoStateKind.ShallowHistory, shallow ), s1 );
			new Transition<State, String>( s1, s2, ( state, c ) => c.Equals( "move" ) );
			new Transition<State, String>( shallow, deep, ( state, c ) => c.Equals( "go deep" ) );
			new Transition<State, String>( deep, shallow, ( state, c ) => c.Equals( "go shallow" ) );
			new Transition<State, String>( s2, final, ( state, c ) => c.Equals( "end" ) );

			var instance = new State();

			machine.Initialise( instance );

			Trace.Assert( machine.Process( instance, "move" ) );
			Trace.Assert( machine.Process( instance, "go deep" ) );
			Trace.Assert( machine.Process( instance, "go shallow" ) );
			Trace.Assert( !machine.Process( instance, "go shallow" ) );
			Trace.Assert( machine.Process( instance, "end" ) );
			Trace.Assert( machine.IsComplete( instance ) );
		}
	}
}
