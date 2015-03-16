/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
/*
namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// A StateMachine is the root node of a hierarchical state machine model.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	public sealed class StateMachine<TInstance> : State<TInstance> where TInstance : IActiveStateConfiguration<TInstance> {
		internal Boolean Clean { get; set; }

		/// <summary>
		/// Initialises a new instance of the StateMachine class.
		/// </summary>
		/// <param name="name">The name of the StateMachine.</param>
		public StateMachine (String name)
			: base (name, null) {

			Trace.Assert (name != null, "StateMachines must have a name");

			this.Root = this;
		}

		/// <summary>
		/// Returns the elements parent.
		/// </summary>
		/// <remarks>
		/// A StateMachine will have no parent; this value will always be null.
		/// </remarks>
		public override Element<TInstance> Parent { get { return null; } }

		protected internal override bool IsActive (IActiveStateConfiguration<TInstance> instance) {
			return true;
		}

		/// <summary>
		/// Initialises a state machine model.
		/// </summary>
		/// <remarks>
		/// Initialising a state machine model pre-compiles all the transitions.
		/// This process will be triggered automatically on any call to StateMachine.Initialise or StateMachine.Process if the model structure has changed.
		/// If you want to take greater control of when this happens, pass autoInitialise = false to StateMachine.Initialise or StateMachine.Process and call Initialise as required instead.
		/// </remarks>
		public void Initialise () {
			this.Reset ();
			this.Clean = true;

			this.BootstrapElement (false);
			this.BootstrapTransitions ();
		}

		/// <summary>
		/// Initialises a state machine instance to its initial state; this causes the state machine instance to enter its initial state.
		/// </summary>
		/// <param name="instance">The state machine instance.</param>
		/// <param name="autoInitialise">True if you wish to automatically re-initialise the state machine model prior to initialising the state machine instance.</param>
		public void Initialise (TInstance instance, Boolean autoInitialise = true) {
			if (!this.Clean && autoInitialise)
				this.Initialise ();

			this.Enter (null, instance, false);
		}

		internal override void BootstrapElement (bool deepHistoryAbove) {
			base.Reset ();
			this.Clean = true;

			base.BootstrapElement (deepHistoryAbove);
			base.BootstrapTransitions ();
		}

		/// <summary>
		/// Pass a message to a state machine instance for evaluation.
		/// </summary>
		/// <param name="instance">The state machine instance to evaluate the message against.</param>
		/// <param name="message">The message to evaluate.</param>
		/// <param name="autoInitialise">True if you wish to automatically re-initialise the state machine model prior to evaluating the message.</param>
		/// <returns>True if the message triggered a state transition.</returns>
		/// <remarks>
		/// Note that due to the potential for orthogonal Regions in composite States, it is possible for multiple transitions to be triggered.
		/// </remarks>
		public Boolean Evaluate (Object message, TInstance instance, Boolean autoInitialise = true) {
			if (!this.Clean && autoInitialise)
				this.Initialise ();

			if (instance.IsTerminated)
				return false;

			return base.Evaluate (message, instance);
		}
	}
}
*/

