// Copyright © 2013 Steelbreeze Limited.
// This file is part of state.cs.
//
// state.cs is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published
// by the Free Software Foundation, either version 3 of the License,
// or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// The various kinds of PseudoState
	/// </summary>
	public sealed class PseudoStateKind
	{
		private static readonly Random random = new Random();

		private static Completion RandomOrDefault( IEnumerable<Completion> items )
		{
			return items.Count() == 0 ? default( Completion ) : items.ElementAt( random.Next( items.Count() ) );
		}

		/// <summary>
		/// Choice vertices which, when reached, result in the dynamic evaluation of the guards of the triggers of its outgoing transitions.
		/// This realizes a dynamic conditional branch.
		/// It allows splitting of transitions into multiple outgoing paths such that the decision on which path to take may be a function of the results of prior actions performed in the same run-to-completion step.
		/// If more than one of the guards evaluates to true, an arbitrary one is selected.
		/// If none of the guards evaluates to true, then the model is considered ill-formed. (To avoid this, it is recommended to define one outgoing transition with the predefined “else” guard for every choice vertex.)
		/// Choice vertices should be distinguished from static branch points that are based on junction points.
		/// </summary>
		public static readonly PseudoStateKind Choice = new PseudoStateKind( "Choice", c => RandomOrDefault( c.Where( t => t.guard() ) ) ?? c.Single( t => t.guard.Equals( Guard.Else ) ), false, false );

		/// <summary>
		/// DeepHistory represents the most recent active configuration of the Region that directly contains this PseudoState (e.g., the state configuration that was active when the Region was last exited).
		/// A composite state can have at most one deep history vertex.
		/// At most one transition may originate from the history connector to the default deep history state; this transition is taken in case the composite state had never been active before.
		/// Entry actions of states entered on the implicit direct path from the deep history to the innermost state(s) represented by a deep history are performed. The entry action is preformed only once for each state in the active state configuration being restored.
		/// </summary>
		public static readonly PseudoStateKind DeepHistory = new PseudoStateKind( "DeepHistory", Enumerable.Single, true, true );

		/// <summary>
		/// An EntryPoint PseudoState is an entry point of a Region.
		/// In each Region of the StateMachine or CompositeState it has at most a single Transition to a Vertex within the same Region.
		/// </summary>
		public static readonly PseudoStateKind EntryPoint = new PseudoStateKind( "EntryPoint", Enumerable.Single, false, true );

		/// <summary>
		/// An ExitPoint pseudostate is an exit point of a state machine or composite state.
		/// Entering an exit point within any region of the CompositeState or StateMachine referenced by a submachine state implies the exit of this CompositeState or submachine state and the triggering of the transition that has this exit point as source in the state machine enclosing the submachine or composite state.
		/// </summary>
		public static readonly PseudoStateKind ExitPoint = new PseudoStateKind( "ExitPoint", c => c.Single( t => t.guard() ), false, false );

		/// <summary>
		/// An Initial PseudoState represents a default Vertex that is the source for a single transition to the default state of a Region.
		/// There can be at most one initial vertex in a region.
		/// The outgoing transition from the initial vertex is a completion transition.
		/// </summary>
		public static readonly PseudoStateKind Initial = new PseudoStateKind( "Initial", Enumerable.Single, false ,true );

		/// <summary>
		/// Junction PseudoStates are semantic-free vertices that are used to chain together multiple transitions.
		/// They are used to construct compound transition paths between States.
		/// For example, a junction can be used to converge multiple incoming transitions into a single outgoing transition representing a shared transition path (this is known as a merge).
		/// Conversely, they can be used to split an incoming transition into multiple outgoing transition segments with different guard conditions.
		/// This realizes a static conditional branch. (In the latter case, outgoing transitions whose guard conditions evaluate to false are disabled.
		/// A predefined guard denoted “else” may be defined for at most one outgoing transition. This transition is enabled if all the guards labeling the other transitions are false.)
		/// Static conditional branches are distinct from dynamic conditional branches that are realized by choice vertices.
		/// </summary>
		public static readonly PseudoStateKind Junction = new PseudoStateKind( "Junction", c => c.SingleOrDefault( t => t.guard() ) ?? c.Single( t => t.guard.Equals( Guard.Else ) ), false, false );

		/// <summary>
		/// ShallowHistory represents the most recent active substate of its containing state (but not the substates of that substate).
		/// A composite state can have at most one shallow history vertex.
		/// A transition coming into the shallow history vertex is equivalent to a transition coming into the most recent active substate of a state.
		/// At most one transition may originate from the history connector to the default shallow history state.
		/// This transition is taken in case the composite state had never been active before.
		/// The entry action of the state represented by the shallow history is performed.
		/// </summary>
		public static readonly PseudoStateKind ShallowHistory = new PseudoStateKind( "ShallowHistory", Enumerable.Single, true, true );

		internal readonly String Name;
		internal readonly Boolean IsHistory;
		internal readonly Boolean IsInitial;
		internal readonly Func<IEnumerable<Completion>, Completion> GetCompletion;

		private PseudoStateKind( String name, Func<IEnumerable<Completion>, Completion> getCompletion, Boolean isHistory, Boolean isInitial )
		{
			Name = name;
			GetCompletion = getCompletion;
			IsHistory = isHistory;
			IsInitial = isInitial;
		}

		/// <summary>
		/// Returns the name of the PseudoStateKind.
		/// </summary>
		/// <returns>The name of the PseudoStateKind.</returns>
		public override string ToString()
		{
			return Name;
		}
	}
}