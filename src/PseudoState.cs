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
	public class PseudoState : IVertex
	{
		IElement IElement.Owner { get { return owner; } }

		private readonly IRegion owner;

		internal ICollection<Completion> completions { get; set; }

		/// <summary>
		/// The name of the pseudo state.
		/// </summary>
		public String Name { get; private set; }

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
		{
			this.Name = name;
			this.Kind = kind;
			this.owner = owner;

			if( this.Kind.IsInitial() )
				this.owner.Initial = this;
		}

		/// <summary>
		/// Creates a pseudo state within an owning composite state.
		/// </summary>
		/// <param name="name">The name of the pseudo state.</param>
		/// <param name="kind">The kind of the pseudo state.</param>
		/// <param name="owner">The owenr of the pseudo state.</param>
		public PseudoState( String name, PseudoStateKind kind, CompositeState owner )
		{
			this.Name = name;
			this.Kind = kind;
			this.owner = owner;

			if( this.Kind.IsInitial() )
				this.owner.Initial = this;
		}

		void IElement.OnExit( IState context )
		{
			Debug.WriteLine( this, "Leave" );

			context.SetActive( this, false );
		}

		void IElement.OnBeginEnter( IState context )
		{
			IVertex vertex = this;

			if( context.GetActive( this ) )
				vertex.OnExit( context );

			Debug.WriteLine( this, "Enter" );

			context.SetActive( this, true );

			if( this.Kind == PseudoStateKind.Terminated )
				context.IsTerminated = true;
		}

		void IVertex.OnEndEnter( IState context, Boolean deepHistory )
		{
			var completion = Kind.Completion( completions );

			if( completion != null )
				completion.Traverse( context, deepHistory );
		}

		/// <summary>
		/// Returns the fully qualified name of the pseudo state.
		/// </summary>
		/// <returns>The fully qualified name of the pseudo state.</returns>
		public override string ToString()
		{
			return this.Ancestors().Select( ancestor => ancestor.Name ).Aggregate( ( right, left ) => left + "." + right );
		}
	}
}
