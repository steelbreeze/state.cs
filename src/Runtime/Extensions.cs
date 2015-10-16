/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
using System;
using System.Linq;
using Steelbreeze.StateMachines.Model;

namespace Steelbreeze.StateMachines.Runtime {
	public static class Extensions {
		private static Random random = new Random();
		public static Func<int, int> RandomSelector { get; set; }

		static Extensions () {
			RandomSelector = (maxValue) => random.Next(maxValue);
		}

		public static void Initialise<TInstance> (this StateMachine<TInstance> model, TInstance instance, Boolean autoInitialiseModel = true) where TInstance : IInstance<TInstance> {
			// initialise the state machine model if necessary
			if (model.Clean == false) {
				model.Initialise();
			}

			// log as required
			Console.WriteLine("initialise " + instance);

			// enter the state machine instance for the first time
			model.OnInitialise(null, instance, false);
		}

		public static void Initialise<TInstance> (this StateMachine<TInstance> model) where TInstance : IInstance<TInstance> {
			// log as required
			Console.WriteLine("initialise " + model);

			// initialise the state machine model
			model.Accept(new InitialiseElements<TInstance>(), false);
			model.Clean = true;
		}

		public static bool IsComplete<TInstance> (this Region<TInstance> region, TInstance instance) where TInstance : IInstance<TInstance> {
			return instance.GetCurrent(region).IsFinal;
		}

		public static bool IsComplete<TInstance> (this State<TInstance> state, TInstance instance) where TInstance : IInstance<TInstance> {
			return state.Regions.All(region => IsComplete(region, instance));
		}

		public static bool IsActive<TInstance> (this Vertex<TInstance> state, TInstance instance) where TInstance : IInstance<TInstance> {
			return state.Region != null ? (IsActive(state.Region.State, instance) && (instance.GetCurrent(state.Region) == state)) : true;
		}

		public static bool Evaluate<TInstance> (this StateMachine<TInstance> model, TInstance instance, object message, bool autoInitialiseModel = true) where TInstance : IInstance<TInstance> {
			// initialise the state machine model if necessary
			if (autoInitialiseModel && model.Clean == false) {
				model.Initialise();
			}

			// terminated state machine instances will not evaluate messages
			if (instance.IsTerminated) {
				return false;
			}

			return model.EvaluateState(instance, message);
		}

		// evaluates messages against a state, executing transitions as appropriate
		internal static bool EvaluateState<TInstance> (this State<TInstance> state, TInstance instance, object message) where TInstance : IInstance<TInstance> {
			var result = false;

			// delegate to child regions first
			foreach (var region in state.Regions) {
				if (EvaluateState(instance.GetCurrent(region), instance, message)) {
					result = true;

					if (state.IsActive(instance) == false) {
						break;
					}
				}
			}

			// if a transition occured in a child region, check for completions
			if (result == true) {
				if ((message != state) && state.IsComplete(instance)) {
					EvaluateState(state, instance, state);
				}
			} else {
				// otherwise look for a transition from this state
				var transitions = state.Outgoing.Where(transition => transition.guard(message, instance));

				if (transitions.Count() == 1) {
					// execute if a single transition was found
					result = transitions.Single().Traverse(instance, message);
				} else if (transitions.Count() > 1) {
					// error if multiple transitions evaluated true
					Console.Error.WriteLine(state + ": multiple outbound transitions evaluated true for message " + message);
				}
			}

			return result;
		}

		// traverses a transition
		internal static bool Traverse<TInstance> (this Transition<TInstance> transition, TInstance instance, object message) where TInstance : IInstance<TInstance> {
			var onTraverse = transition.onTraverse;
//			var target = transition.Target;

			// process static conditional branches
			while (transition.Target != null && transition.Target is PseudoState<TInstance>) {
				var pseudoState = transition.Target as PseudoState<TInstance>;

				if (pseudoState.Kind != PseudoStateKind.Junction) {
					break;
				}

				transition = pseudoState.SelectTransition(instance, message);

				// concatenate behaviour before and after junctions
				onTraverse += transition.onTraverse;
			}

			// execute the transition behaviour
			onTraverse(message, instance, false);

			// process dynamic conditional branches
			if (transition.Target != null) {
				if (transition.Target is PseudoState<TInstance>) {
					var pseudoState = transition.Target as PseudoState<TInstance>;

					if (pseudoState.Kind == PseudoStateKind.Choice) {
						Traverse(pseudoState.SelectTransition(instance, message), instance, message);
					}
				} else if (transition.Target is State<TInstance>) {
					var state = transition.Target as State<TInstance>;

					// test for completion transitions
					if (state.IsComplete(instance)) {
						state.EvaluateState(instance, state);
					}
				}
			}

			return true;
		}

		// select next leg of composite transitions after choice and junction pseudo states
		internal static Transition<TInstance> SelectTransition<TInstance> (this PseudoState<TInstance> pseudoState, TInstance instance, object message) where TInstance : IInstance<TInstance> {
			var results = pseudoState.Outgoing.Where(transition => transition.guard(message, instance));

			if (pseudoState.Kind == PseudoStateKind.Choice) {
				return results.Count() != 0 ? results.ElementAt(Extensions.RandomSelector(results.Count())) : pseudoState.FindElse();
			} else {
				if (results.Count() > 1) {
					throw new Exception("Multiple outbound transition guards returned true at " + pseudoState + " for " + message);
				} else {
					return results.SingleOrDefault() ?? pseudoState.FindElse();
				}
			}
		}

		// look for else transitins from a junction or choice
		internal static Transition<TInstance> FindElse<TInstance> (this PseudoState<TInstance> pseudoState) where TInstance : IInstance<TInstance> {
			return pseudoState.Outgoing.Where(transition => transition.guard == Transition<TInstance>.FalseGuard).SingleOrDefault();
		}
	}
}