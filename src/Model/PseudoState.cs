/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
namespace Steelbreeze.StateMachines.Model {
	/// <summary>
	/// Represents a pseudo state within a region.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	/// <remarks>A pseudo state is a transitory vertex within a region; when entered, it will automatically exit via the appropriate outgoing transition. The behavior of a pseudo state varies according to its kind; see the PseudoStateKind documentation for details.</remarks>
	public class PseudoState<TInstance> : Vertex<TInstance> where TInstance : IInstance<TInstance> {
		/// <summary>
		/// The kind of the pseudo state defining its use and behavior.
		/// </summary>
		public readonly PseudoStateKind Kind;

		/// <summary>
		/// Creates a new pseudo state as a child of a region.
		/// </summary>
		/// <param name="region">The parent region.</param>
		/// <param name="name">The name of the new pseudo state.</param>
		/// <param name="kind">The kind of the new pseudo state.</param>
		/// <returns>The newly created pseudo state.</returns>
		public PseudoState (string name, Region<TInstance> region, PseudoStateKind kind = PseudoStateKind.Initial)
			: base(name, region) {

			this.Kind = kind;
		}

		/// <summary>
		/// Flag indicating that the pseduo state kind is one of the history pseudo state kinds.
		/// </summary>
		/// <remarks>The history pseudo state kinds are: deep history and shallow history.</remarks>
		public bool IsHistory {
			get {
				return this.Kind == PseudoStateKind.DeepHistory || this.Kind == PseudoStateKind.ShallowHistory;
			}
		}

		/// <summary>
		/// Flad indicating that hte pseudo state kind is one of the initial pseudo state kinds.
		/// </summary>
		/// <remarks>The initial pseudo state kinds are: initial, deep history and shallow history.</remarks>
		public bool IsInitial {
			get {
				return this.Kind == PseudoStateKind.Initial || this.IsHistory;
			}
		}

		/// <summary>
		/// Accepts a visitor.
		/// </summary>
		/// <param name="visitor">The visitor to accept.</param>
		public override void Accept(Visitor<TInstance> visitor)
        {
            visitor.VisitPseudoState(this);
        }

		/// <summary>
		/// Accepts a visitor.
		/// </summary>
		/// <typeparam name="TArg">The type of the argument passed into the visitor.</typeparam>
		/// <param name="visitor">The visitor to accept.</param>
		/// <param name="arg">The argument to pass to each element visited.</param>
		public override void Accept<TArg>(Visitor<TInstance, TArg> visitor, TArg arg)
        {
            visitor.VisitPseudoState(this, arg);
        }
    }
}