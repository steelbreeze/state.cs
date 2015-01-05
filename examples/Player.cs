/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Xml.Linq;

namespace Steelbreeze.Behavior.StateMachines.Examples
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
		static void Main() {
			// create the state machine model
			var model = new StateMachine<XContext>( "player" );
			var initial = new PseudoState<XContext>( "initial", model, PseudoStateKind.Initial );
			var operational = new State<XContext>( "operational", model );
			var choice = new PseudoState<XContext>( "choice", model, PseudoStateKind.Choice );
			var final = new FinalState<XContext>( "final", model );

			var history = new PseudoState<XContext>( "history", operational, PseudoStateKind.DeepHistory );
			var stopped = new State<XContext>( "stopped", operational );
			var active = new State<XContext>( "active", operational ).Entry( EngageHead ).Exit( DisengageHead );

			var running = new State<XContext>( "running", active ).Entry( StartMotor ).Exit( StopMotor );
			var paused = new State<XContext>( "paused", active );

			// create the transitions between vertices of the model
			initial.To( operational ).Effect( DisengageHead, StartMotor );
			history.To( stopped );
			stopped.To( running ).When<String>( command => command == "play" );
			active.To( stopped ).When<String>( command => command == "stop" );
			running.To( paused ).When<String>( command => command == "pause" );
			paused.To( running ).When<String>( command => command == "play" );
			operational.To( final ).When<String>( command => command == "off" );
			operational.To( choice ).When<String>( command => command == "rand" );
			choice.To( operational ).Effect( () => Console.WriteLine( "- transition A back to operational" ) );
			choice.To( operational ).Effect( () => Console.WriteLine( "- transition B back to operational" ) );

			// add an internal transition to show the current state at any time			
			operational.When<String>( command => command == "current" ).Effect( state => Console.WriteLine( state.XElement ) );

			// create an instance of the state machine state
			var context = new XContext();

			// initialises the state machine state (enters the region for the first time, causing transition from the initial PseudoState)
			model.Initialise( context );

			// main event loop
			while( !model.IsComplete( context ) ) {
				// write a prompt
				Console.Write( "alamo> " );

				// process lines read from the console
				if( !model.Evaluate( Console.ReadLine(), context ) )
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