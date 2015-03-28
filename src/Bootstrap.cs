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
	/// Holds the behaviour for a given element used within the bootstrapping process
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	internal class ElementBehaviour<TInstance> where TInstance : class, IActiveStateConfiguration<TInstance> {
		internal Action<Object, TInstance, Boolean> Leave;
		internal Action<Object, TInstance, Boolean> BeginEnter;
		internal Action<Object, TInstance, Boolean> EndEnter;
		internal Action<Object, TInstance, Boolean> Enter;
	}

	/// <summary>
	/// Bootstraps a state machine model.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instnce.</typeparam>
	/// <remarks>Bootstrapping a state machine model pre-determines all operations and evaluations required when traversing a transition; the results are then cached within the transition.</remarks>
	internal class Bootstrap<TInstance> : Visitor<TInstance, Boolean> where TInstance : class, IActiveStateConfiguration<TInstance> {
		private static BootstrapTransitions<TInstance> bootstrapTransitions = new BootstrapTransitions<TInstance> ();

		/// <summary>
		/// Cache of the behaviour required within the state machine model.
		/// </summary>
		private Dictionary<Element<TInstance>, ElementBehaviour<TInstance>> behaviour;

		/// <summary>
		/// Returns the behaviour for a given element within the state machine model.
		/// </summary>
		/// <param name="element">The element to return the behaviour for.</param>
		/// <returns>The state machine behaviour for a given model element.</returns>
		private ElementBehaviour<TInstance> ElementBehaviour (Element<TInstance> element) {
			ElementBehaviour<TInstance> result = null;

			if (!behaviour.TryGetValue (element, out result))
				behaviour.Add (element, result = new ElementBehaviour<TInstance> ());

			return result;
		}

		public override void VisitElement (Element<TInstance> element, Boolean deepHistoryAbove) {
#if DEBUG
			ElementBehaviour (element).Leave += (message, instance, history) => Console.WriteLine ("{0} leave {1}", instance, element);
			ElementBehaviour (element).BeginEnter += (message, instance, history) => Console.WriteLine ("{0} enter {1}", instance, element);
#endif
		}

		public override void VisitRegion (Region<TInstance> region, Boolean deepHistoryAbove) {
			foreach (var vertex in region.Vertices)
				vertex.Accept (this, deepHistoryAbove || (region.Initial != null && region.Initial.Kind == PseudoStateKind.DeepHistory));

			ElementBehaviour (region).Leave += (message, instance, history) => {
				State<TInstance> current = instance[ region ];

				if (ElementBehaviour (current).Leave != null) {
					ElementBehaviour (current).Leave (message, instance, history);
				}
			};

			if (deepHistoryAbove || region.Initial == null || region.Initial.IsHistory) {
				ElementBehaviour (region).EndEnter += (message, instance, history) => {
					Vertex<TInstance> initial = region.Initial;

					if (history || region.Initial.IsHistory) {
						initial = instance[ region ];

						if (initial == null)
							initial = region.Initial;
					}

					ElementBehaviour (initial).Enter (message, instance, history || region.Initial.Kind == PseudoStateKind.DeepHistory);
				};
			} else
				ElementBehaviour (region).EndEnter += ElementBehaviour (region.Initial).Enter;

			this.VisitElement (region, deepHistoryAbove);

			ElementBehaviour (region).Enter = ElementBehaviour (region).BeginEnter + ElementBehaviour (region).EndEnter;
		}

		public override void VisitVertex (Vertex<TInstance> vertex, Boolean deepHistoryAbove) {
			this.VisitElement (vertex, deepHistoryAbove);
	
			ElementBehaviour (vertex).EndEnter += vertex.Completion;
			ElementBehaviour (vertex).Enter = ElementBehaviour (vertex).BeginEnter + ElementBehaviour (vertex).EndEnter;
		}

		public override void VisitPseudoState (PseudoState<TInstance> pseudoState, Boolean deepHistoryAbove) {
			this.VisitVertex (pseudoState, deepHistoryAbove);

			if (pseudoState.Kind == PseudoStateKind.Terminate)
				ElementBehaviour (pseudoState).Enter += (message, instance, history) => instance.IsTerminated = true;
		}

		public override void VisitState (State<TInstance> state, Boolean deepHistoryAbove) {
			foreach (var region in state.Regions) {
				region.Accept (this, deepHistoryAbove);

				ElementBehaviour (state).Leave += ElementBehaviour (region).Leave;
				ElementBehaviour (state).EndEnter += ElementBehaviour (region).Enter;
			}

			this.VisitVertex (state, deepHistoryAbove);

			if (state.exit != null)
				ElementBehaviour (state).Leave += state.OnExit;

			if (state.entry != null)
				ElementBehaviour (state).BeginEnter += state.OnEntry;

			ElementBehaviour (state).BeginEnter += (message, instance, history) => {
				if (state.Region != null)
					instance[ state.Region ] = state;
			};

			ElementBehaviour (state).Enter = ElementBehaviour (state).BeginEnter + ElementBehaviour (state).EndEnter;
		}

		public override void VisitStateMachine (StateMachine<TInstance> stateMachine, Boolean param) {
			behaviour = new Dictionary<Element<TInstance>, ElementBehaviour<TInstance>> ();

			base.VisitStateMachine (stateMachine, param);

			stateMachine.initialise = ElementBehaviour (stateMachine).Enter;

			stateMachine.Accept (bootstrapTransitions, ElementBehaviour); ;
		}
	}

	/// <summary>
	/// Bootstraps the transitions after all elements have been bootstrapped
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance</typeparam>
	internal class BootstrapTransitions<TInstance> : Visitor<TInstance, Func<Element<TInstance>, ElementBehaviour<TInstance>>> where TInstance : class, IActiveStateConfiguration<TInstance> {
		public override void VisitTransition (Transition<TInstance> transition, Func<Element<TInstance>, ElementBehaviour<TInstance>> Behaviour) {
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

					if (element is State<TInstance>) { // TODO: find a way to remove the is/as code
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