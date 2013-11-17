// Copyright © 2008-13 Steelbreeze, all rights reserved.
// All code contained herein is provided to you 'AS IS' without warantees of any kind.
using System;
using Steelbreeze.Behavior;

namespace Steelbreeze.Examples
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
			var player = new StateMachine( "player" );

			// create some states
			var initial = new PseudoState( "initial", PseudoStateKind.Initial, player );
			var operational = new CompositeState( "operational", player );
			var flipped = new SimpleState( "flipped", player );
			var final = new FinalState( "final", player );
			var terminated = new PseudoState( "terminated", PseudoStateKind.Terminate, player );

			var history = new PseudoState( "history", PseudoStateKind.DeepHistory, operational );
			var stopped = new SimpleState( "stopped", operational );
			var active = new CompositeState( "active", operational );

			var running = new SimpleState( "running", active );
			var paused = new SimpleState( "paused", active );
			
			// some state behaviour
			active.Entry += EngageHead;
			active.Exit += DisengageHead;

			running.Entry += StartMotor;
			running.Exit += StopMotor;

			// create transitions between states (one with transition behaviour)
			var t0 = new Transition( initial, operational );
			new Transition( history, stopped );
			new Transition<String>( stopped, running, s => s.Equals( "play" ) );
			new Transition<String>( active, stopped, s => s.Equals( "stop" ) );
			new Transition<String>( running, paused, s => s.Equals( "pause" ) );
			new Transition<String>( paused, running, s => s.Equals( "play" ) );
			new Transition<String>( operational, flipped, s => s.Equals( "flip" ) );
			new Transition<String>( flipped, operational, s => s.Equals( "flip" ) );
			new Transition<String>( operational, final, s => s.Equals( "off" ) );
			new Transition<String>( operational, terminated, s => s.Equals( "term" ) );
			var help = new Transition<String>( operational, operational, s => s.StartsWith( "help" ) );

			t0.Effect += DisengageHead;
			t0.Effect += StopMotor;
			help.Effect += s => Console.WriteLine( "help yourself" );

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

		private static void EngageHead()
		{
			Console.WriteLine( "- engaging head" );
		}

		private static void DisengageHead()
		{
			Console.WriteLine( "- disengaging head" );
		}

		private static void StartMotor()
		{
			Console.WriteLine( "- starting motor" );
		}

		private static void StopMotor()
		{
			Console.WriteLine( "- stopping motor" );
		}

		private static void PowerDown( String command )
		{
			Console.WriteLine( "- powering down" );
		}
	}
}