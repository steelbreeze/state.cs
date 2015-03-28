/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// Bootstraps a state machine model.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instnce.</typeparam>
	/// <remarks>Bootstrapping a state machine model pre-determines all operations and evaluations required when traversing a transition; the results are then cached within the transition.</remarks>
	internal class Bootstrap<TInstance> : Visitor<TInstance, Boolean> where TInstance : class, IActiveStateConfiguration<TInstance> {
		private class Actions {
			internal Action<Object, TInstance, Boolean> Leave;
			internal Action<Object, TInstance, Boolean> BeginEnter;
			internal Action<Object, TInstance, Boolean> EndEnter;
			internal Action<Object, TInstance, Boolean> Enter;
		}

		/// <summary>
		/// Cache of the behaviour required within the state machine model.
		/// </summary>
		private Dictionary<Element<TInstance>, Actions> behaviour;

		/// <summary>
		/// Returns the behaviour for a given element within the state machine model.
		/// </summary>
		/// <param name="element">The element to return the behaviour for.</param>
		/// <returns>The state machine behaviour for a given model element.</returns>
		private Actions Behaviour (Element<TInstance> element) {
			Actions result = null;

			if (!behaviour.TryGetValue (element, out result))
				behaviour.Add (element, result = new Actions ());

			return result;
		}

		public override void VisitElement (Element<TInstance> element, Boolean deepHistoryAbove) {
#if DEBUG
			Behaviour (element).Leave += (message, instance, history) => Console.WriteLine ("{0} leave {1}", instance, element);
			Behaviour (element).BeginEnter += (message, instance, history) => Console.WriteLine ("{0} enter {1}", instance, element);
#endif
		}

		public override void VisitRegion (Region<TInstance> region, Boolean deepHistoryAbove) {
			foreach (var vertex in region.Vertices)
				vertex.Accept (this, deepHistoryAbove || (region.Initial != null && region.Initial.Kind == PseudoStateKind.DeepHistory));

			Behaviour (region).Leave += (message, instance, history) => {
				State<TInstance> current = instance[ region ];

				if (Behaviour (current).Leave != null) {
					Behaviour (current).Leave (message, instance, history);
				}
			};

			if (deepHistoryAbove || region.Initial == null || region.Initial.IsHistory) {
				Behaviour (region).EndEnter += (message, instance, history) => {
					Vertex<TInstance> initial = region.Initial;

					if (history || region.Initial.IsHistory) {
						initial = instance[ region ];

						if (initial == null)
							initial = region.Initial;
					}

					Behaviour (initial).Enter (message, instance, history || region.Initial.Kind == PseudoStateKind.DeepHistory);
				};
			} else
				Behaviour (region).EndEnter += Behaviour (region.Initial).Enter;

			this.VisitElement (region, deepHistoryAbove);

			Behaviour (region).Enter = Behaviour (region).BeginEnter + Behaviour (region).EndEnter;
		}

		public override void VisitVertex (Vertex<TInstance> vertex, Boolean deepHistoryAbove) {
			this.VisitElement (vertex, deepHistoryAbove);
	
			Behaviour (vertex).EndEnter += vertex.Completion;
			Behaviour (vertex).Enter = Behaviour (vertex).BeginEnter + Behaviour (vertex).EndEnter;
		}

		public override void VisitPseudoState (PseudoState<TInstance> pseudoState, Boolean deepHistoryAbove) {
			this.VisitVertex (pseudoState, deepHistoryAbove);

			if (pseudoState.Kind == PseudoStateKind.Terminate)
				Behaviour (pseudoState).Enter += (message, instance, history) => instance.IsTerminated = true;
		}

		public override void VisitState (State<TInstance> state, Boolean deepHistoryAbove) {
			foreach (var region in state.Regions) {
				region.Accept (this, deepHistoryAbove);

				Behaviour (state).Leave += Behaviour (region).Leave;
				Behaviour (state).EndEnter += Behaviour (region).Enter;
			}

			this.VisitVertex (state, deepHistoryAbove);

			if (state.exit != null)
				Behaviour (state).Leave += state.OnExit;

			if (state.entry != null)
				Behaviour (state).BeginEnter += state.OnEntry;

			Behaviour (state).BeginEnter += (message, instance, history) => {
				if (state.Region != null)
					instance[ state.Region ] = state;
			};

			Behaviour (state).Enter = Behaviour (state).BeginEnter + Behaviour (state).EndEnter;
		}

		public override void VisitStateMachine (StateMachine<TInstance> stateMachine, bool param) {
			behaviour = new Dictionary<Element<TInstance>, Actions> ();

			base.VisitStateMachine (stateMachine, param);

			stateMachine.initialise = Behaviour (stateMachine).Enter;
		}

		public override void VisitTransition (Transition<TInstance> transition, Boolean deepHistoryAbove) {
			// reset the traverse operation to cater for re-initialisation
			transition.Traverse = null;

			// internal transitions
			if (transition.Target == null) {
				// just perform the transition effect; no actual transition
				if (transition.effect != null)
					transition.Traverse += transition.OnEffect;

				// local transitions
			} else if (transition.Target.Region == transition.Source.Region) {
				// leave the source
				transition.Traverse += Behaviour (transition.Source).Leave;

				// perform the transition effect
				if (transition.effect != null)
					transition.Traverse += transition.OnEffect;

				// enter the target
				transition.Traverse += Behaviour (transition.Target).Enter;

				// complex (external) transitions
			} else {
				var sourceAncestors = transition.Source.Ancestors;
				var targetAncestors = transition.Target.Ancestors;
				var sourceAncestorsCount = sourceAncestors.Count ();
				var targetAncestorsCount = targetAncestors.Count ();
				int i = 0, l = Math.Min (sourceAncestorsCount, sourceAncestorsCount);

				// find the index of the first uncommon ancestor
				while ((i < l) && sourceAncestors.ElementAt (i) == targetAncestors.ElementAt (i)) ++i;

				// validation rule (not in the UML spec currently)
				Trace.Assert (sourceAncestors.ElementAt (i) is Region<TInstance> == false, "Transitions may not cross sibling orthogonal regions");

				// leave the first uncommon ancestor
				transition.Traverse = Behaviour (i < sourceAncestorsCount ? sourceAncestors.ElementAt (i) : transition.Source).Leave;

				// perform the transition effect
				if (transition.effect != null)
					transition.Traverse += transition.OnEffect;

				// edge case when transitioning to a state in the vertex ancestry
				if (i >= targetAncestorsCount)
					transition.Traverse += Behaviour (transition.Target).BeginEnter;

				// enter the target ancestry
				while (i < targetAncestorsCount) {
					var element = targetAncestors.ElementAt (i++);
					var next = i < targetAncestorsCount ? targetAncestors.ElementAt (i) : null;

					transition.Traverse += Behaviour (element).BeginEnter;

					if (element is State<TInstance>) {
						var state = element as State<TInstance>;

						if (state.IsOrthogonal) {
							foreach (var region in state.Regions) {
								if (region != next) {
									transition.Traverse += Behaviour (region).Enter;
								}
							}
						}
					}
				}

				// trigger cascade
				transition.Traverse += Behaviour (transition.Target).EndEnter;
			}
		}
	}
}