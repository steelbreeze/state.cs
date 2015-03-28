/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// Set of extension methods to facilitate easier model building
	/// </summary>
	public static class Extensions {
		/// <summary>
		/// Creates a new instance of the Region class under the parent State.
		/// </summary>
		/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
		/// <param name="state">The parent state of the new Region.</param>
		/// <param name="name">The name of the Region.</param>
		/// <returns>The newly created Region.</returns>
		public static Region<TInstance> CreateRegion<TInstance> (this State<TInstance> state, String name) where TInstance : class, IActiveStateConfiguration<TInstance> {
			return new Region<TInstance> (name, state);
		}

		/// <summary>
		/// Creates a new instance of the PseudoState class under the parent Region.
		/// </summary>
		/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
		/// <param name="region">The parent region of the new PseudoState.</param>
		/// <param name="name">The name of the PseudoState.</param>
		/// <param name="kind">The kind of the PseudoState.</param>
		/// <returns>The newly created PseudoState.</returns>
		public static PseudoState<TInstance> CreatePseudoState<TInstance> (this Region<TInstance> region, String name, PseudoStateKind kind = PseudoStateKind.Initial) where TInstance : class, IActiveStateConfiguration<TInstance> {
			return new PseudoState<TInstance> (name, region, kind);
		}

		/// <summary>
		/// Creates a new instance of the PseudoState class under the parent State.
		/// </summary>
		/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
		/// <param name="state">The parent state of the new PseudoState.</param>
		/// <param name="name">The name of the PseudoState.</param>
		/// <param name="kind">The kind of the PseudoState.</param>
		/// <returns>The newly created PseudoState.</returns>
		/// <remarks>Note that the PseudoState will be created under the parent state's default region.</remarks>
		public static PseudoState<TInstance> CreatePseudoState<TInstance> (this State<TInstance> state, String name, PseudoStateKind kind = PseudoStateKind.Initial) where TInstance : class, IActiveStateConfiguration<TInstance> {
			return new PseudoState<TInstance> (name, state, kind);
		}

		/// <summary>
		/// Creates a new instance of the State class under the parent Region.
		/// </summary>
		/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
		/// <param name="region">The parent region of the new State.</param>
		/// <param name="name">The name of the State.</param>
		/// <returns>The newly created State.</returns>
		public static State<TInstance> CreateState<TInstance> (this Region<TInstance> region, String name) where TInstance : class, IActiveStateConfiguration<TInstance> {
			return new State<TInstance> (name, region);
		}

		/// <summary>
		/// Creates a new instance of the State class under the parent State.
		/// </summary>
		/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
		/// <param name="state">The parent state of the new State.</param>
		/// <param name="name">The name of the State.</param>
		/// <returns>The newly created State.</returns>
		/// <remarks>Note that the State will be created under the parent state's default region.</remarks>
		public static State<TInstance> CreateState<TInstance> (this State<TInstance> state, String name) where TInstance : class, IActiveStateConfiguration<TInstance> {
			return new State<TInstance> (name, state);
		}

		/// <summary>
		/// Creates a new instance of the FinalState class under the parent Region.
		/// </summary>
		/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
		/// <param name="region">The parent region of the new FinalState.</param>
		/// <param name="name">The name of the FinalState.</param>
		/// <returns>The newly created FinalState.</returns>
		public static FinalState<TInstance> CreateFinalState<TInstance> (this Region<TInstance> region, String name) where TInstance : class, IActiveStateConfiguration<TInstance> {
			return new FinalState<TInstance> (name, region);
		}

		/// <summary>
		/// Creates a new instance of the FinalState class under the parent State.
		/// </summary>
		/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
		/// <param name="state">The parent state of the new FinalState.</param>
		/// <param name="name">The name of the FinalState.</param>
		/// <returns>The newly created FinalState.</returns>
		/// <remarks>Note that the FinalState will be created under the parent state's default region.</remarks>
		public static FinalState<TInstance> CreateFinalState<TInstance> (this State<TInstance> state, String name) where TInstance : class, IActiveStateConfiguration<TInstance> {
			return new FinalState<TInstance> (name, state);
		}
	}
}
