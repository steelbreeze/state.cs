/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Steelbreeze.StateMachines.Model;

namespace Steelbreeze.StateMachines.Runtime {
	// bootstraps all the elements within a state machine model
	internal class InitialiseElements<TInstance> : Visitor<TInstance, bool> where TInstance : IInstance<TInstance> {
		private Dictionary<NamedElement, ElementBehavior<TInstance>> behaviours = new Dictionary<NamedElement, ElementBehavior<TInstance>>();

		private ElementBehavior<TInstance> Behaviour (NamedElement element) {
			var result = default(ElementBehavior<TInstance>);

			if (this.behaviours.TryGetValue(element, out result) == false) {
				this.behaviours.Add(element, result = new ElementBehavior<TInstance>());
			}

			return result;
		}

		public override void VisitElement (NamedElement element, bool deepHistoryAbove) {
			this.Behaviour(element).leave += (message, instance, history) => Console.WriteLine(instance + " leave " + element);
			this.Behaviour(element).beginEnter += (message, instance, history) => Console.WriteLine(instance + " enter " + element);
		}

		public override void VisitRegion (Region<TInstance> region, bool deepHistoryAbove) {
			var regionInitial = region.Vertices.OfType<PseudoState<TInstance>>().Where(pseudoState => pseudoState.IsInitial).SingleOrDefault();

			foreach (var vertex in region.Vertices) {
				vertex.Accept(this, deepHistoryAbove || (regionInitial != null && regionInitial.Kind == PseudoStateKind.DeepHistory));
			}

			// leave the curent active child state when exiting the region
			this.Behaviour(region).leave += (message, instance, history) => this.Behaviour(instance.GetCurrent(region)).leave(message, instance, history);

			// enter the appropriate child vertex when entering the region
			if (deepHistoryAbove || regionInitial != null || regionInitial.IsHistory) { // NOTE: history needs to be determined at runtime
				this.Behaviour(region).endEnter += (message, instance, history) => (this.Behaviour((history || regionInitial.IsHistory) ? (Vertex<TInstance>)instance.GetCurrent(region) ?? regionInitial : regionInitial)).Enter(message, instance, history || regionInitial.Kind == PseudoStateKind.DeepHistory);
			} else {
				this.Behaviour(region).endEnter += this.Behaviour(regionInitial).Enter;
			}

			this.VisitElement(region, deepHistoryAbove);
		}

		public override void VisitPseudoState (PseudoState<TInstance> pseudoState, bool deepHistoryAbove) {
			base.VisitPseudoState(pseudoState, deepHistoryAbove);

			// evaluate comppletion transitions once vertex entry is complete
			if (pseudoState.IsInitial) {
				this.Behaviour(pseudoState).endEnter += (message, instance, history) => pseudoState.Outgoing.Single().Traverse(instance, null);
			} else if (pseudoState.Kind == PseudoStateKind.Terminate) {
				// terminate the state machine instance upon transition to a terminate pseudo state
				this.Behaviour(pseudoState).beginEnter += (message, instance, history) => instance.IsTerminated = true;
			}
		}

		public override void VisitState (State<TInstance> state, bool deepHistoryAbove) {
			// NOTE: manually iterate over the child regions to control the sequence of behaviour
			foreach (var region in state.Regions) {
				region.Accept(this, deepHistoryAbove);

				this.Behaviour(state).leave += this.Behaviour(region).leave;
				this.Behaviour(state).endEnter += this.Behaviour(region).Enter;
			}

			this.VisitVertex(state, deepHistoryAbove);

			// add the user defined behaviour when entering and exiting states
			this.Behaviour(state).leave += state.exitBehavior;
			this.Behaviour(state).beginEnter += state.entryBehavior;

			// update the parent regions current state
			this.Behaviour(state).beginEnter += (message, instance, history) => {
				if (state.Region != null) {
					instance.SetCurrent(state.Region, state);
				}
			};
		}

		public override void VisitStateMachine (StateMachine<TInstance> stateMachine, bool deepHistoryAbove) {
			base.VisitStateMachine(stateMachine, deepHistoryAbove);

			// initiaise all the transitions once all the elements have been initialised
			stateMachine.Accept(new InitialiseTransitions<TInstance>(), this.Behaviour);

			// define the behaviour for initialising a state machine instance
			stateMachine.OnInitialise = this.Behaviour(stateMachine).Enter;
		}
	}
}