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

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// A transient state within a pseudo state model.
	/// </summary>
	public sealed class PseudoState<TState> : Element<TState> where TState : IState<TState>
	{
		private readonly ICollection<Transition<TState>> completions = new HashSet<Transition<TState>>();

		/// <summary>
		/// The kind of the pseudo state.
		/// </summary>
		public readonly PseudoStateKind Kind;

		/// <summary>
		/// Creates a pseudo state within an owning region.
		/// </summary>
		/// <param name="name">The name of the pseudo state.</param>
		/// <param name="kind">The kind of the pseudo state.</param>
		/// <param name="owner">The owenr of the pseudo state.</param>
		public PseudoState( String name, PseudoStateKind kind, Region<TState> owner )
			: base( name, owner )
		{
			this.Kind = kind;

			if( this.Kind.IsInitial() )
			{
				if( owner.initial != null )
					throw new Exception( "Region can have only one initial PseudoState: " + owner );

				owner.initial = this;
			}
		}

		/// <summary>
		/// Creates a pseudo state within an owning composite state.
		/// </summary>
		/// <param name="name">The name of the pseudo state.</param>
		/// <param name="kind">The kind of the pseudo state.</param>
		/// <param name="owner">The owenr of the pseudo state.</param>
		public PseudoState( String name, PseudoStateKind kind, CompositeState<TState> owner )
			: base( name, owner )
		{
			this.Kind = kind;

			if( this.Kind.IsInitial() )
			{
				if( owner.initial != null )
					throw new Exception( "Region can have only one initial PseudoState: " + owner );

				owner.initial = this;
			}
		}

		internal void Add( Transition<TState> completion )
		{
			Trace.Assert( !( this.Kind.IsInitial() && completions.Count != 0 ), "initial pseudo states can have at most one outbound completion transition" );

			this.completions.Add( completion );
		}

		internal override void EndEntry( TState state, Boolean deepHistory )
		{
			if( this.Kind == PseudoStateKind.Terminate )
				state.IsTerminated = true;
			else
				this.Kind.Completion<TState>( state, completions ).Traverse( state, deepHistory );
		}
	}
}
