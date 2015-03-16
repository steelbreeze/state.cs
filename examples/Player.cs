/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Xml.Linq;

namespace Steelbreeze.Behavior.StateMachines.Examples {
	/// <summary>
	/// A controller for a simple cassette player
	/// </summary>
	/// <remarks>
	/// The player now inherits from a Region, so can be used in isolation, or as a region in a larger device.
	/// The player now implements an 'operational' composite state so the off command can be used while in any sub-state.
	/// </remarks>
	public class Player {
		public static StateMachine<StateMachineInstance> Model;

		static Player () {
			// create the state machine model
			Model = new StateMachine<StateMachineInstance> ("model");

			// create the vertices within the model
			var initial = Model.CreatePseudoState ("initial", PseudoStateKind.Initial);
			var operational = Model.CreateState ("operational");
			var choice = Model.CreatePseudoState ("choice", PseudoStateKind.Choice);
			var final = Model.CreateFinalState ("final");
			var history = operational.CreatePseudoState ("history", PseudoStateKind.DeepHistory);
			var stopped = operational.CreateState ("stopped");
			var active = operational.CreateState ("active").Entry (EngageHead).Exit (DisengageHead);
			var running = active.CreateState ("running").Entry (StartMotor).Exit (StopMotor);
			var paused = active.CreateState ("paused");

			// create the transitions between vertices of the model
			initial.To (operational).Effect (DisengageHead, StartMotor);
			history.To (stopped);
			stopped.To (running).When<String> (command => command == "play");
			active.To (stopped).When<String> (command => command == "stop");
			running.To (paused).When<String> (command => command == "pause");
			paused.To (running).When<String> (command => command == "play");
			operational.To (final).When<String> (command => command == "off");
			operational.To (choice).When<String> (command => command == "rand");
			choice.To (operational).Effect (() => Console.WriteLine ("- transition A back to operational"));
			choice.To (operational).Effect (() => Console.WriteLine ("- transition B back to operational"));
		}

		static void Main () {
			// create an instance of the state machine state
			var instance = new StateMachineInstance ("player");

			// initialises the state machine state (enters the region for the first time, causing transition from the initial PseudoState)
			Model.Initialise (instance);

			// main event loop
			while (!Model.IsComplete (instance)) {
				// write a prompt
				Console.Write ("alamo> ");

				// process lines read from the console
				if (!Model.Evaluate (Console.ReadLine (), instance))
					Console.WriteLine ("unknown command");
			}

			Console.WriteLine ("Press any key to quit");
			Console.ReadKey ();
		}

		private static void EngageHead () {
			Console.WriteLine ("- engaging head");
		}

		private static void DisengageHead () {
			Console.WriteLine ("- disengaging head");
		}

		private static void StartMotor () {
			Console.WriteLine ("- starting motor");
		}

		private static void StopMotor () {
			Console.WriteLine ("- stopping motor");
		}
	}
}