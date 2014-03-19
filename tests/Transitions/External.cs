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
	public class External
	{
		public static void Test()
		{
			var stateMachine = new StateMachine<State>( "external" );
			var region = stateMachine.CreateRegion( "external" );

			var initial = region.CreatePseudoState( "initial", PseudoStateKind.Initial );
			var composite = region.CreateCompositeState( "composite" );
			var orthogonal = region.CreateOrthogonalState( "orthogonal" );
			var final = region.CreateFinalState( "final" );

			var c1 = composite.CreateSimpleState( "c1" );
			var c2 = composite.CreateSimpleState( "c2" );
		
			var region1 = orthogonal.CreateRegion( "region1" );
			var region2 = orthogonal.CreateRegion( "region2" );

			var o1 = region1.CreateSimpleState( "o1" );
			var o2 = region2.CreateSimpleState( "o2" );

			var j1 = region2.CreatePseudoState( "junction", PseudoStateKind.Junction );

			stateMachine.CreateTransition( initial, composite );
			stateMachine.CreateTransition( composite.CreatePseudoState( "initial", PseudoStateKind.Initial ), c1 );
			stateMachine.CreateTransition<String>( c2, c1, command => command == "1" );
			stateMachine.CreateTransition<String>( c1, j1, command => command == "2" );
			stateMachine.CreateElse( j1, o1 );
			stateMachine.CreateTransition<String>( o1, o2, command => command == "3" );
			stateMachine.CreateTransition<String>( o2, c2, command => command == "4" );
			stateMachine.CreateTransition<String>( composite, orthogonal, command => command == "5" );
			stateMachine.CreateTransition<String>( composite, final, command => command == "x" );

			stateMachine.CreateTransition( region1.CreatePseudoState( "initial", PseudoStateKind.Initial ), o1 );
			stateMachine.CreateTransition( region2.CreatePseudoState( "initial", PseudoStateKind.Initial ), o2 );

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