/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
namespace Steelbreeze.StateMachines.Model {
	/// <summary>
	/// Defines the specific semantics of a Transition.
	/// </summary>
	public enum TransitionKind {
		/// <summary>
		/// An internal transition may only be applied to a state; it only calls the transition behavior when called and will not cause a state transition.
		/// </summary>
		Internal,

		/// <summary>
		/// A local transition may be applied only to a composite state as its source and the target must be a child of the source; it does not exit the source composite state, but will enter and exit child elements as required.
		/// </summary>
		Local,

		/// <summary>
		/// An external transition is the default transition type; it will exit the source vertex and enter the target.
		/// </summary>
		External
	}
}