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
	/// A Region is an orthogonal part of either a CompositeState or a StateMachine. It contains states and transitions.
	/// </summary>
	public class Region : StateMachineBase
	{
		internal HashSet<Vertex> vertices = new HashSet<Vertex>();
		internal PseudoState initial = null;
		private Object sync = new Object();

		/// <summary>
		/// The Region's parent State
		/// </summary>
		public State Parent { get; private set; }

		/// <summary>
		/// The name of Region
		/// </summary>
		public String Name { get; private set; }

		/// <summary>
		/// A flag indicating that the Region is active (entered, but not yet exited)
		/// </summary>
		public Boolean IsActive { get; internal set; }

		/// <summary>
		/// The current state of the region.
		/// </summary>
		/// <remarks>
		/// Note that if the region is no longer active, this represents the last known active state of the region.
		/// </remarks>
		public StateBase Current { get; internal set; } // internal set allows deserialisers to restore current active state

		/// <summary>
		/// Returns true when the Region is completed.
		/// </summary>
		public Boolean IsComplete { get { return Current is FinalState; } }

		/// <summary>
		/// Creates a Region.
		/// </summary>
		/// <param name="name">The name of the Region.</param>
		/// <param name="parent">The parent CompositeState.</param>
		public Region( String name, State parent = null )
		{
			Trace.Assert( name != null, "Region name must be provided" );

			Name = name;

			if( ( this.Parent = parent ) != null )
				( parent.regions ?? ( parent.regions = new HashSet<Region>() ) ).Add( this );
		}

		/// <summary>
		/// Initialises a node to its initial state.
		/// <param name="transaction">An optional transaction that the process operation will participate in.</param>
		/// </summary>
		public void Initialise( ITransaction transaction = null )
		{
			lock( sync )
			{
				Boolean transactionOwner = transaction == null;

				if( transactionOwner )
					transaction = TransactionManager.Default();

				Initialise( transaction, false );

				if( transactionOwner )
					transaction.Commit();
			}
		}

		public void Reset( ITransaction transaction = null )
		{
			lock( sync )
			{
				var transactionOwner = transaction == null;

				if( transactionOwner )
					transaction = TransactionManager.Default();

				foreach( var state in vertices.OfType<StateBase>() )
					state.Reset( transaction );

				transaction.SetActive( this, false );
				transaction.SetCurrent( this, null );

				if( transactionOwner )
					transaction.Commit();
			}
		}

		internal void Initialise( ITransaction transaction, Boolean deepHistory )
		{
			BeginEnter( transaction );

			var vertex = deepHistory || initial.Kind.IsHistory ? transaction.GetCurrent( this ) as Vertex ?? initial : initial;

			vertex.Initialise( transaction, deepHistory || ( initial.Kind == PseudoStateKind.DeepHistory ) );
		}

		override internal void OnExit( ITransaction transaction )
		{
			if( transaction.GetCurrent( this ) != null )
				Current.OnExit( transaction );

			transaction.SetActive( this, false );

			base.OnExit( transaction );
		}

		internal override void BeginEnter( ITransaction transaction )
		{
			if( transaction.GetActive( this ) )
				OnExit( transaction );

			base.BeginEnter( transaction );

			transaction.SetActive( this, true );
		}

		/// <summary>
		/// Attempts to process a message to facilitate state transitions
		/// </summary>
		/// <param name="message">The message to process.</param>
		/// <param name="transaction">An optional transaction that the process operation will participate in.</param>
		/// <returns>A Boolean indicating if the message was processed.</returns>
		public Boolean Process( Object message, ITransaction transaction = null )
		{
			lock( sync )
			{
				var transactionOwner = transaction == null;

				if( transactionOwner )
					transaction = TransactionManager.Default();

				var processed = Current.Process( message );

				if( transactionOwner )
					transaction.Commit();

				return processed;
			}
		}

		/// <summary>
		/// Accepts a Visitor object and visits all child Vertices.
		/// </summary>
		/// <typeparam name="TContext">The type of the context to pass while visiting the CompositeState.</typeparam>
		/// <param name="visitor">The Visitor object.</param>
		/// <param name="context">The context to pass while visiting the CompositeState.</param>
		/// <returns>Context to pass on to sibling Regions within the parent CompositeState.</returns>
		override public TContext Accept<TContext>( Visitor<TContext> visitor, TContext context = default( TContext ) )
		{
			context = visitor.Visit( this, base.Accept( visitor, context ) );

			foreach( var child in vertices )
				child.Accept( visitor, context );

			return context;
		}

		/// <summary>
		/// Displays the fully qualified name of the Region or Vertex
		/// </summary>
		/// <returns></returns>
		override public String ToString()
		{
			return Parent == null ? Name : Parent + "." + Name;
		}
	}
}