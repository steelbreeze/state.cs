/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
namespace Steelbreeze.StateMachines.Model {
	/// <summary>
	/// Base class for visitors.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	public abstract class Visitor<TInstance> where TInstance : IInstance<TInstance> {
		/// <summary>
		/// Visits an element within a state machine model.
		/// </summary>
		/// <param name="element">The element to visit.</param>
		public virtual void VisitElement (NamedElement element) { }

		/// <summary>
		/// Visits a region within a state machine model.
		/// </summary>
		/// <param name="region">The region to visit.</param>
		/// <remarks>Prior to visiting the region, it is visited as the element type.</remarks>
		public virtual void VisitRegion (Region<TInstance> region) {
			this.VisitElement(region);

			foreach (var vertex in region.Vertices) {
				vertex.Accept(this);
			}
		}

		/// <summary>
		/// Visits a vertex within a state machine model.
		/// </summary>
		/// <param name="vertex">The vertex to visit.</param>
		/// <remarks>Prior to visiting the vertex, it is visited as the element type.</remarks>
		public virtual void VisitVertex (Vertex<TInstance> vertex) {
			this.VisitElement(vertex);

			foreach (var transition in vertex.Outgoing) {
				transition.Accept(this);
			}
		}

		/// <summary>
		/// Visits a pseudo state within a state machine model.
		/// </summary>
		/// <param name="pseudoState">The pseudo state to visit.</param>
		/// <remarks>Prior to visiting the pseudo state, it is visited as the vertex type.</remarks>
		public virtual void VisitPseudoState (PseudoState<TInstance> pseudoState) {
			this.VisitVertex(pseudoState);
		}

		/// <summary>
		/// Visits a state within a state machine model.
		/// </summary>
		/// <param name="state">The pseudo state to visit.</param>
		/// <remarks>Prior to visiting the state, it is visited as the vertex type.</remarks>
		public virtual void VisitState (State<TInstance> state) {
			this.VisitVertex(state);

			foreach (var region in state.Regions) {
				region.Accept(this);
			}
		}

		/// <summary>
		/// Visits a final state within a state machine model.
		/// </summary>
		/// <param name="finalState">The final state to visit.</param>
		/// <remarks>Prior to visiting the final state, it is visited as the state type.</remarks>
		public virtual void VisitFinalState (FinalState<TInstance> finalState) {
			this.VisitState(finalState);
		}

		/// <summary>
		/// Visits a state machine within a state machine model.
		/// </summary>
		/// <param name="stateMachine">The state machine to visit.</param>
		/// <remarks>Prior to visiting the state machine, it is visited as the state type.</remarks>
		public virtual void VisitStateMachine (StateMachine<TInstance> stateMachine) {
			this.VisitState(stateMachine);
		}

		/// <summary>
		/// Visits a transition within a state machine model.
		/// </summary>
		/// <param name="transition">The transition to visit.</param>
		public virtual void VisitTransition (Transition<TInstance> transition) { }
	}

	/// <summary>
	/// Base class for visitors.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	/// <typeparam name="TArg">The type of an argument to pass on to the elements when visiting.</typeparam>
	public abstract class Visitor<TInstance, TArg> where TInstance : IInstance<TInstance> {
		/// <summary>
		/// Visits an element within a state machine model.
		/// </summary>
		/// <param name="element">The element to visit.</param>
		/// <param name="arg">The argument passed to the visitor.</param>
		public virtual void VisitElement (NamedElement element, TArg arg) { }

		/// <summary>
		/// Visits a region within a state machine model.
		/// </summary>
		/// <param name="region">The region to visit.</param>
		/// <param name="arg">The argument passed to the visitor.</param>
		/// <remarks>Prior to visiting the region, it is visited as the element type.</remarks>
		public virtual void VisitRegion (Region<TInstance> region, TArg arg) {
			this.VisitElement(region, arg);

			foreach (var vertex in region.Vertices) {
				vertex.Accept(this, arg);
			}
		}

		/// <summary>
		/// Visits a vertex within a state machine model.
		/// </summary>
		/// <param name="vertex">The vertex to visit.</param>
		/// <param name="arg">The argument passed to the visitor.</param>
		/// <remarks>Prior to visiting the vertex, it is visited as the element type.</remarks>
		public virtual void VisitVertex (Vertex<TInstance> vertex, TArg arg) {
			this.VisitElement(vertex, arg);

			foreach (var transition in vertex.Outgoing) {
				transition.Accept(this, arg);
			}
		}

		/// <summary>
		/// Visits a pseudo state within a state machine model.
		/// </summary>
		/// <param name="pseudoState">The pseudo state to visit.</param>
		/// <param name="arg">The argument passed to the visitor.</param>
		/// <remarks>Prior to visiting the pseudo state, it is visited as the vertex type.</remarks>
		public virtual void VisitPseudoState (PseudoState<TInstance> pseudoState, TArg arg) {
			this.VisitVertex(pseudoState, arg);
		}

		/// <summary>
		/// Visits a state within a state machine model.
		/// </summary>
		/// <param name="state">The pseudo state to visit.</param>
		/// <param name="arg">The argument passed to the visitor.</param>
		/// <remarks>Prior to visiting the state, it is visited as the vertex type.</remarks>
		public virtual void VisitState (State<TInstance> state, TArg arg) {
			this.VisitVertex(state, arg);

			foreach (var region in state.Regions) {
				region.Accept(this, arg);
			}
		}

		/// <summary>
		/// Visits a final state within a state machine model.
		/// </summary>
		/// <param name="finalState">The final state to visit.</param>
		/// <param name="arg">The argument passed to the visitor.</param>
		/// <remarks>Prior to visiting the final state, it is visited as the state type.</remarks>
		public virtual void VisitFinalState (FinalState<TInstance> finalState, TArg arg) {
			this.VisitState(finalState, arg);
		}

		/// <summary>
		/// Visits a state machine within a state machine model.
		/// </summary>
		/// <param name="stateMachine">The state machine to visit.</param>
		/// <param name="arg">The argument passed to the visitor.</param>
		/// <remarks>Prior to visiting the state machine, it is visited as the state type.</remarks>
		public virtual void VisitStateMachine (StateMachine<TInstance> stateMachine, TArg arg) {
			this.VisitState(stateMachine, arg);
		}

		/// <summary>
		/// Visits a transition within a state machine model.
		/// </summary>
		/// <param name="transition">The transition to visit.</param>
		/// <param name="arg">The argument passed to the visitor.</param>
		public virtual void VisitTransition (Transition<TInstance> transition, TArg arg) { }
	}
}