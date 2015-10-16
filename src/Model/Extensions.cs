/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */

namespace Steelbreeze.StateMachines.Model {
	/// <summary>
	/// Extensions to the core model used as a short-hand to create model elements without repeating the instance class.
	/// </summary>
	public static class Extensions {
		/// <summary>
		/// Creates a new region as a child of a state.
		/// </summary>
		/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
		/// <param name="state">The parent state.</param>
		/// <param name="name">The name of the new region.</param>
		/// <returns>The newly created region.</returns>
		public static Region<TInstance> CreateRegion<TInstance>(this State<TInstance> state, string name) where TInstance : IInstance<TInstance> {
			return new Region<TInstance>(name, state);
		}

		/// <summary>
		/// Creates a new pseudo state as a child of a region.
		/// </summary>
		/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
		/// <param name="region">The parent region.</param>
		/// <param name="name">The name of the new pseudo state.</param>
		/// <param name="kind">The kind of the new pseudo state.</param>
		/// <returns>The newly created pseudo state.</returns>
		public static PseudoState<TInstance> CreatePseudoState<TInstance>(this Region<TInstance> region, string name, PseudoStateKind kind = PseudoStateKind.Initial) where TInstance : IInstance<TInstance> {
			return new PseudoState<TInstance>(name, region, kind);
		}

		/// <summary>
		/// Creates a new pseudo state as a child of a state. 
		/// </summary>
		/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
		/// <param name="state">The parent state.</param>
		/// <param name="name">The name of the new pseudo state.</param>
		/// <param name="kind">The kind of the new pseudo state.</param>
		/// <returns>The newly created pseudo state.</returns>
		/// <remarks>The newly created pseduo state will be created in the default region of the parent state. The default region will be created as required.</remarks>
		public static PseudoState<TInstance> CreatePseudoState<TInstance>(this State<TInstance> state, string name, PseudoStateKind kind = PseudoStateKind.Initial) where TInstance : IInstance<TInstance> {
			return new PseudoState<TInstance>(name, state, kind);
		}

		/// <summary>
		/// Creates a new state as a child of a region.
		/// </summary>
		/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
		/// <param name="region">The parent region.</param>
		/// <param name="name">The name of the new state.</param>
		/// <returns>The newly created state.</returns>
		public static State<TInstance> CreateState<TInstance>(this Region<TInstance> region, string name) where TInstance : IInstance<TInstance> {
			return new State<TInstance>(name, region);
		}

		/// <summary>
		/// Creates a new state as a child of a state.
		/// </summary>
		/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
		/// <param name="state">The parent state.</param>
		/// <param name="name">The name of the new state.</param>
		/// <returns>The newly created state.</returns>
		/// <remarks>The newly created state will be created in the default region of the parent state. The default region will be created as required.</remarks>
		public static State<TInstance> CreateState<TInstance>(this State<TInstance> state, string name) where TInstance : IInstance<TInstance> {
			return new State<TInstance>(name, state);
		}

		/// <summary>
		/// Creates a new final state as a child of a region.
		/// </summary>
		/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
		/// <param name="region">The parent region.</param>
		/// <param name="name">The name of the new final state.</param>
		/// <returns>The newly created final state.</returns>
		public static FinalState<TInstance> CreateFinalState<TInstance>(this Region<TInstance> region, string name) where TInstance : IInstance<TInstance> {
			return new FinalState<TInstance>(name, region);
		}

		/// <summary>
		/// Creates a new final state as a child of a state.
		/// </summary>
		/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
		/// <param name="state">The parent state.</param>
		/// <param name="name">The name of the new final state.</param>
		/// <returns>The newly created final state.</returns>
		/// <remarks>The newly created final state will be created in the default region of the parent state. The default region will be created as required.</remarks>
		public static FinalState<TInstance> CreateFinalState<TInstance>(this State<TInstance> state, string name) where TInstance : IInstance<TInstance> {
			return new FinalState<TInstance>(name, state);
		}
	}
}