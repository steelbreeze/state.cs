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
			private class UncommittedStateBase
			{
				public Boolean IsActive; // NOTE: make nullable if adding any more properties

				public void Commit( StateBase stateBase )
				{
					stateBase.IsActive = IsActive;
				}
			}

			private class UncommittedRegion
			{
				public Nullable<Boolean> IsActive;
				public StateBase Current;

				public void Commit( Region region )
				{
					if( IsActive != null )
						region.IsActive = IsActive.Value;

					if( Current != null )
						region.Current = Current;
				}
			}

			private Dictionary<Region, UncommittedRegion> uncommittedRegions = new Dictionary<Region, UncommittedRegion>();
			private Dictionary<StateBase, UncommittedStateBase> uncommittedStateBases = new Dictionary<StateBase, UncommittedStateBase>();

			public StateBase GetCurrent( Region region )
			{
				UncommittedRegion uncommittedRegion;

				return uncommittedRegions.TryGetValue( region, out uncommittedRegion ) ? ( uncommittedRegion.Current ?? region.Current ) : region.Current;
			}

			public Boolean GetActive( Region region )
			{
				UncommittedRegion uncommittedRegion;

				return uncommittedRegions.TryGetValue( region, out uncommittedRegion ) ? ( uncommittedRegion.IsActive ?? region.IsActive ) : region.IsActive;
			}

			public Boolean GetActive( StateBase stateBase )
			{
				UncommittedStateBase uncommittedStateBase;

				return uncommittedStateBases.TryGetValue( stateBase, out uncommittedStateBase ) ? uncommittedStateBase.IsActive : stateBase.IsActive;
			}

			public void SetActive( Region region, Boolean value )
			{
				UncommittedRegion uncommittedRegion;

				if( !uncommittedRegions.TryGetValue( region, out uncommittedRegion ) )
					uncommittedRegions.Add( region, uncommittedRegion = new UncommittedRegion() );

				uncommittedRegion.IsActive = value;
			}

			public void SetActive( StateBase stateBase, Boolean value )
			{
				UncommittedStateBase uncommittedStateBase;

				if( !uncommittedStateBases.TryGetValue( stateBase, out uncommittedStateBase ) )
					uncommittedStateBases.Add( stateBase, uncommittedStateBase = new UncommittedStateBase() );

				uncommittedStateBase.IsActive = value;
			}

			public void SetCurrent( Region region, StateBase value )
			{
				UncommittedRegion uncommittedRegion;

				if( !uncommittedRegions.TryGetValue( region, out uncommittedRegion ) )
					uncommittedRegions.Add( region, uncommittedRegion = new UncommittedRegion() );

				uncommittedRegion.Current = value;
			}

			public void Commit()
			{
				foreach( var uncommitedRegion in uncommittedRegions )
					uncommitedRegion.Value.Commit( uncommitedRegion.Key );

				foreach( var uncommittedStateBase in uncommittedStateBases )
					uncommittedStateBase.Value.Commit( uncommittedStateBase.Key );

				Rollback();
			}

			public void Rollback()
			{
				uncommittedRegions.Clear();
				uncommittedStateBases.Clear();
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