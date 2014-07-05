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

namespace Steelbreeze.Behavior.Tests.Users
{
	public class Muximise1
	{
		public static void Test()
		{
			try
			{
				var main = new Region<State>( "muximise1" );

				var initial = new PseudoState<State>( "initial", PseudoStateKind.Initial, main );
				var ortho = new OrthogonalState<State>( "ortho", main );
				var simple = new SimpleState<State>( "simple", main );
				var final = new FinalState<State>( "final", main );

				var r1 = new Region<State>( "r1", ortho );
				var r2 = new Region<State>( "r2", ortho );

				var i1 = new PseudoState<State>( "initial", PseudoStateKind.ShallowHistory, r1 );
				var i2 = new PseudoState<State>( "initial", PseudoStateKind.ShallowHistory, r2 );

				var s1 = new SimpleState<State>( "s1", r1 );
				var s2 = new SimpleState<State>( "s2", r2 );

				var f1 = new FinalState<State>( "f1", r1 );
				var f2 = new FinalState<State>( "f2", r2 );

				new Transition<State>( initial, ortho );

				new Transition<State>( i1, s1 );
				new Transition<State>( i2, s2 );

				new Transition<State>( ortho, final ); // This should happen once all regions in ortho are complete?

				new Transition<State, String>( s1, f1, ( s, c ) => c == "complete1" );
				new Transition<State, String>( s2, f2, ( s, c ) => c == "complete2" );

				new Transition<State, String>( ortho, simple, ( s, c ) => c == "jump" );
				new Transition<State, String>( simple, ortho, ( s, c ) => c == "back" );

				var state = new State();

				main.Initialise( state );

				main.Process( state, "complete1" );
				main.Process( state, "complete2" );

				Trace.Assert( main.IsComplete( state ) );

			}
			catch( Exception x )
			{
				Trace.Fail( x.Message );
			}
		}
	}
}