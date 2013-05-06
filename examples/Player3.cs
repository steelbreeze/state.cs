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
	public class Player3 : Region
	{
		public static void Main()
		{
			// create a player
			var player = new Player3();

			// initialises the state machine (enters the region for the first time, causing transition from the initial PseudoState)
			player.Initialise();

			// main event loop
			while( !player.IsComplete )
			{
				// write a prompt
				Console.Write( "alamo> " );

				// process lines read from the console
				if( !player.Process( Console.ReadLine() ) )
					Console.WriteLine( "unknown command" );
			}

			Console.WriteLine( "Press any key to quit" );
			Console.ReadKey();
		}

		public Player3() : base( "player" )
		{
			// create some states
			var initial = new PseudoState( PseudoStateKind.Initial, this );
			var operational = new State( "operational", this );
			var final = new FinalState( "final", this );

			var stopped = new State( "stopped", operational );
			var active = new State( "active", operational );

			var running = new State( "running", active );
			var paused = new State( "paused", active );

			// some state behaviour
			active.Entry += EngageHead;
			active.Exit += DisengageHead;

			running.Entry += StartMotor;
			running.Exit += StopMotor;

			// create transitions between states (one with transition behaviour)
			new Transition( initial, stopped );
			new Transition<String>( stopped, running, s => s.Equals( "play" ) );
			new Transition<String>( active, stopped, s => s.Equals( "stop" ) );
			new Transition<String>( running, paused, s => s.Equals( "pause" ) );
			new Transition<String>( paused, running, s => s.Equals( "play" ) );
			new Transition<String>( operational, final, s => s.Equals( "off" ) ).Effect += PowerDown;
		}

		private void EngageHead()
		{
			Console.WriteLine( "- engaging head" );
		}

		private void DisengageHead()
		{
			Console.WriteLine( "- disengaging head" );
		}

		private void StartMotor()
		{
			Console.WriteLine( "- starting motor" );
		}

		private void StopMotor()
		{
			Console.WriteLine( "- stopping motor" );
		}

		private void PowerDown( String command )
		{
			Console.WriteLine( "- powering down" );
		}
	}
}