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
namespace Steelbreeze.Behavior
{
	/// <summary>
	/// Base class for Visitors.
	/// </summary>
	/// <typeparam name="TContext">The type of the context to pass while visiting.</typeparam>
	public abstract class Visitor<TContext>
	{
		/// <summary>
		/// Visit a state machine node.
		/// </summary>
		/// <param name="node">The state machine node being visited.</param>
		/// <param name="context">The context passed in.</param>
		/// <returns>The context to pass on to sibling Regions.</returns>
		virtual public TContext Visit( StateMachineBase node, TContext context ) { return context; }

		/// <summary>
		/// Visit a Region.
		/// </summary>
		/// <param name="region">The Region being visited.</param>
		/// <param name="context">The context passed in.</param>
		/// <returns>The context to pass on to sibling Regions.</returns>
		virtual public TContext Visit( Region region, TContext context ) { return context; }

		/// <summary>
		/// Visit a Vertex.
		/// </summary>
		/// <param name="vertex">The PseudoState being visited.</param>
		/// <param name="context">The context passed in.</param>
		/// <returns>The context to pass on to sibling Vertices.</returns>
		virtual public TContext Visit( Vertex vertex, TContext context ) { return context; }

		/// <summary>
		/// Visit a PseudoState.
		/// </summary>
		/// <param name="pseudoState">The PseudoState being visited.</param>
		/// <param name="context">The context passed in.</param>
		/// <returns>The context to pass on to sibling Vertices.</returns>
		virtual public TContext Visit( PseudoState pseudoState, TContext context ) { return context; }

		/// <summary>
		/// Visit a State.
		/// </summary>
		/// <param name="state">State being visited.</param>
		/// <param name="context">The context passed in.</param>
		/// <returns>The context to pass on to sibling Vertices.</returns>
		virtual public TContext Visit( StateBase state, TContext context ) { return context; }

		/// <summary>
		/// Visit a State.
		/// </summary>
		/// <param name="state">State being visited.</param>
		/// <param name="context">The context passed in.</param>
		/// <returns>The context to pass on to sibling Vertices.</returns>
		virtual public TContext Visit( State state, TContext context ) { return context; }

		/// <summary>
		/// Visit a FinalState.
		/// </summary>
		/// <param name="finalState">The PseudoState being visited.</param>
		/// <param name="context">The context passed in.</param>
		/// <returns>The context to pass on to sibling Vertices.</returns>
		virtual public TContext Visit( FinalState finalState, TContext context ) { return context; }
	}
}