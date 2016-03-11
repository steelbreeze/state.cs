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
	/// Represents a region within a state machine model.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine model.</typeparam>
	/// <remarks>A region is a container for vertices (states and pseudo states).</remarks>
	public class Region<TInstance> : NamedElement where TInstance : IInstance<TInstance> {
		/// <summary>
		/// The name used for default regions.
		/// </summary>
		public static string DefaultName = "default";

		/// <summary>
		/// Implicit conversion from a state to a region facilitating simpler model creation; returns (and creates as required) the state's default region.
		/// </summary>
		/// <param name="state">The state to return the default region for.</param>
		public static implicit operator Region<TInstance>(State<TInstance> state) {
			return state.Regions.SingleOrDefault(region => region.Name == Region<TInstance>.DefaultName) ?? new Region<TInstance>(Region<TInstance>.DefaultName, state);
		}

		/// <summary>
		/// The parent state of the region.
		/// </summary>
		public readonly State<TInstance> State;

		/// <summary>
		/// The child vertices of the region.
		/// </summary>
		public ICollection<Vertex<TInstance>> Vertices = new HashSet<Vertex<TInstance>>();

		/// <summary>
		/// Creates a new region as a child of a state.
		/// </summary>
		/// <param name="state">The parent state.</param>
		/// <param name="name">The name of the new region.</param>
		/// <returns>The newly created region.</returns>
		public Region (string name, State<TInstance> state)
			: base(name, state) {

			this.State = state;

			this.State.Regions.Add(this);

			this.State.Root.Clean = false;
		}

		/// <summary>
		/// Removes the region from the state machine model.
		/// </summary>
		public void Remove() {
			var vertices = this.Vertices.ToList();

			foreach( var vertex in vertices) {
				vertex.Remove();
			}

			this.State.Regions.Remove(this);

			System.Console.WriteLine("remove" + this);

			this.State.Root.Clean = false;
		}

		/// <summary>
		/// Returns the state machine that this region is a part of.
		/// </summary>
		public StateMachine<TInstance> Root {
			get {
				return this.State.Root;
			}
		}

		/// <summary>
		/// Accepts a visitor.
		/// </summary>
		/// <param name="visitor">The visitor to accept.</param>
		public virtual void Accept (Visitor<TInstance> visitor) {
			visitor.VisitRegion(this);
		}

		/// <summary>
		/// Accepts a visitor.
		/// </summary>
		/// <typeparam name="TArg">The type of the argument passed into the visitor.</typeparam>
		/// <param name="visitor">The visitor to accept.</param>
		/// <param name="arg">The argument to pass to each element visited.</param>
		public virtual void Accept<TArg>(Visitor<TInstance, TArg> visitor, TArg arg) {
			visitor.VisitRegion(this, arg);
		}
	}
}