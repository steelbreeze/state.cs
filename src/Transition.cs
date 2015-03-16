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
	/// A Transition describes a valid path from one Vertex in a state machine to another, and the trigger that will cause it to be followed.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	/// <remarks>
	/// There are two types of transition, completion transitions and message based transitions.
	/// Completion transitions are evaluated when a vertex has been entered and is deemed to be complete; this is the default for newly created transitions.
	/// Message based transitions have an additional guard condition that a message (event) and the current state machine instance will be evaluated against; this is defined by the Transition.When method thereby turning a completion transition into a message based transition.
	/// </remarks>
	public class Transition<TInstance> where TInstance : IActiveStateConfiguration<TInstance> {
		#region Static members
		private static Func<Object, TInstance, Boolean> IsElse = (message, instance) => false;

		internal static Func<Transition<TInstance>[], Object, TInstance, Transition<TInstance>> PseudoState (PseudoStateKind kind) {
			switch (kind) {
				case PseudoStateKind.Initial:
				case PseudoStateKind.DeepHistory:
				case PseudoStateKind.ShallowHistory:
					return Transition<TInstance>.Initial;

				case PseudoStateKind.Junction:
					return Transition<TInstance>.Junction;

				case PseudoStateKind.Choice:
					return Transition<TInstance>.Choice;

				case PseudoStateKind.Terminate:
					return Transition<TInstance>.Terminate;

				default: // NOTE: all PseudoStateKinds dealt with above so should not be an issue
					return null;
			}
		}

		internal static Transition<TInstance> State (Transition<TInstance>[] transitions, Object message, TInstance instance) {
			Transition<TInstance> result = null;

			if (transitions != null) {
				for (int i = 0, l = transitions.Length; i < l; ++i) {
					if (transitions[ i ].Predicate (message, instance)) {
						if (result != null)
							throw new InvalidOperationException ("Multiple outbound transitions evaluated true");

						result = transitions[ i ];
					}
				}
			}

			return result;
		}

		private static Transition<TInstance> Initial (Transition<TInstance>[] transitions, Object message, TInstance instance) {
			if (transitions.Length == 1)
				return transitions[ 0 ];
			else
				throw new InvalidOperationException ("Initial transition must have a single outbound transition");
		}

		private static Transition<TInstance> Junction (Transition<TInstance>[] transitions, Object message, TInstance instance) {
			return transitions.SingleOrDefault (t => t.Predicate (message, instance)) ?? transitions.Single (transition => transition.Predicate.Equals (Transition<TInstance>.IsElse));
		}

		private static readonly Random random = new Random ();

		private static Transition<TInstance> Choice (Transition<TInstance>[] transitions, Object message, TInstance instance) {
			var transition = default (Transition<TInstance>);
			var items = transitions.Where (t => t.Predicate (message, instance));
			var count = items.Count ();

			if (count == 1)
				transition = items.First ();

			else if (count > 1)
				transition = items.ElementAt (random.Next (count));

			return transition ?? transitions.Single (t => t.Predicate.Equals (Transition<TInstance>.IsElse));
		}

		internal static Transition<TInstance> Terminate (Transition<TInstance>[] transitions, Object message, TInstance instance) {
			return null;
		}
		#endregion
		internal readonly Vertex<TInstance> Source;
		internal readonly Vertex<TInstance> Target;
		internal Func<Object, TInstance, Boolean> Predicate;
		internal Action<Object, TInstance, Boolean> Traverse;

		private event Action<Object, TInstance> effect;

		internal Transition (Vertex<TInstance> source, Vertex<TInstance> target) {
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
		protected void OnEffect (Object message, TInstance instance, Boolean history) {
			this.effect (message, instance);
		}

		internal void BootstrapTransitions () {
			// reset the traverse operation to cater for re-initialisation
			this.Traverse = null;

			// internal transitions
			if (this.Target == null) {
				// just perform the transition effect; no actual transition
				if (this.effect != null)
					this.Traverse += this.OnEffect;

				// local transitions
			} else if (this.Target.Region == this.Source.Region) {
				// leave the source
				this.Traverse += this.Source.Leave;

				// perform the transition effect
				if (this.effect != null)
					this.Traverse += this.OnEffect;

				// enter the target
				this.Traverse += this.Target.Enter;

				// complex (external) transitions
			} else {
				var sourceAncestors = this.Source.Ancestors;
				var targetAncestors = this.Target.Ancestors;
				var sourceAncestorsCount = sourceAncestors.Count ();
				var targetAncestorsCount = targetAncestors.Count ();
				int i = 0, l = Math.Min (sourceAncestorsCount, sourceAncestorsCount);

				// find the index of the first uncommon ancestor
				while ((i < l) && sourceAncestors.ElementAt (i) == targetAncestors.ElementAt (i)) ++i;

				// validation rule (not in the UML spec currently)
				Trace.Assert (sourceAncestors.ElementAt (i) is Region<TInstance> == false, "Transitions may not cross sibling orthogonal regions");

				// leave the first uncommon ancestor
				this.Traverse = (i < sourceAncestorsCount ? sourceAncestors.ElementAt (i) : this.Source).Leave;

				// perform the transition effect
				if (this.effect != null)
					this.Traverse += this.OnEffect;

				// edge case when transitioning to a state in the vertex ancestry
				if (i >= targetAncestorsCount)
					this.Traverse += this.Target.BeginEnter;

				// enter the target ancestry
				while (i < targetAncestorsCount)
					targetAncestors.ElementAt (i++).BootstrapEnter (ref this.Traverse, i < targetAncestorsCount ? targetAncestors.ElementAt (i) : null);

				// trigger cascade
				this.Traverse += this.Target.EndEnter;
			}
		}
	}
}