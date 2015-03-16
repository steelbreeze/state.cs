/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Diagnostics;

namespace Steelbreeze.Behavior.StateMachines.Tests.Users {
	public class Brice1 {
		public static void Test () {
			var model = new StateMachine<StateMachineInstance> ("model");

			var initial1 = model.CreatePseudoState ("initial");
			var myComposite1 = model.CreateState ("myComposite1");
			var myComposite2 = model.CreateState ("myComposite2");

			var initial2 = myComposite1.CreatePseudoState ("initial");
			var myState1 = myComposite1.CreateState ("myState1");
			var myState2 = myComposite1.CreateState ("myState2");

			initial1.To (myComposite1);
			initial2.To (myState1);

			myComposite1.To (myComposite2).When<String> (message => message == "A").Effect (() => System.Diagnostics.Trace.Fail ("Outer transition fired"));
			myState1.To (myState2).When<String> (message => message == "A");

			var instance = new StateMachineInstance ("brice1");

			model.Initialise (instance);

			model.Evaluate ("A", instance);
		}
	}
}