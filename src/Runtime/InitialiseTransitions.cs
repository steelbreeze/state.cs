/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
using System;
using Steelbreeze.StateMachines.Model;

namespace Steelbreeze.StateMachines.Runtime {
	// determine the type of transition and use the appropriate initiliasition method
	internal class InitialiseTransitions<TInstance> : Visitor<TInstance, Func<NamedElement, ElementBehavior<TInstance>>> where TInstance : IInstance<TInstance> {

		public override void VisitTransition (Transition<TInstance> transition, Func<NamedElement, ElementBehavior<TInstance>> behaviour) {
			transition.onTraverse = null;

			if (transition.Kind == TransitionKind.Internal) {
				VisitInternalTransition(transition, behaviour);
			} else if (transition.Kind == TransitionKind.Local) {
				this.VisitLocalTransition(transition, behaviour);
			} else {
				this.VisitExternalTransition(transition, behaviour);
			}
		}

		public void VisitInternalTransition (Transition<TInstance> transition, Func<NamedElement, ElementBehavior<TInstance>> behaviour) {
			transition.onTraverse += transition.transitionBehavior;

			// add a test for completion
			if( Settings.InternalTransitionsTriggerCompletion) {
				transition.onTraverse += (message, instance, history) => {
					var state = transition.Target as State<TInstance>;

					// fire a completion transition as required
					if (state.IsComplete(instance)) {
						state.EvaluateState(instance, state);
					}
				};
			}
		}

		// initialise internal transitions: these do not leave the source state
		public void VisitLocalTransition (Transition<TInstance> transition, Func<NamedElement, ElementBehavior<TInstance>> behaviour) {
			transition.onTraverse += (message, instance, history) => {
				var targetAncestors = transition.Target.Ancestry();
				var i = 0;

				// find the first inactive element in the target ancestry
				while (targetAncestors[ i ].IsActive(instance)) {
					++i;
				}

				// exit the active sibling
				behaviour(instance.GetCurrent(targetAncestors[ i ].Region)).leave(message, instance, false);

				// perform the transition action;
				if (transition.transitionBehavior != null) {
					transition.transitionBehavior(message, instance, false);
				}

				// enter the target ancestry
				while (i < targetAncestors.Count) {
					this.cascadeElementEntry(transition, behaviour, targetAncestors[ i++ ], i < targetAncestors.Count ? targetAncestors[ i ] : null, behavior => behavior(message, instance, false));
				}

				// trigger cascade
				var endEnter = behaviour(transition.Target).endEnter;

				if (endEnter != null) {
					endEnter(message, instance, false);
				}
			};
		}

		// initialise external transitions: these are abritarily complex
		public void VisitExternalTransition (Transition<TInstance> transition, Func<NamedElement, ElementBehavior<TInstance>> behaviour) {
			var sourceAncestors = transition.Source.Ancestry();
			var targetAncestors = transition.Target.Ancestry();
			var i = Math.Min(sourceAncestors.Count, targetAncestors.Count) - 1;

			// find the index of the first uncommon ancestor (or for external transitions, the source)
			while (sourceAncestors[ i - 1 ] != targetAncestors[ i - 1 ]) { --i; }

			// leave source ancestry as required
			transition.onTraverse += behaviour(sourceAncestors[ i ]).leave;

			// perform the transition effect
			transition.onTraverse += transition.transitionBehavior;

			// enter the target ancestry
			while (i < targetAncestors.Count) {
				this.cascadeElementEntry(transition, behaviour, targetAncestors[ i++ ], i < targetAncestors.Count ? targetAncestors[ i ] : null, behavior => transition.onTraverse += behavior);
			}

			// trigger cascade
			transition.onTraverse += behaviour(transition.Target).endEnter;
		}

		private void cascadeElementEntry (Transition<TInstance> transition, Func<NamedElement, ElementBehavior<TInstance>> behaviour, Vertex<TInstance> element, Vertex<TInstance> next, Action<Behavior<TInstance>> task) {
			task(behaviour(element).beginEnter);

			if (next != null && element is State<TInstance>) {
				var state = element as State<TInstance>;

				foreach (var region in state.Regions) {
					task(behaviour(region).beginEnter);

					if (region != next.Region) {
						task(behaviour(region).endEnter);
					}
				}
			}
		}
	}
}