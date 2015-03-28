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
	/// A state models a situation during which some (usually implicit) invariant condition holds.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	/// <remarks>
	/// The invariant may represent a static situation such as an object waiting for some external event to occur.
	/// </remarks>
	public class State<TInstance> : Vertex<TInstance> where TInstance : class, IActiveStateConfiguration<TInstance> {
		/// <summary>
		/// The child Regions where the State is composite.
		/// </summary>
		private readonly HashSet<Region<TInstance>> regions = new HashSet<Region<TInstance>>();

		/// <summary>
		/// The behaviour to execute when exiting the state.
		/// </summary>
		internal Action<Object, TInstance> exit;

		/// <summary>
		/// The behaviour to execute when entering the state.
		/// </summary>
		internal Action<Object, TInstance> entry;

		/// <summary>
		/// Creates a new instance of the State class.
		/// </summary>
		/// <param name="name">The name of the state.</param>
		/// <param name="parent">The parent Region.</param>
		public State (String name, Region<TInstance> parent)
			: base (name, parent) {
			Trace.Assert (name != null, "States must have a name");
		}

		/// <summary>
		/// The child Regions where the State is composite.
		/// </summary>
		public IEnumerable<Region<TInstance>> Regions {
			get {
				return this.regions;
			}
		}

		/// <summary>
		/// True if the State is a simple State.
		/// </summary>
		/// <remarks>
		/// A simple State is one that has no child Regions.
		/// </remarks>
		public Boolean IsSimple {
			get {
				return this.regions == null || this.regions.Count == 0;
			}
		}

		/// <summary>
		/// True if the State is a composite State.
		/// </summary>
		/// <remarks>
		/// A composite State is one that has one or more child Regions.
		/// </remarks>
		public Boolean IsComposite {
			get {
				return this.regions != null && this.regions.Count > 0;
			}
		}

		/// <summary>
		/// True if the State is an orthogonal State.
		/// </summary>
		/// <remarks>
		/// A composite State is one that has more than one child Regions.
		/// </remarks>
		public Boolean IsOrthogonal {
			get {
				return this.regions != null && this.regions.Count > 1;
			}
		}

		/// <summary>
		/// Test the state to see if it is a final state
		/// </summary>
		/// <remarks>Final states are oned that have no outbound transitions.</remarks>
		public Boolean IsFinal {
			get {
				return this.Transitions.Count () == 0;
			}
		}


		/// <summary>
		/// Tests the state to determine if it is part of the current active state confuguration
		/// </summary>
		/// <param name="instance">The state machine instance.</param>
		/// <returns>True if the element is active.</returns>
		public override Boolean IsActive (TInstance instance) {
			return this.Parent.IsActive (instance) && instance[ this.Region ] == this;
		}

		/// <summary>
		/// Sets optional exit behavior that is called when leaving the State.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that triggered the state transition.</typeparam>
		/// <param name="behavior">One or more actions that take both the state machine instance and the triggering message as parameters.</param>
		/// <returns>Returns the State itself.</returns>
		/// <remarks>If the type of the triggering message does not match TMessage the behavior will not be called.</remarks>
		public State<TInstance> Exit<TMessage> (params Action<TMessage, TInstance>[] behavior) where TMessage : class {
			foreach (var exit in behavior)
				this.exit += (message, instance) => { if (message is TMessage) exit (message as TMessage, instance); };

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Sets optional exit behavior that is called when leaving the State.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that triggered the state transition.</typeparam>
		/// <param name="behavior">One or more actions that takes the triggering message as a parameter.</param>
		/// <returns></returns>
		/// <remarks>If the type of the triggering message does not match TMessage the behavior will not be called.</remarks>
		public State<TInstance> Exit<TMessage> (params Action<TMessage>[] behavior) where TMessage : class {
			foreach (var exit in behavior)
				this.exit += (message, instance) => { if (message is TMessage) exit (message as TMessage); };

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Sets optional exit behabiour that is called when leaving the State.
		/// </summary>
		/// <param name="behavior">One or more actions that takes the state machine instance as a parameter.</param>
		/// <returns>Returns the State itself.</returns>
		public State<TInstance> Exit (params Action<TInstance>[] behavior) {
			foreach (var exit in behavior)
				this.exit += (message, instance) => exit (instance);

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Sets optional exit behabiour that is called when leaving the State.
		/// </summary>
		/// <param name="behavior">One or more actions that take no parameters.</param>
		/// <returns>Returns the State itself.</returns>
		public State<TInstance> Exit (params Action[] behavior) {
			foreach (var exit in behavior)
				this.exit += (message, instance) => exit ();

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Sets optional entry behavior that is called when entering the State.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that triggered the state transition.</typeparam>
		/// <param name="behavior">One or more actions that take both the state machine instance and the triggering message as parameters.</param>
		/// <returns>Returns the State itself.</returns>
		/// <remarks>If the type of the triggering message does not match TMessage the behavior will not be called.</remarks>
		public State<TInstance> Entry<TMessage> (params Action<TMessage, TInstance>[] behavior) where TMessage : class {
			foreach (var entry in behavior)
				this.entry += (message, instance) => { if (message is TMessage) entry (message as TMessage, instance); };

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Sets optional entry behavior that is called when entering the State.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that triggered the state transition.</typeparam>
		/// <param name="behavior">One or more actions that takes the triggering message as a parameter.</param>
		/// <returns>Returns the State itself.</returns>
		/// <remarks>If the type of the triggering message does not match TMessage the behavior will not be called.</remarks>
		public State<TInstance> Entry<TMessage> (params Action<TMessage>[] behavior) where TMessage : class {
			foreach (var entry in behavior)
				this.entry += (message, instance) => { if (message is TMessage) entry (message as TMessage); };

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Sets optional entry behavior that is called when entering the State.
		/// </summary>
		/// <param name="behavior">One or more actions that takes the state machine instance as a parameter.</param>
		/// <returns>Returns the State itself.</returns>
		public State<TInstance> Entry (params Action<TInstance>[] behavior) {
			foreach (var entry in behavior)
				this.entry += (message, instance) => entry (instance);

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Sets optional entry behavior that is called when entering the State.
		/// </summary>
		/// <param name="behavior">One or more actions that takes no parameters.</param>
		/// <returns>Returns the State itself.</returns>
		public State<TInstance> Entry (params Action[] behavior) {
			foreach (var entry in behavior)
				this.entry += (message, instance) => entry ();

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Creates an internal transition.
		/// </summary>
		/// <typeparam name="TMessage">The type of the messaage that the internal transition will react to.</typeparam>
		/// <param name="guard">The guard condition that must be met for hte transition to be traversed.</param>
		/// <returns>The internal transition.</returns>
		/// <remarks>
		/// An internal transition does not exit or enter any states, however the transitions effect will be invoked if the guard condition of the transition is met
		/// </remarks>
		public Transition<TInstance> When<TMessage> (Func<TMessage, TInstance, Boolean> guard) where TMessage : class {
			return this.To (null).When (guard);
		}

		/// <summary>
		/// Creates an internal trantiion.
		/// </summary>
		/// <typeparam name="TMessage">The type of the messaage that the internal transition will react to.</typeparam>
		/// <param name="guard">The guard condition that must be met for hte transition to be traversed.</param>
		/// <returns>The internal transition.</returns>
		public Transition<TInstance> When<TMessage> (Func<TMessage, Boolean> guard) where TMessage : class {
			return this.To (null).When (guard);
		}

		/// <summary>
		/// Invokes the Exit behavior upon exiting a State.
		/// </summary>
		/// <param name="instance">The state machine instance.</param>
		/// <param name="message">The message that triggered the state transition.</param>
		/// <param name="history">A flag denoting if history semantics were in play during the transition.</param>
		public virtual void OnExit (Object message, TInstance instance, Boolean history) {
			this.exit (message, instance);
		}

		/// <summary>
		/// Invokes the Entry behavior upon entering a State.
		/// </summary>
		/// <param name="instance">The state machine instance.</param>
		/// <param name="message">The message that triggered the state transition.</param>
		/// <param name="history">A flag denoting if history semantics were in play during the transition.</param>
		public virtual void OnEntry (Object message, TInstance instance, Boolean history) {
			this.entry (message, instance);
		}

		/// <summary>
		/// Tests the element to determine if it is deemed to be complete.
		/// </summary>
		/// <param name="instance">The state machine instance.</param>
		/// <returns>True if the state is complete</returns>
		/// <remarks>Simple states are always deemed to be complete; composite states are complete if all child regions are complete.</remarks>
		public override Boolean IsComplete (TInstance instance) {
			return this.IsSimple || this.regions.All (region => region.IsComplete (instance));
		}

		/// <summary>
		/// Adds a new child region to the state.
		/// </summary>
		/// <param name="region">The child region to add to the state.</param>
		/// <remarks>Adding regions to a state turns the state in to a composite state.</remarks>
		internal virtual void Add (Region<TInstance> region) {
			this.regions.Add (region);

			region.Root.Clean = false;
		}

		/// <summary>
		/// Algorithm for selecting the transitions
		/// </summary>
		/// <param name="message">The message that may trigger a transition.</param>
		/// <param name="instance">The state machine instance.</param>
		/// <returns>Returns a single transition whose predicate evaluates true for the message and state machine instance, or null.</returns>
		/// <exception cref="System.ArgumentNullException">If more than one transitions predicate evaluates true the model is deemed to be malformed.</exception>
		protected internal override Transition<TInstance> Select (object message, TInstance instance) {
			return this.Transitions.SingleOrDefault( t => t.Predicate( message, instance ) );
		}

		/// <summary>
		/// Evaluates a state to determine if a transition should be traversed.
		/// </summary>
		/// <param name="message">The message that may cause a state transition.</param>
		/// <param name="instance">The state machine instance.</param>
		/// <returns>True if a state transition occured.</returns>
		internal override Boolean Evaluate (Object message, TInstance instance) {
			var processed = false;

			foreach (var region in this.regions)
				if (this.IsActive (instance) == true)
					if (region.Evaluate (message, instance))
						processed = true;

			if (!processed)
				processed = base.Evaluate (message, instance);

			if (processed == true && message != this)
				this.Completion (this, instance, false);

			return processed;
		}

		/// <summary>
		/// Accepts a visitor
		/// </summary>
		/// <param name="visitor">The visitor to visit.</param>
		/// <param name="param">A parameter passed to the visitor when visiting elements.</param>
		/// <remarks>
		/// A visitor will walk the state machine model from this element to all child elements including transitions calling the approritate visit method on the visitor.
		/// </remarks>
		public override void Accept<TParam> (Visitor<TInstance, TParam> visitor, TParam param) {
			visitor.VisitState (this, param);
		}
	}
}