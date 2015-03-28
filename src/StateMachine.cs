/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
//using System.Collections.Generic;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// A StateMachine is the root node of a hierarchical state machine model.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	public class StateMachine<TInstance> : State<TInstance> where TInstance : class, IActiveStateConfiguration<TInstance> {
		#region Static members
		/// <summary>
		/// The visitor used to bootstrap the state machine model.
		/// </summary>
		private static Bootstrap<TInstance> Bootstrap = new Bootstrap<TInstance> ();
		#endregion
		
		/// <summary>
		/// Initialises a state machine instance to its initial state.
		/// </summary>
		internal Action<Object, TInstance, Boolean> initialise = null;

		/// <summary>
		/// Flag to denote that a state machine model requires bootstrapping.
		/// </summary>
		internal Boolean Clean { get; set; }

		/// <summary>
		/// Initialises a new instance of the StateMachine class.
		/// </summary>
		/// <param name="name">The name of the StateMachine.</param>
		public StateMachine (String name)
			: base (name, null) {
		}

		/// <summary>
		/// The parent state machine that this element forms a part of.
		/// </summary>
		public override StateMachine<TInstance> Root {
			get {
				return Parent == null ? this : Parent.Root;
			}
		}

		/// <summary>
		/// Returns the elements parent.
		/// </summary>
		/// <remarks>
		/// A StateMachine will have no parent; this value will always be null.
		/// </remarks>
		public override Element<TInstance> Parent { get { return null; } }

		/// <summary>
		/// Tests the element to determine if it is part of the current active state confuguration
		/// </summary>
		/// <param name="instance">The state machine instance.</param>
		/// <returns>True if the element is active.</returns>
		public override bool IsActive (TInstance instance) {
			return Parent == null ? true : base.IsActive (instance);
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
			this.Clean = true;

			this.Accept (Bootstrap, false);
		}

		/// <summary>
		/// Initialises a state machine instance to its initial state; this causes the state machine instance to enter its initial state.
		/// </summary>
		/// <param name="instance">The state machine instance.</param>
		/// <param name="autoInitialise">True if you wish to automatically re-initialise the state machine model prior to initialising the state machine instance.</param>
		public void Initialise (TInstance instance, Boolean autoInitialise = true) {
			if (!this.Clean && autoInitialise)
				this.Initialise ();

			this.initialise (null, instance, false);
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

		/// <summary>
		/// Accepts a visitor
		/// </summary>
		/// <param name="visitor">The visitor to visit.</param>
		/// <param name="param">A parameter passed to the visitor when visiting elements.</param>
		/// <remarks>
		/// A visitor will walk the state machine model from this element to all child elements including transitions calling the approritate visit method on the visitor.
		/// </remarks>
		public override void Accept<TParam> (Visitor<TInstance, TParam> visitor, TParam param) {
			visitor.VisitStateMachine (this, param);
		}
	}
}