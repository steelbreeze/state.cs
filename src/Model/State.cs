/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace Steelbreeze.StateMachines.Model {
	public class State<TInstance> : Vertex<TInstance> where TInstance : IInstance<TInstance> {
		internal Behavior<TInstance> exitBehavior;
		internal Behavior<TInstance> entryBehavior;

		public ICollection<Region<TInstance>> Regions = new HashSet<Region<TInstance>>();

		/// <summary>
		/// Creates a new state as a child of a region.
		/// </summary>
		/// <param name="region">The parent region.</param>
		/// <param name="name">The name of the new state.</param>
		/// <returns>The newly created state.</returns>
		public State (string name, Region<TInstance> region) : base(name, region) { }

		public Region<TInstance> DefaultRegion {
			get {
				return this.Regions.SingleOrDefault(r => r.Name == Region<TInstance>.DefaultName) ?? new Region<TInstance>(Region<TInstance>.DefaultName, this);
			}
		}

		public bool IsFinal {
			get {
				return this.Outgoing.Count == 0;
			}
		}

		public bool IsSimple {
			get {
				return this.Regions.Count == 0;
			}
		}

		public bool IsComposite {
			get {
				return this.Regions.Count > 0;
			}
		}

		public bool IsOrthogonal {
			get {
				return this.Regions.Count > 1;
			}
		}

		public State<TInstance> Exit (Action exitAction) {
			this.exitBehavior += (message, instance, history) => exitAction();

			this.Root.Clean = false;

			return this;
		}

		public State<TInstance> Exit (Action<TInstance> exitAction) {
			this.exitBehavior += (message, instance, history) => exitAction(instance);

			this.Root.Clean = false;

			return this;
		}

		public State<TInstance> Exit<TMessage> (Action<TMessage> exitAction) where TMessage : class {
			this.exitBehavior += (message, instance, history) => { if (message is TMessage) exitAction(message as TMessage); };

			this.Root.Clean = false;

			return this;
		}

		public State<TInstance> Exit<TMessage> (Action<TMessage, TInstance> exitAction) where TMessage : class {
			this.exitBehavior += (message, instance, history) => { if (message is TMessage) exitAction(message as TMessage, instance); };

			this.Root.Clean = false;

			return this;
		}

		public State<TInstance> Entry (Action entryAction) {
			this.entryBehavior += (message, instance, history) => entryAction();

			this.Root.Clean = false;

			return this;
		}

		public State<TInstance> Entry (Action<TInstance> entryAction) {
			this.entryBehavior += (message, instance, history) => entryAction(instance);

			this.Root.Clean = false;

			return this;
		}

		public State<TInstance> Entry<TMessage> (Action<TMessage> entryAction) where TMessage : class {
			this.entryBehavior += (message, instance, history) => { if (message is TMessage) entryAction(message as TMessage); };

			this.Root.Clean = false;

			return this;
		}

		public State<TInstance> Entry<TMessage> (Action<TMessage, TInstance> entryAction) where TMessage : class {
			this.entryBehavior += (message, instance, history) => { if (message is TMessage) entryAction(message as TMessage, instance); };

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Accepts a visitor.
		/// </summary>
		/// <param name="visitor">The visitor to accept.</param>
		public override void Accept(Visitor<TInstance> visitor)
        {
            visitor.VisitState(this);
        }

		/// <summary>
		/// Accepts a visitor.
		/// </summary>
		/// <typeparam name="TArg">The type of the argument passed into the visitor.</typeparam>
		/// <param name="visitor">The visitor to accept.</param>
		/// <param name="arg">The argument to pass to each element visited.</param>
		public override void Accept<TArg>(Visitor<TInstance, TArg> visitor, TArg arg)
        {
            visitor.VisitState(this, arg);
        }
    }
}