/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Diagnostics;

namespace Steelbreeze.Behavior.StateMachines.Tests.Transitions {
	public class Activity<TInstance> : State<TInstance> where TInstance : IActiveStateConfiguration<TInstance> {
		public Activity (String name, Region<TInstance> region)
			: base (name, region) {
			this.CreatePseudoState ("initial", PseudoStateKind.Initial).To (this.CreateFinalState ("final"));
		}
	}

	public static class ActivityEx {
		public static Activity<TInstance> CreateActivity<TInstance> (this StateMachine<TInstance> stateMachine, String name) where TInstance : IActiveStateConfiguration<TInstance> {
			return new Activity<TInstance> (name, stateMachine);
		}
	}

	public class Completions {
		public static void Test () {
			try {
				var model = new StateMachine<StateMachineInstance> ("model");
				var initial = model.CreatePseudoState("initial");
				var activity1 = model.CreateState ("activity1");
				var activity2 = model.CreateActivity ("activity2");
				var junction1 = model.CreatePseudoState("junction1", PseudoStateKind.Junction);
				var junction2 = model.CreatePseudoState ("junction2", PseudoStateKind.Junction);
				var activity3 = model.CreateState ("activity3");
				var final = model.CreateFinalState ("final");

				initial.To (activity1);
				activity1.To (activity2);
				activity2.To (junction1);
				junction1.To (junction2);
				junction2.To (activity3);
				activity3.To (final);

				var instance = new StateMachineInstance ("completion");

				model.Initialise (instance);

				Trace.Assert (model.IsComplete (instance));
			} catch (Exception x) {
				Trace.Fail (x.Message);
			}
		}
	}
}