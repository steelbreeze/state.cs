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

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// A region within a state machine model.
	/// </summary>
	/// <remarks>
	/// A region is a container for states and pseudo states.
	/// </remarks>
	public class Region<TState> : Element<TState> where TState : IState<TState>
	{
		internal PseudoState<TState> initial;

		/// <summary>
		/// Creates a new region within a state machine.
		/// </summary>
		/// <param name="name">The name of the region.</param>
		/// <param name="owner">The parent state machine.</param>
		/// <remarks>
		/// A state machine is a container of states and pseudo states within a state machine model; it can be used as a root state machine.
		/// </remarks>
		public Region( String name, StateMachine<TState> owner )
			: base( name, owner )
		{
			if( owner != null )
				owner.regions.Add( this );
		}

		/// <summary>
		/// Creates a new region within an orthogonal state.
		/// </summary>
		/// <param name="name">The name of the region.</param>
		/// <param name="owner">The optional parent orthogonal state.</param>
		/// <remarks>
		/// A region is a container of states and pseudo states within a state machine model; it can be used as a root state machine.
		/// </remarks>
		public Region( String name, OrthogonalState<TState> owner = null )
			: base( name, owner )
		{
			if( owner != null )
				owner.regions.Add( this );
		}

		/// <summary>
		/// Creates a new child PseudoState within the Region
		/// </summary>
		/// <param name="name">The name of the PseudoState</param>
		/// <param name="kind">The kind of the PseudoState</param>
		/// <returns>The new child PseudoState</returns>
		public PseudoState<TState> CreatePseudoState( String name, PseudoStateKind kind )
		{
			return new PseudoState<TState>( name, kind, this );
		}

		/// <summary>
		/// Creates a new child SimpleState within the Region
		/// </summary>
		/// <param name="name">The name of the SimpleState</param>
		/// <returns>The new child SimpleState</returns>
		public SimpleState<TState> CreateSimpleState( String name )
		{
			return new SimpleState<TState>( name, this );
		}

		/// <summary>
		/// Creates a new child CompositeState within the Region
		/// </summary>
		/// <param name="name">The name of the CompositeState</param>
		/// <returns>The new child CompositeState</returns>
		public CompositeState<TState> CreateCompositeState( String name )
		{
			return new CompositeState<TState>( name, this );
		}

		/// <summary>
		/// Creates a new child OrthogonalState within the Region
		/// </summary>
		/// <param name="name">The name of the OrthogonalState</param>
		/// <returns>The new child Orthogonaltate</returns>
		public OrthogonalState<TState> CreateOrthogonalState( String name )
		{
			return new OrthogonalState<TState>( name, this );
		}

		/// <summary>
		/// Creates a new child FinalState within the Region
		/// </summary>
		/// <param name="name">The name of the FinalState</param>
		/// <returns>The new child FinalState</returns>
		public FinalState<TState> CreateFinalState( String name )
		{
			return new FinalState<TState>( name, this );
		}

		/// <summary>
		/// Determines if a region is completed.
		/// </summary>
		/// <param name="state">The state machine state to test completeness for.</param>
		/// <returns>A boolean value indicating that the region is completed.</returns>
		/// <remarks>A region is deemed to be completed when its current child state is a final state.</remarks>
		public Boolean IsComplete( TState state )
		{
			var current = state.GetCurrent( this );

			return state.IsTerminated || current == null || current is FinalState<TState> || state.GetActive( current ) == false;
		}

		/// <summary>
		/// Initialises the state machine state context with its initial state.
		/// </summary>
		/// <param name="state">The state machine state context to initialise.</param>
		public void Initialise( TState state )
		{
			BeginEntry( state );
			EndEntry( state, false );
		}

		internal override void BeginExit( TState state )
		{
			var current = state.GetCurrent( this );

			if( current != null )
			{
				current.BeginExit( state );
				current.EndExit( state );
			}
		}

		internal override void EndEntry( TState state, Boolean deepHistory )
		{
			Element<TState> current = null;

			if( deepHistory || this.initial.Kind.IsHistory() )
				current = state.GetCurrent( this );

			if( current == null )
				current = initial;

			current.BeginEntry( state );
			current.EndEntry( state, deepHistory || this.initial.Kind == PseudoStateKind.DeepHistory );
		}

		/// <summary>
		/// Attempts to process a message against a region.
		/// </summary>
		/// <param name="state">The state machine state.</param>
		/// <param name="message">The message to evaluate.</param>
		/// <returns>A boolean indicating if the message caused a state change.</returns>
		public Boolean Process( TState state, Object message )
		{
			if( state.IsTerminated )
				return false;

			return state.GetActive( this ) && state.GetCurrent( this ).Process( state, message );
		}
	}
}
