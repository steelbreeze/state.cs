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

namespace Steelbreeze.Behavior {
	/// <summary>
	/// A continuation transition between states or pseudo states within a state machine.
	/// </summary>
	/// <remarks>
	/// Continuation transitions are tested for after sucessful entry to pseudo states or completed states.
	/// </remarks>
	public class Completion {
		public sealed class Else : Completion {
			public Else(PseudoState source, PseudoState target) : base(source, target, () => false) { }

			public Else(PseudoState source, SimpleState target) : base(source, target, () => false) { }
		}


		/// <summary>
		/// The guard comdition to be used within 'else' transitions.
		/// </summary>
		/// <remarks>
		/// An 'else' transition is a default path to follow after a choice or junction pseudo state where no other transition's guards evaluate true.
		/// </remarks>
		//		public static readonly Func<Boolean> Else = () => false;

		private Action<IState> onExit;
		private Action<IState> onBeginEnter;
		private IVertex target;
		private Func<Boolean> guard;

		//		internal Boolean IsElse { get { return this.guard.Equals( Else ); } }

		/// <summary>
		/// The action(s) to perform while traversing the transition.
		/// </summary>
		public event Action Effect;

		/// <summary>
		/// Creates a continuation transition between pseudo states.
		/// </summary>
		/// <param name="source">The source pseudo state.</param>
		/// <param name="target">The target pseudo state.</param>
		/// <param name="guard">The guard condition to be tested in order to follow the transition.</param>
		/// <remarks>For initial pseudo states, this type of tranision initiates a compound transition, for others, it is a particiapnt in a compound transition.</remarks>
		public Completion(PseudoState source, PseudoState target, Func<Boolean> guard = null) {
			this.target = target;
			this.guard = guard;

			Completion.Path(source, target, ref onExit, ref onBeginEnter);

			(source.completions ?? (source.completions = new HashSet<Completion>())).Add(this);
		}

		/// <summary>
		/// Creates a continuation transition from a pseudo state to a state.
		/// </summary>
		/// <param name="source">The source pseudo state.</param>
		/// <param name="target">The target state.</param>
		/// <param name="guard">The guard condition to be tested in order to follow the transition.</param>
		/// <remarks>This type of transition completes a compound transition.</remarks>
		public Completion(PseudoState source, SimpleState target, Func<Boolean> guard = null) {
			this.target = target;
			this.guard = guard;

			Completion.Path(source, target, ref onExit, ref onBeginEnter);

			(source.completions ?? (source.completions = new HashSet<Completion>())).Add(this);
		}

		/// <summary>
		/// Creates a continuation transition from a state to a pseudo state.
		/// </summary>
		/// <param name="source">The source state.</param>
		/// <param name="target">The target pseudo state.</param>
		/// <param name="guard">The guard condition to be tested in order to follow the transition.</param>
		/// <remarks>Continuation transitions are tested for after a state has been entered if the state is deemed to be completed.</remarks>
		/// <remarks>This type of transition initiates a compound transition.</remarks>
		public Completion(SimpleState source, PseudoState target, Func<Boolean> guard = null) {
			this.target = target;
			this.guard = guard;

			Completion.Path(source, target, ref onExit, ref onBeginEnter);

			(source.completions ?? (source.completions = new HashSet<Completion>())).Add(this);
		}

		/// <summary>
		/// Creates a continuation transition between states.
		/// </summary>
		/// <param name="source">The source state.</param>
		/// <param name="target">The target state.</param>
		/// <param name="guard">The guard condition to be tested in order to follow the transition.</param>
		/// <remarks>Continuation transitions are tested for after a state has been entered if the state is deemed to be completed.</remarks>
		public Completion(SimpleState source, SimpleState target, Func<Boolean> guard = null) {
			this.target = target;
			this.guard = guard;

			Completion.Path(source, target, ref onExit, ref onBeginEnter);

			(source.completions ?? (source.completions = new HashSet<Completion>())).Add(this);
		}

		internal Boolean Guard() {
			return guard == null || guard();
		}

		internal void Traverse(IState context, Boolean deepHistory) {
			if(onExit != null)
				onExit(context);

			OnEffect();

			if(onBeginEnter != null)
				onBeginEnter(context);

			if(target != null)
				target.OnEndEnter(context, deepHistory);
		}

		/// <summary>
		/// Invokes the transition effect action.
		/// </summary>
		/// <remarks>Override this method to create custom transition behaviour.</remarks>
		protected virtual void OnEffect() {
			if(Effect != null)
				Effect();
		}

		internal static void Path(IVertex source, IVertex target, ref Action<IState> onExit, ref Action<IState> onBeginEnter) {
			var sourceAncestors = source.Ancestors().Reverse().GetEnumerator();
			var targetAncestors = target.Ancestors().Reverse().GetEnumerator();

			while(sourceAncestors.MoveNext() && targetAncestors.MoveNext() && sourceAncestors.Current.Equals(targetAncestors.Current)) { }

			if(source is PseudoState && !sourceAncestors.Current.Equals(source))
				onExit += source.OnExit;

			onExit += sourceAncestors.Current.OnExit;

			do { onBeginEnter += targetAncestors.Current.OnBeginEnter; } while(targetAncestors.MoveNext());
		}
	}
}
