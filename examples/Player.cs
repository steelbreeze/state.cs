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
		private long count = 0;
		private System.Timers.Timer counter = new System.Timers.Timer(1000);

		/// <summary>
		/// The play counter; increments in seconds while the player is running.
		/// </summary>
		public long Count { get { return Interlocked.Read(ref count); } }

		/// <summary>
		/// Creates a new instance of the player class.
		/// </summary>
		public Player () {
			this.counter.Elapsed += (s, e) => Interlocked.Increment(ref count);
		}

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

			counter.Start();
		}

		/// <summary>
		/// Stops the players motor.
		/// </summary>
		public void StopMotor () {
			Console.WriteLine("- stopping motor");

			counter.Stop();
		}

		/// <summary>
		/// Resets the play counter.
		/// </summary>
		/// <param name="value"></param>
		public void ResetCounter (long value = 0) {
			Interlocked.Exchange(ref count, value);
		}
	}
}