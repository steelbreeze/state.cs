/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Diagnostics;

namespace Steelbreeze.Behavior.StateMachines.Tests
{
	public class Program
	{
		public static void Main( String[] args )
		{
			Trace.WriteLine( "Running unit tests for Steelbreeze.Alamo.StateMachines" );

			// TODO: add more transition tests (incl. else)
			Transitions.Completions.Test();
			History.Shallow.Test();

			// user test cases
			Users.Muximise1.Test();
			Users.Brice1.Test();
			Users.P3pp3r.Test();
		}
	}
}