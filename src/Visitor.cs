/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// Implementation of a visitor pattern for the state machine.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	/// <typeparam name="TParam">The type of the parameter to pass through the visitor.</typeparam>
	public class Visitor<TInstance, TParam> where TInstance : class, IActiveStateConfiguration<TInstance> {

		/// <summary>
		/// Visits an element within a state machine model.
		/// </summary>
		/// <param name="element">The element to visit.</param>
		/// <param name="param">The parameter passed to the visitor.</param>
		public virtual void VisitElement (Element<TInstance> element, TParam param) {
		}

		/// <summary>
		/// Visits a region within a state machine model.
		/// </summary>
		/// <param name="region">The region to visit</param>
		/// <param name="param">The parameter passed to the visitor.</param>
		public virtual void VisitRegion (Region<TInstance> region, TParam param) {
			this.VisitElement (region, param);

			foreach (var vertex in region.Vertices)
				vertex.Accept (this, param);
		}

		/// <summary>
		/// Visits a vertex within a state machine model.
		/// </summary>
		/// <param name="vertex">The vertex to visit.</param>
		/// <param name="param">The parameter passed to the visitor.</param>
		public virtual void VisitVertex (Vertex<TInstance> vertex, TParam param) {
			this.VisitElement (vertex, param);
		}

		/// <summary>
		/// Visits a pseudo state within a state machine model.
		/// </summary>
		/// <param name="pseudoState">The pseudostate to visit.</param>
		/// <param name="param">The parameter passed to the visitor.</param>
		public virtual void VisitPseudoState (PseudoState<TInstance> pseudoState, TParam param) {
			this.VisitVertex (pseudoState, param);
		}

		/// <summary>
		/// Visits a state within a state machine model.
		/// </summary>
		/// <param name="state">The state to visit.</param>
		/// <param name="param">The parameter passed to the visitor.</param>
		public virtual void VisitState (State<TInstance> state, TParam param) {
			this.VisitVertex (state, param);

			foreach (var region in state.Regions)
				region.Accept (this, param);
		}

		/// <summary>
		/// Visits a final state within a state machine model.
		/// </summary>
		/// <param name="finalState">The final state to visit.</param>
		/// <param name="param">The parameter passed to the visitor.</param>
		public virtual void VisitFinalState (FinalState<TInstance> finalState, TParam param) {
			this.VisitState (finalState, param);
		}

		/// <summary>
		/// Visits the root state machine within a state machine model.
		/// </summary>
		/// <param name="stateMachine">The state machine to visit.</param>
		/// <param name="param">The parameter passed to the visitor.</param>
		public virtual void VisitStateMachine (StateMachine<TInstance> stateMachine, TParam param) {
			this.VisitState (stateMachine, param);

			this.VisitTransition (stateMachine as Vertex<TInstance>, param);
		}

		private void VisitTransition (Region<TInstance> region, TParam param) {
			foreach (var vertex in region.Vertices)
				this.VisitTransition (vertex, param);
		}

		private void VisitTransition (Vertex<TInstance> vertex, TParam param) {
			foreach (var transition in vertex.Transitions)
				this.VisitTransition (transition, param);

			if (vertex is State<TInstance>) {
				var state = vertex as State<TInstance>;

				if (state.IsComposite)
					foreach (var region in state.Regions)
						this.VisitTransition (region, param);
			}
		}

		/// <summary>
		/// Visits a transition within a state machine model.
		/// </summary>
		/// <param name="transition">The transition to visit.</param>
		/// <param name="param">A parameter passed to the visitor when visiting elements.</param>
		public virtual void VisitTransition (Transition<TInstance> transition, TParam param) {
		}
	}
}