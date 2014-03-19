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
using System.Diagnostics;
using System.Linq;

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// A continuation transition between states or pseudo states within a state machine.
	/// </summary>
	/// <remarks>
	/// Continuation transitions are tested for after sucessful entry to pseudo states or completed states.
	/// </remarks>
	public partial class Transition<TState>
	{
		/// <summary>
		/// An else continuation transition; used as the default path from choice or junction pseudo states
		/// </summary>
		public sealed class Else : Transition<TState>
		{
			/// <summary>
			/// Creates an else completion transition between pseudo states.
			/// </summary>
			/// <param name="source">The source pseudo state.</param>
			/// <param name="target">The target pseudo state.</param>
			public Else( PseudoState<TState> source, PseudoState<TState> target )
				: base( source, target, False )
			{
				Trace.Assert( source.Kind == PseudoStateKind.Choice || source.Kind == PseudoStateKind.Junction, "Else can only originate from choice or junction pseudo states" );
			}

			/// <summary>
			/// Creates an else completion transition from a pseudo state to a state.
			/// </summary>
			/// <param name="source">The source pseudo state.</param>
			/// <param name="target">The target state.</param>
			public Else( PseudoState<TState> source, SimpleState<TState> target )
				: base( source, target, False )
			{
				Trace.Assert( source.Kind == PseudoStateKind.Choice || source.Kind == PseudoStateKind.Junction, "Else can only originate from choice or junction pseudo states" );
			}
		}
	}
}
