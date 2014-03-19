// Copyright © 2014 Steelbreeze Limited.
// This file is part of state.cs.
//
// state.cs is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published
// by the Free Software Foundation, either version 3 of the License,
// or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
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