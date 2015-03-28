/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Diagnostics;
using System.Linq;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// A PseudoState is an abstraction that encompasses different types of transient vertices in the state machine.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	/// <remarks>
	/// Pseudostates are typically used to connect multiple transitions into more complex state transitions path.
	/// </remarks>
	public sealed class PseudoState<TInstance> : Vertex<TInstance> where TInstance : class, IActiveStateConfiguration<TInstance> {
		#region Static members
		/// <summary>
		/// For use in Choice pseudo states where multiple outbound transitions guards evaluate true.
		/// </summary>
		private static readonly Random random = new Random ();
		#endregion
		/// <summary>
		/// Determines the precise type of the Pseudostate.
		/// </summary>
		/// <remarks>
		/// The default kind of a PseudoState is Initial.
		/// </remarks>
		public readonly PseudoStateKind Kind;

		/// <summary>
		/// True if the pseudo state is a DeepHistory or ShallowHistory kinds.
		/// </summary>
		internal Boolean IsHistory { get { return this.Kind == PseudoStateKind.DeepHistory || this.Kind == PseudoStateKind.ShallowHistory; } }

		/// <summary>
		/// True if the pseudo state is an Initial, DeepHistory or ShallowHistory kinds.
		/// </summary>
		internal Boolean IsInitial { get { return this.Kind == PseudoStateKind.Initial || this.IsHistory; } }

		/// <summary>
		/// Initialises a new instance of the PseudoState class.
		/// </summary>
		/// <param name="name">The name of the PseudoState.</param>
		/// <param name="parent">The parent Region.</param>
		/// <param name="kind">The kind of the PseudoState</param>
		/// <remarks>
		/// The kind of the PseudoState dictates is use and semantics; see the documentation of PseudoStateKind.
		/// </remarks>
		public PseudoState (String name, Region<TInstance> parent, PseudoStateKind kind = PseudoStateKind.Initial)
			: base (name, parent) {
			Trace.Assert (name != null, "PseudoStates must have a name");
			Trace.Assert (parent != null, "PseudoStates must have a parent Region");

			this.Kind = kind;

			if (this.IsInitial)
				this.Region.Initial = this;
		}

		/// <summary>
		/// Tests the vertex to determine if it is part of the current active state confuguration
		/// </summary>
		/// <param name="instance">The state machine instance.</param>
		/// <returns>True if the element is active.</returns>
		public override Boolean IsActive (TInstance instance) {
			return this.Parent.IsActive (instance);
		}

		/// <summary>
		/// Tests the pseudo state to determine if it is deemed to be complete.
		/// </summary>
		/// <param name="instance">The state machine instance.</param>
		/// <returns>True if the pseudo state is complete.</returns>
		/// <remarks>Pseudo states are always deemed to be complete.</remarks>
		public override bool IsComplete (TInstance instance) {
			return true;
		}

		/// <summary>
		/// Selects a transition for a given message and state machine instance for the pseudo state.
		/// </summary>
		/// <param name="message">The message that may trigger a transition.</param>
		/// <param name="instance">The state machine instance.</param>
		/// <returns>The selected transition.</returns>
		/// <exception cref="System.ArgumentNullException">This exception is thown if no transition is found from the pseudo state.</exception>
		protected internal override Transition<TInstance> Select (object message, TInstance instance) {
			switch (this.Kind) {
				case PseudoStateKind.Initial:
				case PseudoStateKind.DeepHistory:
				case PseudoStateKind.ShallowHistory:
					return this.Transitions.Single ();

				case PseudoStateKind.Choice:
				case PseudoStateKind.Junction:
					var transitions = this.Transitions.Where (t => t.Predicate (message, instance));

					switch (transitions.Count ()) {
						case 1:
							return transitions.Single ();

						case 0:
							transitions = this.Transitions.Where (t => t.Predicate.Equals( Transition<TInstance>.IsElse));

							if( transitions.Count() == 0 )
								throw new Exception (String.Format ("No transitions found from {0} for message {1} on instance {2}", this, message, instance));

							return transitions.Single ();

						default:
							if( this.Kind == PseudoStateKind.Junction )
								throw new Exception( String.Format( "Multiple transitions possble from {0} for message {1} on instance {2}", this, message, instance));

							return transitions.ElementAt (random.Next (transitions.Count ()));
					}

				default:
					return null;
			}
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
			visitor.VisitPseudoState (this, param);
		}
	}
}