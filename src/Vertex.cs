/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// A Vertex is an abstraction of a node in a state machine graph; it can be the source or destination of any number of transitions.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	public abstract class Vertex<TInstance> : Element<TInstance> where TInstance : IActiveStateConfiguration<TInstance> {
		internal readonly Region<TInstance> Region;
		internal Boolean IsFinal { get { return this.transitions == null; } }

		private Transition<TInstance>[] transitions; // trading off model building performance for runtime performance
		private readonly Func<Transition<TInstance>[], Object, TInstance, Transition<TInstance>> selector;

		/// <summary>
		/// Returns the Vertex's parent element.
		/// </summary>
		public override Element<TInstance> Parent { get { return this.Region; } }

		internal Vertex (String name, Region<TInstance> parent, Func<Transition<TInstance>[], Object, TInstance, Transition<TInstance>> selector)
			: base (name, parent) {
			this.Region = parent;
			this.selector = selector;

			if( parent != null )
				parent.Add (this);
		}

		/// <summary>
		/// Tests the vertex to determine if it is part of the current active state confuguration
		/// </summary>
		/// <param name="instance">The state machine instance.</param>
		/// <returns>True if the element is active.</returns>
		internal protected override Boolean IsActive (IActiveStateConfiguration<TInstance> instance) {
			return this.Parent.IsActive (instance);
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

			if (this.transitions == null)
				this.transitions = new Transition<TInstance>[ 1 ] { transition };
			else {
				var transitions = new Transition<TInstance>[ this.transitions.Length + 1 ];

				this.transitions.CopyTo (transitions, 0);

				transitions[ this.transitions.Length ] = transition;

				this.transitions = transitions;
			}

			this.Root.Clean = false;

			return transition;
		}

		internal override void BootstrapElement (Boolean deepHistoryAbove) {
			base.BootstrapElement (deepHistoryAbove);

			this.EndEnter += this.EvaluateCompletions;
			this.Enter = this.BeginEnter + this.EndEnter;
		}

		internal override void BootstrapTransitions () {
			if (this.transitions != null)
				foreach (var transition in this.transitions)
					transition.BootstrapTransitions ();
		}

		internal void EvaluateCompletions (Object message, TInstance instance, Boolean history) {
			if (this.IsComplete (instance))
				this.Evaluate (this, instance);
		}

		public virtual Boolean IsComplete (TInstance instance) {
			return true;
		}

		internal virtual Boolean Evaluate (Object message, TInstance instance) {
			var transition = this.selector (this.transitions, message, instance);

			if (transition == null)
				return false;

			transition.Traverse (message, instance, false);

			return true;
		}
	}
}