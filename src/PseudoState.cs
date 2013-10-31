// Copyright © 2013 Steelbreeze Limited.
// This file is part of state.cs.
//
// state.cs is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published
// by the Free Software Foundation, either version 3 of the License,
// or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// A transient state within a pseudo state model.
	/// </summary>
	public class PseudoState : Element
	{
		internal ICollection<Transition> completions;

		/// <summary>
		/// The kind of the pseudo state.
		/// </summary>
		public PseudoStateKind Kind { get; private set; }

		/// <summary>
		/// Creates a pseudo state within an owning region.
		/// </summary>
		/// <param name="name">The name of the pseudo state.</param>
		/// <param name="kind">The kind of the pseudo state.</param>
		/// <param name="owner">The owenr of the pseudo state.</param>
		public PseudoState( String name, PseudoStateKind kind, Region owner )
			: base( name, owner )
		{
			this.Kind = kind;

			if( this.Kind.IsInitial() )
				owner.initial = this;
		}

		/// <summary>
		/// Creates a pseudo state within an owning composite state.
		/// </summary>
		/// <param name="name">The name of the pseudo state.</param>
		/// <param name="kind">The kind of the pseudo state.</param>
		/// <param name="owner">The owenr of the pseudo state.</param>
		public PseudoState( String name, PseudoStateKind kind, CompositeState owner )
			: base( name, owner )
		{
			this.Kind = kind;

			if( this.Kind.IsInitial() )
				owner.initial = this;
		}

		internal override void BeginEnter( IState context )
		{
			base.BeginEnter( context );

			if( this.Kind == PseudoStateKind.Terminated )
				context.IsTerminated = true;
		}

		internal override void EndEnter( IState context, Boolean deepHistory )
		{
			this.Kind.Completion( completions ).Traverse( context, deepHistory );
		}
	}
}
