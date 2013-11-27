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

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// A composite state is a state that contains states and pseudo states.
	/// </summary>
	public class CompositeState : SimpleState
	{
		internal PseudoState initial;

		/// <summary>
		/// Creates a composite state within an owning (parent) region.
		/// </summary>
		/// <param name="name">The name of the composite state.</param>
		/// <param name="owner">The owning (parent) region.</param>
		/// <remarks>
		/// A composite state is a container of states and pseudo states within a state machine model; it can be used as a root state machine.
		/// </remarks>
		public CompositeState( String name, Region owner ) : base( name, owner ) { }

		/// <summary>
		/// Creates a composite state within an owning (parent) composite state.
		/// </summary>
		/// <param name="name">The name of the composite state.</param>
		/// <param name="owner">The owning (parent) composite state.</param>
		/// <remarks>
		/// A composite state is a container of states and pseudo states within a state machine model; it can be used as a root state machine.
		/// </remarks>
		public CompositeState( String name, CompositeState owner ) : base( name, owner ) { }

		internal override bool IsComplete( IState context )
		{
			return context.IsTerminated || context.GetCurrent( this ) is FinalState;
		}

		internal override void BeginExit( IState context )
		{
			var current = context.GetCurrent( this );

			if( current != null )
			{
				current.BeginExit( context );
				current.EndExit( context );
			}
		}

		internal override void EndEnter( IState context, bool deepHistory )
		{
			Element current = null;

			if( deepHistory || this.initial.Kind.IsHistory() )
				current = context.GetCurrent( this );

			if( current == null )
				current = initial;

			current.BeginEnter( context );
			current.EndEnter( context, deepHistory || this.initial.Kind == PseudoStateKind.DeepHistory );

			base.EndEnter( context, deepHistory );
		}

		internal override Boolean Process( IState context, Object message )
		{
			return base.Process( context, message ) || context.GetCurrent( this ).Process( context, message );
		}
	}
}
