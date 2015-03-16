/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// Defines the specific semantics of a PseudoState in which it is used.
	/// </summary>
	public enum PseudoStateKind {
		/// <summary>
		/// An initial pseudostate represents a default vertex that is the source for a single transition to the default state of a Region.
		/// There can be at most one initial vertex in a Region.
		/// The outgoing transition from the initial vertex may have behavior, but not guard (as defined within the When method).
		/// </summary>
		Initial,

		/// <summary>
		/// DeepHistory represents the most recent active configuration of the Region that directly contains this pseudostate (e.g., the state configuration that was active when the composite state was last exited).
		/// A Region can have at most one deep history vertex.
		/// At most one transition may originate from the history connector to the default deep history state; this transition is taken in case the composite state had never been active before.
		/// Entry actions of states entered on the implicit direct path from the deep history to the innermost state(s) represented by a deep history are performed.
		/// The entry action is preformed only once for each state in the active state configuration being restored.
		/// </summary>
		DeepHistory,

		// TODO: EntryPoint

		// TODO: ExitPoint

		/// <summary>
		/// ShallowHistory represents the most recent active substate of its containing state (but not the substates of that substate).
		/// A Region can have at most one shallow history vertex.
		/// A transition coming into the shallow history vertex is equivalent to a transition coming into the most recent active substate of a state.
		/// At most one transition may originate from the history connector to the default shallow history state.
		/// This transition is taken in case the composite state had never been active before. The entry action of the state represented by the shallow history is performed.
		/// </summary>
		ShallowHistory,

		/// <summary>
		/// Junction vertices are semantic-free vertices that are used to chain together multiple transitions.
		/// They are used to construct compound transition paths between states. For example, a junction can be used to converge multiple incoming transitions into a single outgoing transition representing a shared transition path (this is known as a merge).
		/// Conversely, they can be used to split an incoming transition into multiple outgoing transition segments with different guard conditions. This realizes a static conditional branch. (In the latter case, outgoing transitions whose guard conditions evaluate to false are disabled. A predefined guard denoted “else” may be defined for at most one outgoing transition. This transition is enabled if all the guards labeling the other transitions are false.)
		/// Static conditional branches are distinct from dynamic conditional branches that are realized by choice vertices.
		/// </summary>
		Junction,

		/// <summary>
		/// Choice vertices which, when reached, result in the dynamic evaluation of the guards of the triggers of its outgoing transitions.
		/// This realizes a dynamic conditional branch. It allows splitting of transitions into multiple outgoing paths such that the decision on which path to take may be a function of the results of prior actions performed in the same run- to-completion step.
		/// If more than one of the guards evaluates to true, an arbitrary one is selected. If none of the guards evaluates to true, then the model is considered ill-formed. (To avoid this, it is recommended to define one outgoing transition with the predefined “else” guard for every choice vertex.)
		/// Choice vertices should be distinguished from static branch points that are based on junction points.
		/// </summary>
		Choice,

		/// <summary>
		/// Entering a Terminate pseudostate implies that the execution of this state machine instance is terminated.
		/// The state machine does not exit any states nor does it perform any exit actions other than those associated with the transition leading to the terminate pseudostate.
		/// </summary>
		Terminate
	}
}