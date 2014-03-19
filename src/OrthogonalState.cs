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
	/// An orthogonal state is a state that contains regions.
	/// </summary>
	/// <remarks>
	/// Orthogonal states allow seperation of mutually exclusive child states allowing them to independantly respond to messages. 
	/// </remarks>
	public class OrthogonalState<TState> : SimpleState<TState> where TState : IState<TState>
	{
		internal readonly ICollection<Region<TState>> regions = new HashSet<Region<TState>>();

		/// <summary>
		/// Creates an orthogonal state within an owning (parent) region.
		/// </summary>
		/// <param name="name">The name of the orthogonal state.</param>
		/// <param name="owner">The owning (parent) region.</param>
		/// <remarks>
		/// An orthogonal state is a container of regions within a state machine model; it can be used as a root state machine.
		/// </remarks>
		public OrthogonalState( String name, Region<TState> owner ) : base( name, owner ) { }

		/// <summary>
		/// Creates an orthogonal state within an owning (parent) composite state.
		/// </summary>
		/// <param name="name">The name of the orthogonal state.</param>
		/// <param name="owner">The owning (parent) composite state.</param>
		/// <remarks>
		/// An orthogonal state is a container of regions within a state machine model; it can be used as a root state machine.
		/// </remarks>
		public OrthogonalState( String name, CompositeState<TState> owner ) : base( name, owner ) { }

		/// <summary>
		/// Creates a new child Region within the StateMachine
		/// </summary>
		/// <param name="name">The name of the Region</param>
		/// <returns>The new child Region</returns>
		public Region<TState> CreateRegion( String name )
		{
			return new Region<TState>( name, this );
		}

		internal override bool IsComplete( TState state )
		{
			if( !state.IsTerminated )
				foreach( var region in this.regions )
					if( !region.IsComplete( state ) )
						return false;

			return true;
		}

		internal override void BeginExit( TState state )
		{
			foreach( var region in this.regions )
			{
				if( state.GetActive( region ) )
				{
					region.BeginExit( state );
					region.EndExit( state );
				}
			}
		}

		internal override void EndEntry( TState state, bool deepHistory )
		{
			foreach( var region in this.regions )
			{
				region.BeginEntry( state );
				region.EndEntry( state, deepHistory );
			}

			base.EndEntry( state, deepHistory );
		}

		internal override bool Process( TState state, object message )
		{
			if( state.IsTerminated )
				return false;

			return base.Process( state, message ) || regions.Aggregate( false, ( result, region ) => region.Process( state, message ) || result );
		}
	}
}
