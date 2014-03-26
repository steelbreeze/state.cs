// The MIT License (MIT)
//
// Copyright (c) 2014 Steelbreeze Limited
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
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

		internal static Transition<TState> Completion<TState>( this PseudoStateKind pseudoStateKind, TState state, ICollection<Transition<TState>> completions ) where TState : IState<TState>
		{
			switch( pseudoStateKind )
			{
				case PseudoStateKind.Choice:
					var items = completions.Where( t => t.guard( state ) );
					var count = items.Count();

					return count > 0 ? items.ElementAt( random.Next( count ) ) : completions.Single( t => t is Transition<TState>.Else );

				case PseudoStateKind.Junction:
					return completions.SingleOrDefault( t => t.guard( state ) ) ?? completions.Single( t => t is Transition<TState>.Else );

				case PseudoStateKind.Terminate:
					return null;

				default: // the initial pseudo states
					return completions.ElementAt( 0 );
			}
		}
	}
}
