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
using System.Diagnostics;

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// Represents an element within a state machine heirarchy
	/// </summary>
	public abstract class Element<TState> where TState : IState<TState>
	{
		/// <summary>
		/// The name of the element.
		/// </summary>
		public readonly String Name;

		/// <summary>
		/// The owning parent element of the element
		/// </summary>
		public readonly Element<TState> Owner;

		/// <summary>
		/// Returns the fully qualified name of the element
		/// </summary>
		public String QualifiedName { get { return this.Owner != null ? this.Owner.QualifiedName + "." + this.Name : this.Name; } }

		internal Element( String name, Element<TState> owner )
		{
			this.Name = name;
			this.Owner = owner;
		}

		internal virtual void BeginExit( TState state ) { }

		internal virtual void EndExit( TState state )
		{
			Debug.WriteLine( String.Format("Leave: {0} ({1})", this.QualifiedName, state ) );

			state.SetActive( this, false );
		}

		internal virtual void BeginEntry( TState state )
		{
			if( state.GetActive( this ) ) // NOTE: this check is required to cater for edge cases involving external transitions between orthogonal regions; we like to keep the entry/exit count for elements the same, so we exit an element that active before reentering it
			{
				BeginExit( state );
				EndExit( state );
			}

			Debug.WriteLine( String.Format("Enter: {0} ({1})", this.QualifiedName, state ) );

			state.SetActive( this, true );
		}

		internal virtual void EndEntry( TState state, bool deepHistory ) { }

		/// <summary>
		/// Returns the name of the element.
		/// </summary>
		/// <returns>The fully qualified name of the element.</returns>
		public override string ToString()
		{
			return this.Name;
		}
	}
}
