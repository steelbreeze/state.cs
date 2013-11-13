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
	/// Enumeration of the valid kinds of pseudo states.
	/// </summary>
	public enum PseudoStateKind
	{
		/// <summary>
		/// Enables a dynamic conditional branches; within a compound transition.
		/// </summary>
		/// <remarks>
		/// Choice pseudo states may have multiple outbound transitions; when reached the guards are evaluated and an appropriate outbound transition is followed.
		/// If more than one outbound transition guard evaluates true, an arbitary one is selected.
		/// If no outbound transition guard evaluates true an 'else' transition is looked for.
		/// If no outbound transition or else transition are found, the state machine model is deemed to be malformed and an exception is thrown.
		/// </remarks>
		Choice,

		/// <summary>
		/// A type of initial pseudo state; forms the initial starting point when entering a region or composite state for the first time.
		/// Subsiquent entry of the owning (parent) region or composite state will enter the last know active state for the region or composite state.
		/// Deep history cascades the history behaviour to any and all child region and composite or orthogonal states.
		/// </summary>
		DeepHistory,

		/// <summary>
		/// A type of initial pseudo state; forms the initial starting point when entering a region or composite state for the first time.
		/// Subsiquent entry of the owning (parent) region or composite state will enter at the initial pseudo state unless deep history is in force in some ancestor region or composite state.
		/// </summary>
		Initial,

		/// <summary>
		/// Enables a static conditional branches; within a compound transition.
		/// </summary>
		/// <remarks>
		/// Junction pseudo states may have multiple outbound transitions; when reached the guards are evaluated and an appropriate outbound transition is followed.
		/// If more than one outbound transition guard evaluates true, the state machine model is deemed to be malformed and an exception is thrown.
		/// If no outbound transition guard evaluates true an 'else' transition is looked for.
		/// If no outbound transition or else transition are found, the state machine model is deemed to be malformed and an exception is thrown.
		/// </remarks>
		Junction,

		/// <summary>
		/// A type of initial pseudo state; forms the initial starting point when entering a region or composite state for the first time.
		/// Subsiquent entry of the owning (parent) region or composite state will enter the last know active state for the region or composite state.
		/// </summary>
		ShallowHistory,

		/// <summary>
		/// Entering a terminate pseudostate implies that the execution of this state machine by means of its context object is terminated.
		/// </summary>
		/// <remarks>
		/// The state machine ceases to be active upon entry to the terminate pseudo state.
		/// No further actions are performed and the state machine will not respond to messages.
		/// </remarks>
		Terminate
	}

	internal static class PseudoStateKindMethods
	{
		private static readonly Random random = new Random();

		internal static Boolean IsHistory( this PseudoStateKind pseudoStateKind )
		{
			switch( pseudoStateKind )
			{
				case PseudoStateKind.DeepHistory:
				case PseudoStateKind.ShallowHistory:
					return true;

				default:
					return false;
			}
		}

		internal static Boolean IsInitial( this PseudoStateKind pseudoStateKind )
		{
			switch( pseudoStateKind )
			{
				case PseudoStateKind.DeepHistory:
				case PseudoStateKind.Initial:
				case PseudoStateKind.ShallowHistory:
					return true;

				default:
					return false;
			}
		}

		internal static Transition Completion( this PseudoStateKind pseudoStateKind, ICollection<Transition> completions )
		{
			switch( pseudoStateKind )
			{
				case PseudoStateKind.Choice:
					return GetChoiceCompletion( completions );

				case PseudoStateKind.Junction:
					return GetJunctionCompletion( completions );

				case PseudoStateKind.Terminate:
					return null;

				default: // the initial pseudo states
					return completions.ElementAt( 0 );
			}
		}

		private static Transition GetChoiceCompletion( IEnumerable<Transition> c )
		{
			var items = c.Where( t => t.Guard() );
			var count = items.Count();

			return count > 0 ? items.ElementAt( random.Next( count ) ) : c.Single( t => t.IsElse );
		}

		private static Transition GetJunctionCompletion( IEnumerable<Transition> c )
		{
			return c.SingleOrDefault( t => t.Guard() ) ?? c.Single( t => t.IsElse );
		}
	}
}
