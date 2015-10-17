/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Threading;
using Steelbreeze.StateMachines.Model;

namespace Steelbreeze.StateMachines.Examples {
	public class Player : StateMachineInstance<Player> {
		private long count;
		private System.Timers.Timer counter;
		public long Count { get { return Interlocked.Read(ref count); } }

		public Player () {
			this.counter = new System.Timers.Timer(1000);
			this.counter.Elapsed += (s, e) => Interlocked.Increment(ref count);

			this.ResetCounter();
		}

		public void EngageHead () {
			Console.WriteLine("- engaging head");
		}

		public void DisengageHead () {
			Console.WriteLine("- disengaging head");
		}

		public void StartMotor () {
			Console.WriteLine("- starting motor");

			counter.Start();
		}

		public void StopMotor () {
			Console.WriteLine("- stopping motor");

			counter.Stop();
		}

		public void ResetCounter (long value = 0) {
			Interlocked.Exchange(ref count, value);
		}
	}
}