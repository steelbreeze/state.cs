/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
using System;

namespace Steelbreeze.StateMachines.Model {
	public class Transition<TInstance> where TInstance : IInstance<TInstance> {
		internal delegate bool Guard (object message, TInstance instance);

		static internal Guard TrueGuard = (message, instance) => { return true; };
		static internal Guard FalseGuard = (message, instance) => { return false; };
		internal Guard guard;
		internal Behavior<TInstance> transitionBehavior;
		internal Behavior<TInstance> onTraverse;
		public readonly Vertex<TInstance> Source;
		public readonly Vertex<TInstance> Target;
		public readonly TransitionKind Kind;

		public Transition (Vertex<TInstance> source, Vertex<TInstance> target = null, TransitionKind kind = TransitionKind.External) {
			this.Source = source;
			this.Target = target;
			this.Kind = target != null ? kind : TransitionKind.Internal;
			this.guard = (source is PseudoState<TInstance>) ? Transition<TInstance>.TrueGuard : (message, instance) => message == this.Source;

			this.Source.Outgoing.Add(this);

			this.Source.Root.Clean = false;
		}

		public Transition<TInstance> Else () {
			this.guard = Transition<TInstance>.FalseGuard;

			return this;
		}

		public Transition<TInstance> When<TMessage> (Func<TMessage, TInstance, bool> predicate) where TMessage : class {
			this.guard = (message, instance) => { return message is TMessage && predicate(message as TMessage, instance); };

			return this;
		}

		public Transition<TInstance> When<TMessage> (Func<TMessage, bool> predicate) where TMessage : class {
			this.guard = (message, instance) => { return message is TMessage && predicate(message as TMessage); };

			return this;
		}

		public Transition<TInstance> When (Func<TInstance, bool> predicate) {
			this.guard = (message, instance) => { return predicate(instance); };

			return this;
		}

		public Transition<TInstance> Effect<TMessage> (Action<TMessage, TInstance> transitionAction) where TMessage : class {
			this.transitionBehavior += (message, instance, history) => { if (message is TMessage) transitionAction(message as TMessage, instance); };

			this.Source.Root.Clean = false;

			return this;
		}

		public Transition<TInstance> Effect<TMessage> (Action<TMessage> transitionAction) where TMessage : class {
			this.transitionBehavior += (message, instance, history) => { if (message is TMessage) transitionAction(message as TMessage); };

			this.Source.Root.Clean = false;

			return this;
		}

		public Transition<TInstance> Effect (Action<TInstance> transitionAction) {
			this.transitionBehavior += (message, instance, history) => { transitionAction(instance); };

			this.Source.Root.Clean = false;

			return this;
		}

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

        public override string ToString () {
			return "[" + (this.Target != null ? (this.Source.ToString() + " -> " + this.Target.ToString()) : this.Source.ToString()) + "]";
		}
	}
}