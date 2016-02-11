/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Threading;
using Steelbreeze.StateMachines.Model;

namespace Steelbreeze.StateMachines.Examples {
	/// <summary>
	/// Represents a cassette machine with a few primitive operations.
	/// </summary>
	public class Player : StateMachineInstance<Player> {
		/// <summary>
		/// Play counter for the player
		/// </summary>
		public long Count { get; set; }

		/// <summary>
		/// Engages the players read head.
		/// </summary>
		public void EngageHead () {
			Console.WriteLine("- engaging head");
		}

		/// <summary>
		/// Disengages the players read head.
		/// </summary>
		public void DisengageHead () {
			Console.WriteLine("- disengaging head");
		}

		/// <summary>
		/// Starts the players motor.
		/// </summary>
		public void StartMotor () {
			Console.WriteLine("- starting motor");
		}

		/// <summary>
		/// Stops the players motor.
		/// </summary>
		public void StopMotor () {
			Console.WriteLine("- stopping motor");
		}
	}
}