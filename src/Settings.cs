using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Steelbreeze.StateMachines {
	/// <summary>
	/// Class used to hold the settings for the state library.
	/// </summary>
	public static class Settings {
		/// <summary>
		/// Flag used to control the raising of completion events after internal transitions.
		/// </summary>
		/// <remarks>Defaults to false.</remarks>
		public static bool InternalTransitionsTriggerCompletion { get; set; }
	}
}
