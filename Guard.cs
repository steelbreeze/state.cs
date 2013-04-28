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
	/// Standard guard conditions for completion transitions
	/// </summary>
	public static class Guard
	{
		/// <summary>
		/// A guard condition that always returns true.
		/// </summary>
		public static readonly Func<Boolean> True = () => true;

		/// <summary>
		/// A guard condition that always returns false.
		/// </summary>	
		public static readonly Func<Boolean> False = () => false;
		
		/// <summary>
		/// A guard condition used at Choice and Junction PseudoStates as a catch-all transtiion.
		/// </summary>
		public static readonly Func<Boolean> Else = () => false;
	}

	/// <summary>
	/// Standard guard conditions for message triggered transitions
	/// </summary>
	public static class Guard<TMessage>
	{
		/// <summary>
		/// A guard condition that always returns true.
		/// </summary>
		public static readonly Func<TMessage, Boolean> True = message => true;

		/// <summary>
		/// A guard condition that always returns false.
		/// </summary>
		public static readonly Func<TMessage, Boolean> False = message => false;
	}
}
