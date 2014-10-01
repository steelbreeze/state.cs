/* state v5 finite state machine library
 * Copyright (c) 2014 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Xml.Linq;

namespace Steelbreeze.Behavior.StateMachines.Examples
{
	/// <summary>
	/// Basic example of state machine state implementation
	/// </summary>
	public sealed class PlayerState : XmlContext<PlayerState> {
		public PlayerState( String name ) : base( new XAttribute( "name", name ) ) { }

		public override string ToString() {
			return this.Element.Attribute( "name" ).Value;
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
			var choice = new PseudoState<PlayerState>( "choice", model, PseudoStateKind.Choice );
			var final = new FinalState<PlayerState>( "final", model );

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

			// create the transitions representing a cassette player	
			initial.To( operational ).Do( DisengageHead, StartMotor );
			history.To( stopped );
			stopped.To( running ).When<String>( command => command.Equals( "play" ) );
			active.To( stopped ).When<String>( command => command.Equals( "stop" ) );
			running.To( paused ).When<String>( command => command.Equals( "pause" ) );
			paused.To( running ).When<String>( command => command.Equals( "play" ) );
			operational.To( final ).When<String>( command => command.Equals( "off" ) );
			
			// a couple of transitions to show the random nature of a Choice PseudoState and DeepHistory in action
			operational.To( choice ).When<String>( command => command == "random" );
			choice.To( operational ).Do( () => Console.WriteLine( "- transition A" ) );
			choice.To( operational ).Do( () => Console.WriteLine( "- transition B" ) );

			// add an internal transition to show the current state at any time			
			operational.When<String>( command => command.StartsWith( "current" ) ).Do( state => Console.WriteLine( state.Element ) );

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