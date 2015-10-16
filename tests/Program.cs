/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;

namespace Steelbreeze.StateMachines.Tests {
	public class Program {
		public static int Main (String[] args) {

			Brice.Run();
			Callbacks.Run();
			Choice.Run();
			Dynamic.Run();
			Else.Run();
			History.Run();
			Internal.Run();
			Local.Run();
			Muximise.Run();
			p3pp3r.Run();
			Static.Run();
			Terminate.Run();
			Transitions.Run();

			return 0;
		}
	}
}