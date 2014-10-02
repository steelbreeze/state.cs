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
		static void Main() {
			// create the state machine model
			var model = new StateMachine<PlayerState>( "player" );
			var initial = new PseudoState<PlayerState>( "initial", model, PseudoStateKind.Initial );
			var operational = new State<PlayerState>( "operational", model );
			var final = new FinalState<PlayerState>( "final", model );

			var history = new PseudoState<PlayerState>( "history", operational, PseudoStateKind.DeepHistory );
			var stopped = new State<PlayerState>( "stopped", operational );
			var active = new State<PlayerState>( "active", operational ).Entry( EngageHead ).Exit( DisengageHead );

			var running = new State<PlayerState>( "running", active ).Entry( StartMotor ).Exit( StartMotor );
			var paused = new State<PlayerState>( "paused", active );

			// create the transitions between vertices of the model
			initial.To( operational ).Effect( DisengageHead, StartMotor );
			history.To( stopped );
			stopped.To( running ).When<String>( command => command == "play" );
			active.To( stopped ).When<String>( command => command == "stop" );
			running.To( paused ).When<String>( command => command == "pause" );
			paused.To( running ).When<String>( command => command == "play" );
			operational.To( final ).When<String>( command => command == "off" );

			// add an internal transition to show the current state at any time			
			operational.When<String>( command => command.StartsWith( "current" ) ).Effect( state => Console.WriteLine( state.Element ) );

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
	}
}