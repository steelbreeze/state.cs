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
using System.ComponentModel;

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// A final state is a state that denotes its parent region or composite state is complete.
	/// </summary>
	public sealed class FinalState<TState> : SimpleState<TState> where TState : IState<TState>
	{
		/// <summary>
		/// The final state's entry action (do not set this)
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete( "Entry actions are not permitted for FinalState", true )]
		new public Action Entry { get { return null; } set { throw new Exception( "FinalState cannot have an entry action" ); } }

		/// <summary>
		/// The final state's exit action (do not set this)
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		[Obsolete( "Exit actions are not permitted for FinalState", true )]
		new public Action Exit { get { return null; } set { throw new Exception( "FinalState cannot have an exit action" ); } }

		/// <summary>
		/// Creates a final state within an owning (parent) region.
		/// </summary>
		/// <param name="name">The name of the final state.</param>
		/// <param name="owner">The owning (parent) region.</param>
		public FinalState( String name, Region<TState> owner ) : base( name, owner ) { }

		/// <summary>
		/// Creates a final state within an owning (parent) composite state.
		/// </summary>
		/// <param name="name">The name of the final state.</param>
		/// <param name="owner">The owning (parent) composite state.</param>
		public FinalState( String name, CompositeState<TState> owner ) : base( name, owner ) { }
	}
}
