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
	/// A composite state is a state that contains states and pseudo states.
	/// </summary>
	public class CompositeState<TState> : SimpleState<TState> where TState : IState<TState>
	{
		// TODO: add a collection of elements (needed for serialisation)
		internal PseudoState<TState> initial;

		/// <summary>
		/// Creates a composite state within an owning (parent) region.
		/// </summary>
		/// <param name="name">The name of the composite state.</param>
		/// <param name="owner">The owning (parent) region.</param>
		/// <remarks>
		/// A composite state is a container of states and pseudo states within a state machine model; it can be used as a root state machine.
		/// </remarks>
		public CompositeState( String name, Region<TState> owner ) : base( name, owner ) { }

		/// <summary>
		/// Creates a composite state within an owning (parent) composite state.
		/// </summary>
		/// <param name="name">The name of the composite state.</param>
		/// <param name="owner">The owning (parent) composite state.</param>
		/// <remarks>
		/// A composite state is a container of states and pseudo states within a state machine model; it can be used as a root state machine.
		/// </remarks>
		public CompositeState( String name, CompositeState<TState> owner ) : base( name, owner ) { }

		/// <summary>
		/// Creates a new child PseudoState within the CompositeState
		/// </summary>
		/// <param name="name">The name of the PseudoState</param>
		/// <param name="kind">The kind of the PseudoState</param>
		/// <returns>The new child PseudoState</returns>
		public PseudoState<TState> CreatePseudoState( String name, PseudoStateKind kind )
		{
			return new PseudoState<TState>( name, kind, this );
		}

		/// <summary>
		/// Creates a new child SimpleState within the CompositeState
		/// </summary>
		/// <param name="name">The name of the SimpleState</param>
		/// <returns>The new child SimpleState</returns>
		public SimpleState<TState> CreateSimpleState( String name )
		{
			return new SimpleState<TState>( name, this );
		}

		/// <summary>
		/// Creates a new child CompositeState within the CompositeState
		/// </summary>
		/// <param name="name">The name of the CompositeState</param>
		/// <returns>The new child CompositeState</returns>
		public CompositeState<TState> CreateCompositeState( String name )
		{
			return new CompositeState<TState>( name, this );
		}

		/// <summary>
		/// Creates a new child OrthogonalState within the CompositeState
		/// </summary>
		/// <param name="name">The name of the OrthogonalState</param>
		/// <returns>The new child Orthogonaltate</returns>
		public OrthogonalState<TState> CreateOrthogonalState( String name )
		{
			return new OrthogonalState<TState>( name, this );
		}

		/// <summary>
		/// Creates a new child FinalState within the CompositeState
		/// </summary>
		/// <param name="name">The name of the FinalState</param>
		/// <returns>The new child FinalState</returns>
		public FinalState<TState> CreateFinalState( String name )
		{
			return new FinalState<TState>( name, this );
		}

		internal override bool IsComplete( TState state )
		{
			var current = state.GetCurrent( this );

			return state.IsTerminated || current == null || current is FinalState<TState> || state.GetActive( current ) == false;
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

		internal override void EndEntry( TState state, bool deepHistory )
		{
			Element<TState> current = null;

			if( deepHistory || this.initial.Kind.IsHistory() )
				current = state.GetCurrent( this );

			if( current == null )
				current = initial;

			current.BeginEntry( state );
			current.EndEntry( state, deepHistory || this.initial.Kind == PseudoStateKind.DeepHistory );

			base.EndEntry( state, deepHistory );
		}

		internal override Boolean Process( TState state, Object message )
		{
			if( state.IsTerminated )
				return false;

			var processed = base.Process( state, message ) || state.GetCurrent( this ).Process( state, message );

			// NOTE: this fixes an omission in all versions prior to v4.1.1 (should you get unexpected behavior, please investigate completion transtions from the state)
			if( processed == true )
				this.EvaluateCompletions( state, false );

			return processed;
		}
	}
}
