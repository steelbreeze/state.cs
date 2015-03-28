/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Diagnostics;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// A Transition describes a valid path from one Vertex in a state machine to another, and the trigger that will cause it to be followed.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	/// <remarks>
	/// There are two types of transition, completion transitions and message based transitions.
	/// Completion transitions are evaluated when a vertex has been entered and is deemed to be complete; this is the default for newly created transitions.
	/// Message based transitions have an additional guard condition that a message (event) and the current state machine instance will be evaluated against; this is defined by the Transition.When method thereby turning a completion transition into a message based transition.
	/// </remarks>
	public class Transition<TInstance> where TInstance : class, IActiveStateConfiguration<TInstance> {
		#region Static members
		internal static Func<Object, TInstance, Boolean> IsElse = (message, instance) => false;
		#endregion
		/// <summary>
		/// The source vertex of the transition.
		/// </summary>
		public readonly Vertex<TInstance> Source;

		/// <summary>
		/// The target vertex of the transition. 
		/// </summary>
		/// <remarks>The target may be null for internal transitions.</remarks>
		public readonly Vertex<TInstance> Target;

		/// <summary>
		/// The predicate that must evaluate true before a transition may be traversed.
		/// </summary>
		public Func<Object, TInstance, Boolean> Predicate;

		/// <summary>
		/// The complete bootstrapped behaviour performed when traversing a transition.
		/// </summary>
		internal Action<Object, TInstance, Boolean> Traverse;

		/// <summary>
		/// The user defined behaviour for transition traversal.
		/// </summary>
		internal Action<Object, TInstance> effect;

		/// <summary>
		/// Creates a new instance of the Transition class
		/// </summary>
		/// <param name="source">The source vertex</param>
		/// <param name="target">The target vertex</param>
		public Transition (Vertex<TInstance> source, Vertex<TInstance> target = null) {
			Trace.Assert (source != null, "Transitions must have a source Vertex");

			this.Source = source;
			this.Target = target;

			this.Completion ();
		}

		/// <summary>
		/// Adds a typed guard condition to a transition that can evaluate both the state machine instance and the triggering message.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that can trigger the transition.</typeparam>
		/// <param name="guard">The guard condition taking both the state machine instance and the message that must evaluate true for the transition to be traversed.</param>
		/// <returns>Returns the transition.</returns>
		public Transition<TInstance> When<TMessage> (Func<TMessage, TInstance, Boolean> guard) where TMessage : class {
			this.Predicate = (message, instance) => message is TMessage && guard (message as TMessage, instance);

			return this;
		}

		/// <summary>
		/// Adds a typed guard condition to a transition that can evaluate the triggering message.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that can trigger the transition.</typeparam>
		/// <param name="guard">A guard condition taking the message that must evaluate true for the transition to be traversed.</param>
		/// <returns>Returns the transition.</returns>
		public Transition<TInstance> When<TMessage> (Func<TMessage, Boolean> guard) where TMessage : class {
			this.Predicate = (message, instance) => message is TMessage && guard (message as TMessage);

			return this;
		}

		/// <summary>
		/// Adds a guard condition to a transition that can evaluate the triggering message.
		/// </summary>
		/// <param name="guard">A guard condition, taking just the state machine instance, that must evaluate true for the transition to be traversed.</param>
		/// <returns>Returns the transition.</returns>
		public Transition<TInstance> When (Func<TInstance, Boolean> guard) {
			this.Predicate = (message, instance) => guard (instance);

			return this;
		}

		/// <summary>
		/// Turns a transition into a completion transition
		/// </summary>
		/// <returns>Returns the transition.</returns>
		/// <remarks>
		/// All transitions are completion tansitions when initially created; this method can be used to return a transition to be a completion transition if prior calls to When or Else have been made.
		/// </remarks>
		public Transition<TInstance> Completion () {
			this.Predicate = (message, instance) => message == this.Source;

			return this;
		}

		/// <summary>
		/// Creates an else transition for use with Choice and Junction PseudoStates.
		/// </summary>
		/// <returns>Returns the transition.</returns>
		public Transition<TInstance> Else () {
			Trace.Assert (this.Source is PseudoState<TInstance> && ((this.Source as PseudoState<TInstance>).Kind == PseudoStateKind.Choice || (this.Source as PseudoState<TInstance>).Kind == PseudoStateKind.Junction), "Else is only allowed for transitions from Choice or Junction PseudoStates");

			this.Predicate = Transition<TInstance>.IsElse;

			return this;
		}

		/// <summary>
		/// Adds behavior to a transition.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that triggered the transition.</typeparam>
		/// <param name="behavior">An Action that takes the state machine instance and triggering messages as parameters.</param>
		/// <returns>Returns the transition.</returns>
		/// <remarks>
		/// If the type of the message that triggers the transition does not match TMessage, the behavior will not be called.
		/// </remarks>
		public Transition<TInstance> Effect<TMessage> (params Action<TMessage, TInstance>[] behavior) where TMessage : class {
			foreach (var effect in behavior)
				this.effect += (message, instance) => { if (message is TMessage) effect (message as TMessage, instance); };

			this.Source.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Adds behavior to a transition.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that triggered the transition.</typeparam>
		/// <param name="behavior">An Action that takes the triggering message as a parameter.</param>
		/// <returns>Returns the transition.</returns>
		/// <remarks>
		/// If the type of the message that triggers the transition does not match TMessage, the behavior will not be called.
		/// </remarks>
		public Transition<TInstance> Effect<TMessage> (params Action<TMessage>[] behavior) where TMessage : class {
			foreach (var effect in behavior)
				this.effect += (message, instance) => { if (message is TMessage) effect (message as TMessage); };

			this.Source.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Adds behavior to a transition.
		/// </summary>
		/// <param name="behavior">An Action that takes the state machine instance as a parameter.</param>
		/// <returns>Returns the transition.</returns>
		public Transition<TInstance> Effect (params Action<TInstance>[] behavior) {
			foreach (var effect in behavior)
				this.effect += (message, instance) => effect (instance);

			this.Source.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Adds behavior to a transition.
		/// </summary>
		/// <param name="behavior">An Action that takes no parameters.</param>
		/// <returns>Returns the transition.</returns>
		public Transition<TInstance> Effect (params Action[] behavior) {
			foreach (var effect in behavior)
				this.effect += (message, instance) => effect ();

			this.Source.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Invokes the transition behavior upon traversing a transition.
		/// </summary>
		/// <param name="message">The message that triggered the state transition.</param>
		/// <param name="instance">The state machine instance.</param>
		/// <param name="history">A flag denoting if history semantics were in play during the transition.</param>
		/// <remarks>
		/// For completion transitions, the message is the source vertex that was completed.
		/// </remarks>
		public void OnEffect (Object message, TInstance instance, Boolean history) { // TODO: sort out protection models 
			this.effect (message, instance);
		}

		/// <summary>
		/// Accepts a visitor
		/// </summary>
		/// <param name="visitor">The visitor to visit.</param>
		/// <param name="param">A parameter passed to the visitor when visiting the transition.</param>
		/// <remarks>
		/// A visitor will walk the state machine model from this element to all child elements including transitions calling the approritate visit method on the visitor.
		/// </remarks>
		public void Accept<TParam> (Visitor<TInstance, TParam> visitor, TParam param) {
			visitor.VisitTransition (this, param);
		}
	}
}