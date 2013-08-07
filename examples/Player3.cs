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

		public Player3() : base( "player" )
		{
			// create some states
			var initial = new PseudoState( "initial", PseudoStateKind.Initial, this );
			var operational = new CompositeState( "operational", this );
			var flipped = new SimpleState( "flipped", this );
			var final = new FinalState( "final", this );

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
			new Completion( initial, operational );
			new Completion( history, stopped );
			new Transition<String>( stopped, running, s => s.Equals( "play" ) );
			new Transition<String>( active, stopped, s => s.Equals( "stop" ) );
			new Transition<String>( running, paused, s => s.Equals( "pause" ) );
			new Transition<String>( paused, running, s => s.Equals( "play" ) );
			new Transition<String>( operational, flipped, s => s.Equals( "flip" ) );
			new Transition<String>( flipped, operational, s => s.Equals( "flip" ) );
			new Transition<String>( operational, final, s => s.Equals( "off" ) );
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