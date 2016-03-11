/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
using System.Collections.Generic;
using System.Linq;

namespace Steelbreeze.StateMachines.Model {
	/// <summary>
	/// Represents an element that can be the child of a region.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	public class Vertex<TInstance> : NamedElement where TInstance : IInstance<TInstance> {
		/// <summary>
		/// The parent region that this vertex belongs to
		/// </summary>
		public readonly Region<TInstance> Region;

		/// <summary>
		/// The set of transitions that this vertex is the source of.
		/// </summary>
		public readonly ICollection<Transition<TInstance>> Outgoing = new HashSet<Transition<TInstance>>();

		/// <summary>
		/// The set of transitions that this vertex is the target of.
		/// </summary>
		public readonly ICollection<Transition<TInstance>> Incoming = new HashSet<Transition<TInstance>>();

		internal Vertex (string name, Region<TInstance> parent)
			: base(name, parent) {

			this.Region = parent;

			if (this.Region != null) {
				this.Region.Vertices.Add(this);

				this.Root.Clean = false;
			}
		}

		/// <summary>
		/// Removes the vertex from the state machine model.
		/// </summary>
		public virtual void Remove() {
			var transitions = this.Outgoing.Union(this.Incoming).ToList();

			foreach (var transition in transitions) {
				transition.Remove();
			}

			this.Region.Vertices.Remove(this);

			System.Console.WriteLine("remove " + this);

			this.Root.Clean = false;
		}

		internal List<Vertex<TInstance>> Ancestry () {
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="kind"></param>
		/// <returns></returns>
		public Transition<TInstance> To (Vertex<TInstance> target = null, TransitionKind kind = TransitionKind.External) {
			return new Transition<TInstance>(this, target, kind);
		}

		/// <summary>
		/// Accepts a visitor.
		/// </summary>
		/// <param name="visitor">The visitor to accept.</param>
		public virtual void Accept (Visitor<TInstance> visitor) {
			visitor.VisitVertex(this);
		}

		/// <summary>
		/// Accepts a visitor.
		/// </summary>
		/// <typeparam name="TArg">The type of the argument passed into the visitor.</typeparam>
		/// <param name="visitor">The visitor to accept.</param>
		/// <param name="arg">The argument to pass to each element visited.</param>
		public virtual void Accept<TArg>(Visitor<TInstance, TArg> visitor, TArg arg) {
			visitor.VisitVertex(this, arg);
		}
	}
}