/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Collections.Generic;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// A Vertex is an abstraction of a node in a state machine graph; it can be the source or destination of any number of transitions.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	public abstract class Vertex<TInstance> : Element<TInstance> where TInstance : class, IActiveStateConfiguration<TInstance> {
		/// <summary>
		/// The parent region that owns this Vertex.
		/// </summary>
		internal readonly Region<TInstance> Region;

		/// <summary>
		/// The outgoing transitions from the vertex.
		/// </summary>
		internal protected readonly HashSet<Transition<TInstance>> Transitions = new HashSet<Transition<TInstance>> ();

		/// <summary>
		/// Creates a new instance of a the Vertex abstract class.
		/// </summary>
		/// <param name="name">The name of the vertex.</param>
		/// <param name="region">The parent region that the new vertex is a child of.</param>
		internal protected Vertex (String name, Region<TInstance> region = null)
			: base (name, region) {
			this.Region = region;

			if (region != null)
				region.Add (this);
		}

		/// <summary>
		/// Returns the Vertex's parent element.
		/// </summary>
		public override Element<TInstance> Parent {
			get {
				return this.Region;
			}
		}

		/// <summary>
		/// Creates a new transition from this Vertex.
		/// </summary>
		/// <param name="target">The Vertex to transition to.</param>
		/// <returns>An intance of the Transition class.</returns>
		/// <remarks>
		/// To specify an internal transition, specify a null target.
		/// </remarks>
		public virtual Transition<TInstance> To (Vertex<TInstance> target) {
			var transition = new Transition<TInstance> (this, target);

			this.Transitions.Add (transition);

			this.Root.Clean = false;

			return transition;
		}

		/// <summary>
		/// Triggers completion events if the vertex is deemed to be complete.
		/// </summary>
		/// <param name="message">The origional message that caused the vertex to be entered.</param>
		/// <param name="instance">The state machine instance.</param>
		/// <param name="history">The history semantic that was in force when the vertex was entered.</param>
		internal void Completion (Object message, TInstance instance, Boolean history) {
			if (this.IsComplete (instance))
				this.Evaluate (this, instance);
		}

		/// <summary>
		/// Selects a transition for a given message and state machine instance for the vertex.
		/// </summary>
		/// <param name="message">The message that may trigger a transition.</param>
		/// <param name="instance">The state machine instance.</param>
		/// <returns>A transition if found, or null.</returns>
		internal protected abstract Transition<TInstance> Select (Object message, TInstance instance);

		/// <summary>
		/// Evaluates a message to determine if a state transition is viable.
		/// </summary>
		/// <param name="message">The message that may trigger a state transition.</param>
		/// <param name="instance">The state machine instance.</param>
		/// <returns>True if the message triggered a state transition.</returns>
		internal virtual Boolean Evaluate (Object message, TInstance instance) {
			var transition = this.Select( message, instance);

			if (transition == null)
				return false;

			transition.Traverse (message, instance, false);

			return true;
		}
	}
}