/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace Steelbreeze.StateMachines.Model {
	/// <summary>
	/// Represents a state within a state machine model.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	public class State<TInstance> : Vertex<TInstance> where TInstance : IInstance<TInstance> {
		internal Behavior<TInstance> exitBehavior;
		internal Behavior<TInstance> entryBehavior;

		/// <summary>
		/// The child regions, if any, owned by this state.
		/// </summary>
		/// <remarks>A state with one or more child regions is a composite state.</remarks>
		public ICollection<Region<TInstance>> Regions = new HashSet<Region<TInstance>>();

		/// <summary>
		/// Creates a new state as a child of a region.
		/// </summary>
		/// <param name="region">The parent region.</param>
		/// <param name="name">The name of the new state.</param>
		/// <returns>The newly created state.</returns>
		public State (string name, Region<TInstance> region) : base(name, region) { }

		/// <summary>
		/// Removes the state from the state machine model.
		/// </summary>
		public override void Remove () {
			var regions = this.Regions.ToList();

			foreach (var region in regions) {
				region.Remove();
			}

			base.Remove();
		}

		/// <summary>
		/// Returns the default region for the state.
		/// </summary>
		public Region<TInstance> DefaultRegion {
			get {
				return this.Regions.SingleOrDefault(region => region.Name == Region<TInstance>.DefaultName) ?? new Region<TInstance>(Region<TInstance>.DefaultName, this);
			}
		}

		/// <summary>
		/// If any state has no outgoing transitions, it is deemed to be a final state.
		/// </summary>
		/// <remarks>When reached, a final state will cause its parent region to be deemed complete.</remarks>
		public bool IsFinal {
			get {
				return this.Outgoing.Count == 0;
			}
		}

		/// <summary>
		/// True if the state has no child regions.
		/// </summary>
		public bool IsSimple {
			get {
				return this.Regions.Count == 0;
			}
		}

		/// <summary>
		/// True if the state has one or more child regions.
		/// </summary>
		public bool IsComposite {
			get {
				return this.Regions.Count > 0;
			}
		}

		/// <summary>
		/// True if the state has more than one child regions.
		/// </summary>
		public bool IsOrthogonal {
			get {
				return this.Regions.Count > 1;
			}
		}

		/// <summary>
		/// Register a callback to be executed when the state is exited.
		/// </summary>
		/// <param name="exitAction">A parameterless action to be called when hte state is exited.</param>
		/// <returns>Returns the state; enabling a fluent style interface.</returns>
		public State<TInstance> Exit (Action exitAction) {
			this.exitBehavior += (message, instance, history) => exitAction();

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Register a callback to be executed when the state is exited.
		/// </summary>
		/// <param name="exitAction">An action that takes a single parameter of the state machine instance to be called when hte state is exited.</param>
		/// <returns>Returns the state; enabling a fluent style interface.</returns>
		public State<TInstance> Exit (Action<TInstance> exitAction) {
			this.exitBehavior += (message, instance, history) => exitAction(instance);

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Register a callback to be executed when the state is exited.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that triggered the transition.</typeparam>
		/// <param name="exitAction">An action that takes a single parameter of the message that triggered the transition to be called when hte state is exited.</param>
		/// <returns>Returns the state; enabling a fluent style interface.</returns>
		public State<TInstance> Exit<TMessage>(Action<TMessage> exitAction) where TMessage : class {
			this.exitBehavior += (message, instance, history) => { if (message is TMessage) exitAction(message as TMessage); };

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Register a callback to be executed when the state is exited.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that triggered the transition.</typeparam>
		/// <param name="exitAction">An action that takes two parameters of the message that triggered the transition and the state machine instance to be called when hte state is exited.</param>
		/// <returns>Returns the state; enabling a fluent style interface.</returns>
		public State<TInstance> Exit<TMessage>(Action<TMessage, TInstance> exitAction) where TMessage : class {
			this.exitBehavior += (message, instance, history) => { if (message is TMessage) exitAction(message as TMessage, instance); };

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Register a callback to be executed when the state is entered.
		/// </summary>
		/// <param name="entryAction">A parameterless action to be called when hte state is entered.</param>
		/// <returns>Returns the state; enabling a fluent style interface.</returns>
		public State<TInstance> Entry (Action entryAction) {
			this.entryBehavior += (message, instance, history) => entryAction();

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Register a callback to be executed when the state is entered.
		/// </summary>
		/// <param name="entryAction">An action that takes a single parameter of the state machine instance to be called when hte state is entered.</param>
		/// <returns>Returns the state; enabling a fluent style interface.</returns>
		public State<TInstance> Entry (Action<TInstance> entryAction) {
			this.entryBehavior += (message, instance, history) => entryAction(instance);

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Register a callback to be executed when the state is entered.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that triggered the transition.</typeparam>
		/// <param name="entryAction">An action that takes a single parameter of the message that triggered the transition to be called when hte state is entered.</param>
		/// <returns>Returns the state; enabling a fluent style interface.</returns>
		public State<TInstance> Entry<TMessage>(Action<TMessage> entryAction) where TMessage : class {
			this.entryBehavior += (message, instance, history) => { if (message is TMessage) entryAction(message as TMessage); };

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Register a callback to be executed when the state is entered.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that triggered the transition.</typeparam>
		/// <param name="entryAction">An action that takes two parameters of the message that triggered the transition and the state machine instance to be called when hte state is entered.</param>
		/// <returns>Returns the state; enabling a fluent style interface.</returns>
		public State<TInstance> Entry<TMessage>(Action<TMessage, TInstance> entryAction) where TMessage : class {
			this.entryBehavior += (message, instance, history) => { if (message is TMessage) entryAction(message as TMessage, instance); };

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Accepts a visitor.
		/// </summary>
		/// <param name="visitor">The visitor to accept.</param>
		public override void Accept (Visitor<TInstance> visitor) {
			visitor.VisitState(this);
		}

		/// <summary>
		/// Accepts a visitor.
		/// </summary>
		/// <typeparam name="TArg">The type of the argument passed into the visitor.</typeparam>
		/// <param name="visitor">The visitor to accept.</param>
		/// <param name="arg">The argument to pass to each element visited.</param>
		public override void Accept<TArg>(Visitor<TInstance, TArg> visitor, TArg arg) {
			visitor.VisitState(this, arg);
		}
	}
}