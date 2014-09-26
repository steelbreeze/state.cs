/* state v5 finite state machine library
 * Copyright (c) 2014 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;

namespace Steelbreeze.Behavior.StateMachines.Examples
{
	/// <summary>
	/// Basic example of state machine state implementation
	/// </summary>
	public sealed class PlayerState : DictionaryContext<PlayerState>
	{
		public readonly String Name;

		public PlayerState( String name )
		{
			this.Name = name;
		}

		public override string ToString()
		{
			return this.Name;
		}
	}

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
			// create the state machine model
			var model = new StateMachine<PlayerState>( "player" );
			var initial = new PseudoState<PlayerState>( "initial", model, PseudoStateKind.Initial );
			var operational = new State<PlayerState>( "operational", model );
			var flipped = new State<PlayerState>( "flipped", model );
			var final = new FinalState<PlayerState>( "final", model );
			var terminated = new PseudoState<PlayerState>( "terminated", model, PseudoStateKind.Terminate );

			var history = new PseudoState<PlayerState>( "history", operational, PseudoStateKind.DeepHistory );
			var stopped = new State<PlayerState>( "stopped", operational );
			var active = new State<PlayerState>( "active", operational );

			var running = new State<PlayerState>( "running", active );
			var paused = new State<PlayerState>( "paused", active );

			// some state behaviour
			active.Entry += EngageHead;
			active.Exit += DisengageHead;
			running.Entry += StartMotor;
			running.Exit += StopMotor;

			// create transitions between states (one with transition behaviour)	
			initial.To( operational ).Do( DisengageHead, StartMotor );
			history.To( stopped );
			stopped.To( running ).When<String>( ( state, command ) => command.Equals( "play" ) );
			active.To( stopped ).When<String>( ( state, command ) => command.Equals( "stop" ) );
			running.To( paused ).When<String>( ( state, command ) => command.Equals( "pause" ) );
			paused.To( running ).When<String>( ( state, command ) => command.Equals( "play" ) );
			operational.To( flipped ).When<String>( ( state, command ) => command.Equals( "flip" ) );
			flipped.To( operational ).When<String>( ( state, command ) => command.Equals( "flip" ) );
			operational.To( final ).When<String>( ( state, command ) => command.Equals( "off" ) );
			operational.To( terminated ).When<String>( ( state, command ) => command.Equals( "term" ) );
			operational.When<String>( ( state, command ) => command.StartsWith( "help" ) ).Do( () => Console.WriteLine( "help yourself" ) );

			// create an instance of the state machine state
			var instance = new PlayerState( "example" );

			// initialises the state machine state (enters the region for the first time, causing transition from the initial PseudoState)
			model.Initialise( instance );

			// main event loop
			while( !model.IsComplete( instance ) ) {
				// write a prompt
				Console.Write( "alamo> " );

				// process lines read from the console
				if( !model.Evaluate( instance, Console.ReadLine() ) )
					Console.WriteLine( "unknown command" );
			}

			Console.WriteLine( "Press any key to quit" );
			Console.ReadKey();
		}

		private static void EngageHead( PlayerState context )
		{
			Console.WriteLine( "- engaging head" );
		}

		private static void DisengageHead( PlayerState context )
		{
			Console.WriteLine( "- disengaging head" );
		}

		private static void StartMotor( PlayerState context )
		{
			Console.WriteLine( "- starting motor" );
		}

		private static void StopMotor( PlayerState context )
		{
			Console.WriteLine( "- stopping motor" );
		}
	}
}