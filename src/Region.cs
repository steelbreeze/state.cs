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
	/// A container of vertices within a state machine model.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	/// <remarks>
	/// All state machines and composite states will contain at least one Region; orthogonal composite states will contain more than one.
	/// </remarks>
	public sealed class Region<TInstance> : Element<TInstance> where TInstance : class, IActiveStateConfiguration<TInstance> {
		#region Static members
		/// <summary>
		/// The name used for default regions.
		/// </summary>
		/// <remarks>
		/// Used when a region is created implicitly; typically when a StateMachine or State is passed where a Region is expected.
		/// </remarks>
		public static String DefaultName { get; set; }

		// Initialise the static members.
		static Region () {
			DefaultName = "default";
		}

		/// <summary>
		/// Provides an implicit conversion from a State to a Region, initialising a new instance of the Region class if required under this State.
		/// </summary>
		/// <param name="state">The State to return the default region for.</param>
		/// <returns>The default Region of the composite State.</returns>
		/// <remarks>
		/// The default Region of a composite State is one that has the same name as defined by Region.DefaultName.
		/// </remarks>
		public static implicit operator Region<TInstance> (State<TInstance> state) {
			return state.Regions.SingleOrDefault (r => r.Name == Region<TInstance>.DefaultName) ?? new Region<TInstance> (Region<TInstance>.DefaultName, state);
		}
		#endregion
		/// <summary>
		/// The initial stating state for the region.
		/// </summary>
		/// <remarks>Note that all regions do not have to have an initial state if all entry is via external transitions.</remarks>
		internal PseudoState<TInstance> Initial = null;

		/// <summary>
		/// The vertices owned by this region.
		/// </summary>
		private readonly HashSet<Vertex<TInstance>> vertices = new HashSet<Vertex<TInstance>> ();

		/// <summary>
		/// The regions parement state.
		/// </summary>
		private readonly State<TInstance> parent;

		/// <summary>
		/// Initialises a new instance of the Region class within a composite State.
		/// </summary>
		/// <param name="name">The name of the Region.</param>
		/// <param name="parent">The parent composite State.</param>
		/// <remarks>
		/// Adding a Region to a State makes the State a composite State.
		/// </remarks>
		public Region (String name, State<TInstance> parent)
			: base (name, parent) {
			Trace.Assert (name != null, "Regions must have a name");
			Trace.Assert (parent != null, "Regions must have a parent");

			this.parent = parent;

			parent.Add (this);
		}


		/// <summary>
		/// Returns the Region's parent element.
		/// </summary>
		public override Element<TInstance> Parent {
			get {
				return this.parent;
			}
		}

		/// <summary>
		/// The child Vertices
		/// </summary>
		public IEnumerable<Vertex<TInstance>> Vertices {
			get {
				return this.vertices;
			}
		}

		/// <summary>
		/// Tests the Region to determine if it is part of the current active state confuguration
		/// </summary>
		/// <param name="instance">The state machine instance.</param>
		/// <returns>True if the element is active.</returns>
		public override Boolean IsActive (TInstance instance) {
			return this.Parent.IsActive (instance);
		}

		/// <summary>
		/// Adds a child vertex to the region
		/// </summary>
		/// <param name="vertex"></param>
		internal void Add (Vertex<TInstance> vertex) {
			// some validation
			Trace.Assert (vertex != null, "Cannot add a null vertex");
			Trace.Assert (this.vertices.Where (v => v.Name == vertex.Name).Count () == 0, "Vertices must have a unique name within the scope of their parent Region");

			// add the vertex
			this.vertices.Add (vertex);

			// invalidate the model
			vertex.Root.Clean = false;
		}

		/// <summary>
		/// Tests the region to determine if it is deemed to be complete.
		/// </summary>
		/// <param name="instance">The state machine instance.</param>
		/// <returns>True if the region is complete.</returns>
		/// <remarks>A region is deemed to be complete if it current active state is complete.</remarks>
		public override Boolean IsComplete (TInstance instance) {
			return instance[ this ].IsFinal;
		}

		/// <summary>
		/// Evaluate a message to trigger a state transition.
		/// </summary>
		/// <param name="message">The messaege that may trigger a state transition.</param>
		/// <param name="instance">The state machine instance.</param>
		/// <returns>True if a transition was triggered.</returns>
		internal Boolean Evaluate (Object message, TInstance instance) {
			return instance[ this ].Evaluate (message, instance);
		}

		/// <summary>
		/// Accepts a visitor
		/// </summary>
		/// <param name="visitor">The visitor to visit.</param>
		/// <param name="param">A parameter passed to the visitor when visiting elements.</param>
		/// <remarks>
		/// A visitor will walk the state machine model from this element to all child elements including transitions calling the approritate visit method on the visitor.
		/// </remarks>
		public override void Accept<TParam> (Visitor<TInstance, TParam> visitor, TParam param) {
			visitor.VisitRegion (this, param);
		}
	}
}