// TODO: inherit from State (thereby enabling a machine to be used in another region if required)
namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// A StateMachine is the root node of a hierarchical state machine model.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	public class StateMachine<TInstance> : Element<TInstance> where TInstance : IActiveStateConfiguration<TInstance> {
		/// <summary>
		/// The name of the type without generic considerations
		/// </summary>
		public override string Type { get { return "stateMachineModel"; } }

		/// <summary>
		/// The child Regions.
		/// </summary>
		public IEnumerable<Region<TInstance>> Regions { get { return this.regions; } }

		internal Boolean Clean { get; set; }
		internal Region<TInstance>[] regions;

		/// <summary>
		/// Returns the elements parent.
		/// </summary>
		/// <remarks>
		/// A StateMachine will have no parent; this value will always be null.
		/// </remarks>
		public override Element<TInstance> Parent { get { return null; } }

		/// <summary>
		/// Initialises a new instance of the StateMachine class.
		/// </summary>
		/// <param name="name">The name of the StateMachine.</param>
		public StateMachine (String name)
			: base (name, null) {

			Trace.Assert (name != null, "StateMachines must have a name");

			this.Root = this;
		}

		/// <summary>
		/// Tests the StateMachine  to determine if it is part of the current active state confuguration
		/// </summary>
		/// <param name="instance">The state machine instance.</param>
		/// <returns>True if the element is active.</returns>
		internal protected override Boolean IsActive (IActiveStateConfiguration<TInstance> instance) {
			return true;
		}

		internal void Add (Region<TInstance> region) {
			if (this.regions == null)
				this.regions = new Region<TInstance>[ 1 ] { region };
			else {
				Trace.Assert (this.regions.Where (r => r.Name == region.Name).Count () == 0, "Regions must have a unique name within the scope of their parent StateMachine");

				var regions = new Region<TInstance>[ this.regions.Length + 1 ];

				this.regions.CopyTo (regions, 0);

				regions[ this.regions.Length ] = region;

				this.regions = regions;
			}

			this.Clean = false;
		}

		/// <summary>
		/// Initialises a state machine model.
		/// </summary>
		/// <remarks>
		/// Initialising a state machine model pre-compiles all the transitions.
		/// This process will be triggered automatically on any call to StateMachine.Initialise or StateMachine.Process if the model structure has changed.
		/// If you want to take greater control of when this happens, pass autoInitialise = false to StateMachine.Initialise or StateMachine.Process and call Initialise as required instead.
		/// </remarks>
		public void Initialise () {
			this.Reset ();
			this.Clean = true;

			this.BootstrapElement (false);
			this.BootstrapTransitions ();
		}

		/// <summary>
		/// Initialises a state machine instance to its initial state; this causes the state machine instance to enter its initial state.
		/// </summary>
		/// <param name="instance">The state machine instance.</param>
		/// <param name="autoInitialise">True if you wish to automatically re-initialise the state machine model prior to initialising the state machine instance.</param>
		public void Initialise (TInstance instance, Boolean autoInitialise = true) {
			if (!this.Clean && autoInitialise)
				this.Initialise ();

			this.Enter (null, instance, false);
		}

		/// <summary>
		/// Determines if the state machine instance has completed its processsing.
		/// </summary>
		/// <param name="instance">The state machine instance to test completeness for.</param>
		/// <returns>True if the state machine instance has completed.</returns>
		/// <remarks>
		/// A state machine instance is deemed complete when all its child Regions are complete.
		/// A Region is deemed complete if its current state is a FinalState (States are also considered to be FinalStates if there are no outbound transitions).
		/// In addition, if a state machine instanve is terminated (by virtue of a transition to a Terminate PseudoState) it is also deemed to be completed.
		/// </remarks>
		public Boolean IsComplete (TInstance instance) {
			return instance.IsTerminated || this.regions.All (region => region.IsComplete (instance));
		}

		/// <summary>
		/// Pass a message to a state machine instance for evaluation.
		/// </summary>
		/// <param name="instance">The state machine instance to evaluate the message against.</param>
		/// <param name="message">The message to evaluate.</param>
		/// <param name="autoInitialise">True if you wish to automatically re-initialise the state machine model prior to evaluating the message.</param>
		/// <returns>True if the message triggered a state transition.</returns>
		/// <remarks>
		/// Note that due to the potential for orthogonal Regions in composite States, it is possible for multiple transitions to be triggered.
		/// </remarks>
		public Boolean Evaluate (Object message, TInstance instance, Boolean autoInitialise = true) {
			if (!this.Clean && autoInitialise)
				this.Initialise ();

			Boolean processed = false;

			if (!instance.IsTerminated)
				for (int i = 0, l = this.regions.Length; i < l; ++i)
					if (this.regions[ i ].Evaluate (message, instance))
						processed = true;

			return processed;
		}

		internal override void BootstrapElement (Boolean deepHistoryAbove) {
			foreach (var region in this.regions) {
				region.Reset ();
				region.BootstrapElement (deepHistoryAbove);

				this.EndEnter += region.Enter;
			}

			base.BootstrapElement (deepHistoryAbove);
		}

		internal override void BootstrapTransitions () {
			foreach (var region in this.regions)
				region.BootstrapTransitions ();
		}
	}
}