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
	/// Running and pauseed are now 
	/// </remarks>
	public class Player2
	{
		public static void Main()
		{
			// create the region
			var server = new Region( "server" );

			// create some states
			var initial = new PseudoState( PseudoStateKind.Initial, server );
			var stopped = new State( "stopped", server );
			var active = new State( "active", server );
			var final = new FinalState( "final", server );

			var running = new State( "running", active );
			var paused = new State( "paused", active );

			// create transitions between states
			new Completion( initial, stopped );
			new Transition<String>( stopped, running, s => s.Equals( "play" ) );
			new Transition<String>( active, stopped, s => s.Equals( "stop" ) );
			new Transition<String>( running, paused, s => s.Equals( "pause" ) );
			new Transition<String>( paused, running, s => s.Equals( "play" ) );
			new Transition<String>( stopped, final, s => s.Equals( "off" ) ).Effect += PowerDown;

			// some state behaviour
			active.Entry += EngageHead;
			active.Exit += DisengageHead;

			running.Entry += StartMotor;
			running.Exit += StopMotor;

			// initialises the state machine (causing transition from initial pseudo state)
			server.Initialise();
			
			// keep processing events until the state machine is complete
			while( !server.IsComplete )
			{
				// write a prompt
				Console.Write( "alamo> " );

				// process lines read from the console
				var result = server.Process( Console.ReadLine() );

				Console.WriteLine( "Process returned {0}", result );
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