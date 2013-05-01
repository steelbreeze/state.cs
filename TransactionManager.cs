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

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// The base class and factory for transactions.
	/// </summary>
	public static class TransactionManager
	{
		private static NullTransaction nullTransaction = new NullTransaction();

		/// <summary>
		/// The default transaction factory.
		/// </summary>
		/// <remarks>
		/// The default transaction factory creates deferred transactions for safety.
		/// This attribute can be set to Deferred or a custom delegate that creates custom transactions.
		/// </remarks>
		public static Func<TransactionBase> Default = Deferred;

		/// <summary>
		/// Creates a deferred transaction.
		/// </summary>
		/// <returns>A new deferred transaction</returns>
		/// <remarks>
		/// Deferred transactions update all States and Regions only on Commit.
		/// Deffered transactions are the safest transactions.
		/// </remarks>
		public static TransactionBase Deferred()
		{
			return new DeferredTransaction();
		}

		/// <summary>
		/// Creates a null transaction.
		/// </summary>
		/// <returns>A new null transaction</returns>
		/// <remarks>
		/// Null transactions update states and regions in real time.
		/// Null transactions are the most performant transactions.
		/// </remarks>
		public static TransactionBase Null()
		{
			return nullTransaction;
		}

		private sealed class DeferredTransaction : TransactionBase
		{
			// TODO: make more efficent - only one collection for region...
			private Dictionary<Region, StateBase> regionCurrent = new Dictionary<Region, StateBase>();
			private Dictionary<Region, Boolean> regionActive = new Dictionary<Region, bool>();
			private Dictionary<StateBase, Boolean> stateBase = new Dictionary<StateBase, bool>();

			public StateBase GetCurrent( Region region )
			{
				StateBase value;

				return regionCurrent.TryGetValue( region, out value ) ? value : region.Current;
			}

			public Boolean GetActive( Region region )
			{
				Boolean value;

				return regionActive.TryGetValue( region, out value ) ? value : region.IsActive;
			}

			public Boolean GetActive( StateBase state )
			{
				Boolean value;

				return stateBase.TryGetValue( state, out value ) ? value : state.IsActive;
			}

			public void SetActive( Region region, Boolean value )
			{
				if( !regionActive.ContainsKey( region ) )
					regionActive.Add( region, value );
				else
					regionActive[ region ] = value;
			}

			public void SetActive( StateBase state, Boolean value )
			{
				if( !stateBase.ContainsKey( state ) )
					stateBase.Add( state, value );
				else
					stateBase[ state ] = value;
			}

			public void SetCurrent( Region region, StateBase value )
			{
				if( !regionCurrent.ContainsKey( region ) )
					regionCurrent.Add( region, value );
				else
					regionCurrent[ region ] = value;
			}

			public void Commit()
			{
				foreach( var node in regionActive )
					node.Key.IsActive = node.Value;

				foreach( var node in stateBase )
					node.Key.IsActive = node.Value;

				foreach( var region in regionCurrent )
					region.Key.Current = region.Value;

				Rollback();
			}

			public void Rollback()
			{
				regionActive.Clear();
				stateBase.Clear();
				regionCurrent.Clear();
			}
		}

		private sealed class NullTransaction : TransactionBase
		{
			public StateBase GetCurrent( Region region )
			{
				return region.Current;
			}

			public Boolean GetActive( Region region )
			{
				return region.IsActive;
			}

			public Boolean GetActive( StateBase state )
			{
				return state.IsActive;
			}

			public void SetActive( Region region, Boolean value )
			{
				region.IsActive = value;
			}

			public void SetActive( StateBase state, Boolean value )
			{
				state.IsActive = value;
			}

			public void SetCurrent( Region region, StateBase value )
			{
				region.Current = value;
			}

			public void Commit()
			{
			}

			public void Rollback()
			{
			}
		}
	}
}