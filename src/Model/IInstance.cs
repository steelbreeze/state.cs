/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
namespace Steelbreeze.StateMachines.Model {
	/// <summary>
	/// Interface for state machine instance types to inherit from.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine interface.</typeparam>
	/// <remarks>While this interface can be directly implemented, it is recomended to use StateMachineInstance</remarks>
	public interface IInstance<TInstance> where TInstance : IInstance<TInstance> {
		/// <summary>
		/// Flag indicating if the state machine instance has been terminated.
		/// </summary>
		/// <remarks>A state machine instance is terminated as soon as it reaches a terminate pseudo state.</remarks>
		bool IsTerminated { get; set; }

		/// <summary>
		/// Sets or updates the current active child state of a region.
		/// </summary>
		/// <param name="region">The region to set the current active child state for.</param>
		/// <param name="state">The state to set as the current active child of the region.</param>
		void SetCurrent (Region<TInstance> region, State<TInstance> state);

		/// <summary>
		/// Retreives the last known active child state of a region.
		/// </summary>
		/// <param name="region">The region to get the current active child state for.</param>
		/// <returns>The last known active child state of the region or null if the region has never been entered.</returns>
		/// <remarks>The last known child state of the region is maintained in order to correctly manage history semantics.</remarks>
		State<TInstance> GetCurrent (Region<TInstance> region);
	}
}