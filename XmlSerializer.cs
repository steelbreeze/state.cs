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
using System.Xml.Linq;

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// Serializes a state machine or part thereof to XML
	/// </summary>
	public class XmlSerializer : Visitor<XElement>
	{
		private static XNamespace ns = Names.Namespace;

		/// <summary>
		/// The namespace prefix used while serializing
		/// </summary>
		public String NamespacePrefix { get; private set; }

		/// <summary>
		/// Creates a new XML Serializer
		/// </summary>
		/// <param name="namespacePrefix">An optional namespace prefix</param>
		public XmlSerializer( String namespacePrefix = null )
		{
			NamespacePrefix = namespacePrefix;
		}

		/// <summary>
		/// Creates the XML elements for any node within the state machine
		/// </summary>
		/// <param name="node">The state machine node to creat the element for</param>
		/// <param name="parent">The parent element to add the new element to</param>
		/// <returns>The new element</returns>
		public override XElement Visit( StateMachineBase node, XElement parent )
		{
			var element = new XElement( ns + node.GetType().Name.ToLower() );

			if( NamespacePrefix != null )
				if( parent == null || parent.GetPrefixOfNamespace( ns ) == null )
					element.Add( new XAttribute( XNamespace.Xmlns + NamespacePrefix, ns ) );

			if( parent != null )
				parent.Add( element );

			return element;
		}

		/// <summary>
		/// Adds region specific attributes to the XML element
		/// </summary>
		/// <param name="region">The region to visit</param>
		/// <param name="xml">The  XML element to add to</param>
		/// <returns>The augmented XML element</returns>
		override public XElement Visit( Region region, XElement xml )
		{
			xml.Add( new XAttribute( Names.Name, region.Name ), new XAttribute( Names.Active, region.IsActive ), new XAttribute( Names.Current, region.Current == null ? "" : region.Current.Name ) );

			return xml;
		}

		/// <summary>
		/// Adds pseudo state specific attributes to the XML element
		/// </summary>
		/// <param name="pseudoState">The region to visit</param>
		/// <param name="xml">The  XML element to add to</param>
		/// <returns>The augmented XML element</returns>
		public override XElement Visit( PseudoState pseudoState, XElement xml )
		{
			xml.Add( new XAttribute( Names.Kind, pseudoState.Kind.Name ) );

			return xml;
		}

		/// <summary>
		/// Adds state specific attributes to the XML element
		/// </summary>
		/// <param name="state">The region to visit</param>
		/// <param name="xml">The  XML element to add to</param>
		/// <returns>The augmented XML element</returns>
		public override XElement Visit( StateBase state, XElement xml )
		{
			xml.Add( new XAttribute( Names.Name, state.Name ), new XAttribute( Names.Active, state.IsActive ) );

			return xml;
		}
	}
}