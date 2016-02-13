/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using Steelbreeze.StateMachines.Model;   // classes to create the state machine model
using Steelbreeze.StateMachines.Tools;   // extensions for model validation
using Steelbreeze.StateMachines.Runtime; // extensions for runtime

namespace Steelbreeze.StateMachines.Examples {
	/// <summary>
	/// A controller for a simple cassette player
	/// </summary>
	/// <remarks>
	/// The player 
	/// now inherits from a Region, so can be used in isolation, or as a region in a larger device.
	/// The player now implements an 'operational' composite state so the off command can be used while in any sub-state.
	/// </remarks>
	public class Program {
		static void Main () {
			// create the state machine model
			var model = new StateMachine<Player>("model");

			// create the vertices within the model
			var initial = model.CreatePseudoState("initial", PseudoStateKind.Initial);
			var operational = model.CreateState("operational");
			var choice = model.CreatePseudoState("choice", PseudoStateKind.Choice);
			var flipped = model.CreateState("flipped");
			var final = model.CreateFinalState("final");

			var history = operational.CreatePseudoState("history", PseudoStateKind.DeepHistory);
			var stopped = operational.CreateState("stopped");
			var active = operational.CreateState("active").Entry(i => i.EngageHead()).Exit(i => i.DisengageHead());

			var running = active.CreateState("running").Entry(i => i.StartMotor()).Exit(i => i.StopMotor());
			var paused = active.CreateState("paused");

			// create the transitions between vertices of the model
			initial.To(operational).Effect(i => i.DisengageHead()).Effect(i => i.StopMotor());
			history.To(stopped);
			stopped.To(running).When<string>(command => command == "play");
			active.To(stopped).When<string>(command => command == "stop");
			running.To(paused).When<string>(command => command == "pause");
			running.To().When<string>(command => command == "tick").Effect((Player instance) => instance.Count++);
			paused.To(running).When<string>(command => command == "play");
			operational.To(final).When<string>(command => command == "off");
			operational.To(choice).When<string>(command => command == "rand");
			choice.To(operational).Effect(() => Console.WriteLine("- transition A back to operational"));
			choice.To(operational).Effect(() => Console.WriteLine("- transition B back to operational"));
			operational.To(flipped).When<string>(command => command == "flip");
			flipped.To(operational).When<string>(command => command == "flip");

			// validate the model for correctness
			model.Validate();

			// create a blocking collection make events from multiple sources thread-safe
			var queue = new System.Collections.Concurrent.BlockingCollection<Object>();

			// create an instance of the player - enqueue a tick message for the machine while its playing
			var player = new Player(() => queue.Add("tick"));

			// initialises the players initial state (enters the region for the first time, causing transition from the initial PseudoState)
			model.Initialise(player);

			// create a task to capture commands from the console in another thread
			System.Threading.Tasks.Task.Run(() => {
				string command = "";

				while (command.Trim().ToLower() != "exit") {
					queue.Add(command = Console.ReadLine());
				}

				queue.CompleteAdding();
			});

			// write the initial command prompt
			Console.Write("{0:0000}> ", player.Count);

			// process messages from the queue
			foreach (var message in queue.GetConsumingEnumerable()) {
				// process the message
				model.Evaluate(player, message);

				// manage the command prompt
				var left = Math.Max(Console.CursorLeft, 6);
				var top = Console.CursorTop;
				Console.SetCursorPosition(0, top);
				Console.Write("{0:0000}>", player.Count);
				Console.SetCursorPosition(left, top);
			}
		}
	}
}