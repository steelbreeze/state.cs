/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
using System.Collections.Generic;

namespace Steelbreeze.StateMachines.Model {
	public class Vertex<TInstance> : NamedElement where TInstance : IInstance<TInstance> {
		public readonly Region<TInstance> Region;
		public readonly ICollection<Transition<TInstance>> Outgoing = new HashSet<Transition<TInstance>>();

		internal Vertex (string name, Region<TInstance> parent)
			: base(name, parent) {

			this.Region = parent;

			if (this.Region != null) {
				this.Region.Vertices.Add(this);

				this.Region.Root.Clean = false;
			}
		}

		internal List<Vertex<TInstance>> Ancestry() {
			var ancestors = this.Region != null ? this.Region.State.Ancestry() : new List<Vertex<TInstance>>();

			ancestors.Add(this);

			return ancestors;
		}

		/// <summary>
		/// Returns the state machine that this vertex is a part of.
		/// </summary>
		public virtual StateMachine<TInstance> Root {
			get {
				return this.Region.Root;
			}
		}

		public Transition<TInstance> To (Vertex<TInstance> target = null, TransitionKind kind = TransitionKind.External) {
			return new Transition<TInstance>(this, target, kind);
		}

		/// <summary>
		/// Accepts a visitor.
		/// </summary>
		/// <param name="visitor">The visitor to accept.</param>
		public virtual void Accept(Visitor<TInstance> visitor)
        {
            visitor.VisitVertex(this);
        }

		/// <summary>
		/// Accepts a visitor.
		/// </summary>
		/// <typeparam name="TArg">The type of the argument passed into the visitor.</typeparam>
		/// <param name="visitor">The visitor to accept.</param>
		/// <param name="arg">The argument to pass to each element visited.</param>
		public virtual void Accept<TArg>(Visitor<TInstance, TArg> visitor, TArg arg)
        {
            visitor.VisitVertex(this, arg);
        }
    }
}