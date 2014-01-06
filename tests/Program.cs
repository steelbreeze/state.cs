// Alamo Project
// Copyright © 2013 Steelbreeze, all rights reserved
// All code contained herein is provided to you 'AS IS' without warrantees of any kind.
using System;
using System.Diagnostics;

namespace Steelbreeze.Behavior.Tests
{
	public class Program
	{
		public static void Main( String[] args )
		{
			Trace.WriteLine( "Running unit tests for Steelbreeze.Alamo.StateMachines" );

			// TODO: deep history tests
			// TODO: action tests
			// TODO: other tests...
			// TODO: internal tests
			// TODO: local tests

			Transitions.Completions.Test();
			Transitions.External.Test();
			History.Shallow.Test();
		}
	}
}