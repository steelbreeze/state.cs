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
using Steelbreeze.Behavior;

namespace Steelbreeze.Behavior.Examples
{
	/// <summary>
	/// A controller for a simple cassette player
	/// </summary>
	/// <remarks>
	/// The player now inherits from a Region, so can be used in isolation, or as a region in a larger device.
	/// The player now implements an 'operational' composite state so the off command can be used while in any sub-state.
	/// </remarks>
	public class Player
	{
		static void Main()
		{
			// create the state machine
			var player = new StateMachine<State>( "player" );

			// create some states
			var initial = player.CreatePseudoState( "initial", PseudoStateKind.Initial );
			var operational = player.CreateCompositeState( "operational" );
			var flipped = player.CreateSimpleState( "flipped" );
			var final = player.CreateFinalState( "final" );
			var terminated = player.CreatePseudoState( "terminated", PseudoStateKind.Terminate );

			var history = operational.CreatePseudoState( "history", PseudoStateKind.DeepHistory );
			var stopped = operational.CreateSimpleState( "stopped" );
			var active = operational.CreateCompositeState( "active" );

			var running = active.CreateSimpleState( "running" );
			var paused = active.CreateSimpleState( "paused" );

			// some state behaviour
			active.Entry += EngageHead;
			active.Exit += DisengageHead;

			running.Entry += StartMotor;
			running.Exit += StopMotor;

			// create transitions between states (one with transition behaviour)
			var t0 = player.CreateTransition( initial, operational );
			player.CreateTransition( history, stopped );
			player.CreateTransition<String>( stopped, running, s => s.Equals( "play" ) );
			player.CreateTransition<String>( active, stopped, s => s.Equals( "stop" ) );
			player.CreateTransition<String>( running, paused, s => s.Equals( "pause" ) );
			player.CreateTransition<String>( paused, running, s => s.Equals( "play" ) );
			player.CreateTransition<String>( operational, flipped, s => s.Equals( "flip" ) );
			player.CreateTransition<String>( flipped, operational, s => s.Equals( "flip" ) );
			player.CreateTransition<String>( operational, final, s => s.Equals( "off" ) );
			player.CreateTransition<String>( operational, terminated, s => s.Equals( "term" ) );
			var help = player.CreateTransition<String>( operational, operational, s => s.StartsWith( "help" ) );

			t0.Effect += DisengageHead;
			t0.Effect += StopMotor;
			help.Effect += ( c, s ) => Console.WriteLine( "help yourself" );

			var state = new State();

			// initialises the state machine (enters the region for the first time, causing transition from the initial PseudoState)
			player.Initialise( state );

			// main event loop
			while( !player.IsComplete( state ) )
			{
				// write a prompt
				Console.Write( "alamo> " );

				// process lines read from the console
				if( !player.Process( state, Console.ReadLine() ) )
					Console.WriteLine( "unknown command" );
			}

			Console.WriteLine( "Press any key to quit" );
			Console.ReadKey();
		}

		private static void EngageHead( State context )
		{
			Console.WriteLine( "- engaging head" );
		}

		private static void DisengageHead( State context )
		{
			Console.WriteLine( "- disengaging head" );
		}

		private static void StartMotor( State context )
		{
			Console.WriteLine( "- starting motor" );
		}

		private static void StopMotor( State context )
		{
			Console.WriteLine( "- stopping motor" );
		}
	}
}