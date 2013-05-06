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
using System.Linq;
using System.Xml.Linq;

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// Restores the current active state of a state machine, region or composite state
	/// </summary>
	public class XmlDeserializer : Visitor<XElement>
	{
		private static readonly XNamespace ns = Names.Namespace;

		/// <summary>
		/// Restores the current active state of a region 
		/// </summary>
		/// <param name="region">The region to restore</param>
		/// <param name="xml">The XML of the regions parent state machine or composite state</param>
		/// <returns>The XML for this region</returns>
		public override XElement Visit( Region region, XElement xml )
		{
			// get the xml for this region
			if( !xml.Name.Equals( ns + region.GetType().Name.ToLower() ) )
				xml = xml.Elements( ns + region.GetType().Name.ToLower() ).Single( r => r.Attribute( Names.Name ).Value.Equals( region.Name ) );

			// set active and current states
			region.IsActive =Convert.ToBoolean( xml.Attribute( Names.Active ).Value );
			region.Current = region.vertices.OfType<StateBase>().SingleOrDefault( s => s.Name.Equals( xml.Attribute( Names.Current ).Value ) );

			return xml;
		}

		/// <summary>
		/// Restores the current active state of a composite state
		/// </summary>
		/// <param name="state">The state to restore</param>
		/// <param name="xml">The XML of the parent region</param>
		/// <returns>The XML for this state</returns>
		public override XElement Visit( StateBase state, XElement xml )
		{
			if( !xml.Name.Equals( ns + state.GetType().Name.ToLower() ) )
				xml = xml.Elements( ns + state.GetType().Name.ToLower() ).Single( s => s.Attribute( Names.Name ).Value.Equals( state.Name ) );

			// set active and current states
			state.IsActive = Convert.ToBoolean( xml.Attribute( Names.Active ).Value );

			return xml;
		}
	}
}