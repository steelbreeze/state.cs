/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
using System;
using System.Linq;
using Steelbreeze.StateMachines.Model;

namespace Steelbreeze.StateMachines.Tools {
	internal class Validator<TInstance> : Visitor<TInstance> where TInstance : IInstance<TInstance> {
		override public void VisitPseudoState (PseudoState<TInstance> pseudoState) {
			base.VisitPseudoState(pseudoState);

			if (pseudoState.Kind == PseudoStateKind.Choice || pseudoState.Kind == PseudoStateKind.Junction) {
				// [7] In a complete statemachine, a junction vertex must have at least one incoming and one outgoing transition.
				// [8] In a complete statemachine, a choice vertex must have at least one incoming and one outgoing transition.
				if (pseudoState.Outgoing.Count == 0) {
					Console.Error.WriteLine(pseudoState + ": " + pseudoState.Kind + " pseudo states must have at least one outgoing transition.");
				}

				// choice and junction pseudo state can have at most one else transition
				if (pseudoState.Outgoing.Where(transition => transition.guard == Transition<TInstance>.FalseGuard).Count() > 1) {
					Console.Error.WriteLine(pseudoState + ": " + pseudoState.Kind + " pseudo states cannot have more than one Else transitions.");
				}
			} else {
				// non choice/junction pseudo state may not have else transitions
				if (pseudoState.Outgoing.Where(transition => transition.guard == Transition<TInstance>.FalseGuard).Count() != 0) {
					Console.Error.WriteLine(pseudoState + ": " + pseudoState.Kind + " pseudo states cannot have Else transitions.");
				}

				if (pseudoState.IsInitial) {
					if (pseudoState.Outgoing.Count != 1) {
						// [1] An initial vertex can have at most one outgoing transition.
						// [2] History vertices can have at most one outgoing transition.
						Console.Error.WriteLine(pseudoState + ": initial pseudo states must have one outgoing transition.");
					} else {
						// [9] The outgoing transition from an initial vertex may have a behavior, but not a trigger or guard.
						if (pseudoState.Outgoing.Single().guard != Transition<TInstance>.TrueGuard) {
							Console.Error.WriteLine(pseudoState + ": initial pseudo states cannot have a guard condition.");
						}
					}
				}
			}
		}

		override public void VisitRegion (Region<TInstance> region) {
			base.VisitRegion(region);

			// [1] A region can have at most one initial vertex.
			// [2] A region can have at most one deep history vertex.
			// [3] A region can have at most one shallow history vertex.
			if (region.Vertices.OfType<PseudoState<TInstance>>().Where(pseudoState => pseudoState.IsInitial).Count() > 1) {
				Console.Error.WriteLine(region + ": regions may have at most one initial pseudo state.");
			}
		}

		override public void VisitState (State<TInstance> state) {
			base.VisitState(state);

			if (state.Regions.Where(region => region.Name == Region<TInstance>.DefaultName).Count() > 1) {
				Console.Error.WriteLine(state + ": a state cannot have more than one region named " + Region<TInstance>.DefaultName);
			}
		}

		override public void VisitFinalState (FinalState<TInstance> finalState) {
			base.VisitFinalState(finalState);

			// [1] A final state cannot have any outgoing transitions.
			if (finalState.Outgoing.Count != 0) {
				Console.Error.WriteLine(finalState + ": final states must not have outgoing transitions.");
			}

			// [2] A final state cannot have regions.
			if (finalState.Regions.Count != 0) {
				Console.Error.WriteLine(finalState + ": final states must not have child regions.");
			}

			// [4] A final state has no entry behavior.
			if (finalState.entryBehavior != null) {
				Console.Error.WriteLine(finalState + ": final states may not have entry behavior.");
			}

			// [5] A final state has no exit behavior.
			if (finalState.exitBehavior != null) {
				Console.Error.WriteLine(finalState + ": final states may not have exit behavior.");
			}
		}

		override public void VisitTransition (Transition<TInstance> transition) {
			base.VisitTransition(transition);

			// Local transition target vertices must be a child of the source vertex
			if (transition.Kind == TransitionKind.Local) {
				if (transition.Target.Ancestry().IndexOf(transition.Source) == -1) {
					Console.Error.WriteLine(transition + ": local transition target vertices must be a child of the source composite sate.");
				}
			}
		}
	}
}