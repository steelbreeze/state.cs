/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
using System;

namespace Steelbreeze.StateMachines.Model {
	/// <summary>
	/// Represents a valid transition between vertices within a state machine model.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	public class Transition<TInstance> where TInstance : IInstance<TInstance> {
		static internal Guard TrueGuard = (message, instance) => { return true; };
		static internal Guard FalseGuard = (message, instance) => { return false; };
		internal delegate bool Guard (object message, TInstance instance);
		internal Guard guard;
		internal Behavior<TInstance> transitionBehavior;
		internal Behavior<TInstance> onTraverse;

		/// <summary>
		/// The source of the transition.
		/// </summary>
		public readonly Vertex<TInstance> Source;

		/// <summary>
		/// The target of the transition.
		/// </summary>
		/// <remarks>Internal transitions (within the source state) do not have a target defined; they do not enter or exit the state but only execute the transition behavior if traversed.</remarks>
		public readonly Vertex<TInstance> Target;

		/// <summary>
		/// The kind of the transition defining its use and behavior.
		/// </summary>
		public readonly TransitionKind Kind;

		/// <summary>
		/// Creates a new instance of the Transition class.
		/// </summary>
		/// <param name="source">The source of the transition.</param>
		/// <param name="target">The target of the transition</param>
		/// <param name="kind">The kind of the transition defining its behavior. Note that this may be overriden for to Internal if no target vertex is supplied.</param>
		public Transition (Vertex<TInstance> source, Vertex<TInstance> target = null, TransitionKind kind = TransitionKind.External) {
			this.Source = source;
			this.Target = target;
			this.Kind = target != null ? kind : TransitionKind.Internal;
			this.guard = (source is PseudoState<TInstance>) ? Transition<TInstance>.TrueGuard : (message, instance) => message == this.Source;

			this.Source.Outgoing.Add(this);

			this.Source.Root.Clean = false;
		}

		/// <summary>
		/// Turns the transition into an 'else' transition.
		/// </summary>
		/// <returns>Returns the transition; enabling a fluent style interface.</returns>
		/// <remarks>Else transitions are used as the transition of last resort at choice and junction pseudo states if no other transition can be selected.</remarks>
		public Transition<TInstance> Else () {
			this.guard = Transition<TInstance>.FalseGuard;

			return this;
		}

		/// <summary>
		/// Places a guard condition on a transition to limit the conditions under which it is traversed.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that is to be evaluated.</typeparam>
		/// <param name="guard">The guard condition callback taking the message and state machine instance.</param>
		/// <returns>Returns the transition; enabling a fluent style interface.</returns>
		public Transition<TInstance> When<TMessage> (Func<TMessage, TInstance, bool> guard) where TMessage : class {
			this.guard = (message, instance) => { return message is TMessage && guard(message as TMessage, instance); };

			return this;
		}

		/// <summary>
		/// Places a guard condition on a transition to limit the conditions under which it is traversed.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that is to be evaluated.</typeparam>
		/// <param name="guard">The guard condition callback taking the message only.</param>
		/// <returns>Returns the transition; enabling a fluent style interface.</returns>
		public Transition<TInstance> When<TMessage> (Func<TMessage, bool> guard) where TMessage : class {
			this.guard = (message, instance) => { return message is TMessage && guard(message as TMessage); };

			return this;
		}

		/// <summary>
		/// Places a guard condition on a transition to limit the conditions under which it is traversed.
		/// </summary>
		/// <param name="guard">The guard condition callback taking the state machine instance only.</param>
		/// <returns>Returns the transition; enabling a fluent style interface.</returns>
		public Transition<TInstance> When (Func<TInstance, bool> guard) {
			this.guard = (message, instance) => { return guard(instance); };

			return this;
		}

		/// <summary>
		/// Register a callback to be executed when the transition is traversed. 
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that is to be evaluated.</typeparam>
		/// <param name="transitionAction">An action that takes two parameters of the message that triggered the transition and the state machine instance to be called when the transition is traversed.</param>
		/// <returns>Returns the transition; enabling a fluent style interface.</returns>
		/// <remarks>The callbacks are invoked after the source ancestry is exited and before the target ancestry is entered.</remarks>
		public Transition<TInstance> Effect<TMessage> (Action<TMessage, TInstance> transitionAction) where TMessage : class {
			this.transitionBehavior += (message, instance, history) => { if (message is TMessage) transitionAction(message as TMessage, instance); };

			this.Source.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Register a callback to be executed when the transition is traversed. 
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that is to be evaluated.</typeparam>
		/// <param name="transitionAction">An action that takes a single parameter of the message that triggered the transition when the transition is traversed.</param>
		/// <returns>Returns the transition; enabling a fluent style interface.</returns>
		/// <remarks>The callbacks are invoked after the source ancestry is exited and before the target ancestry is entered.</remarks>
		public Transition<TInstance> Effect<TMessage> (Action<TMessage> transitionAction) where TMessage : class {
			this.transitionBehavior += (message, instance, history) => { if (message is TMessage) transitionAction(message as TMessage); };

			this.Source.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Register a callback to be executed when the transition is traversed. 
		/// </summary>
		/// <param name="transitionAction">An action that takes a single parameter of the state machine instance to be called when the transition is traversed.</param>
		/// <returns>Returns the transition; enabling a fluent style interface.</returns>
		/// <remarks>The callbacks are invoked after the source ancestry is exited and before the target ancestry is entered.</remarks>
		public Transition<TInstance> Effect (Action<TInstance> transitionAction) {
			this.transitionBehavior += (message, instance, history) => { transitionAction(instance); };

			this.Source.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Register a callback to be executed when the transition is traversed. 
		/// </summary>
		/// <param name="transitionAction">An action that takes no parameters to be called when the transition is traversed.</param>
		/// <returns>Returns the transition; enabling a fluent style interface.</returns>
		/// <remarks>The callbacks are invoked after the source ancestry is exited and before the target ancestry is entered.</remarks>
		public Transition<TInstance> Effect (Action transitionAction) {
			this.transitionBehavior += (message, instance, history) => { transitionAction(); };

			this.Source.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Accepts a visitor.
		/// </summary>
		/// <param name="visitor">The visitor to accept.</param>
		public void Accept(Visitor<TInstance> visitor)
        {
            visitor.VisitTransition(this);
        }

		/// <summary>
		/// Accepts a visitor.
		/// </summary>
		/// <typeparam name="TArg">The type of the argument passed into the visitor.</typeparam>
		/// <param name="visitor">The visitor to accept.</param>
		/// <param name="arg">The argument to pass to each element visited.</param>
		public void Accept<TArg>(Visitor<TInstance, TArg> visitor, TArg arg)
        {
            visitor.VisitTransition(this, arg);
        }

		/// <summary>
		/// Returns the name of the transition
		/// </summary>
		/// <returns>The name of the transition.</returns>
		/// <remarks>The name of the transition is a composite of the source and target vertex names.</remarks>
        public override string ToString () {
			return "[" + (this.Target != null ? (this.Source.ToString() + " -> " + this.Target.ToString()) : this.Source.ToString()) + "]";
		}
	}
}