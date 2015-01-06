﻿/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */using System;
using System.Linq;
using System.Xml.Linq;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// A sample of an object to extend as a base for a state machine context objects based on an XElement.
	/// </summary>
	/// <typeparam name="TContext">The type of the derived context class.</typeparam>
	/// <remarks>
	/// By passing the type of the derived class into this base, it allows the callbacks generated by the state machine to pass the fully typed derived class.
	/// Note that properties and methods have been explicitly implemented to hide the members from use other than via the IContext interface.
	/// Should you need persistence, or other such behaviour relating to the context class, implement another class implementing IContext.
	/// </remarks>
	public abstract class XContextBase<TContext> : IContext<TContext> where TContext : IContext<TContext> {
		/// <summary>
		/// The XElement that stores the context information.
		/// </summary>
		/// <remarks>
		/// The context information stored is not a full representation of the state machine model, but just enough to store the current state.
		/// </remarks>
		public XElement XElement { get; private set; }

		/// <summary>
		/// Creates a new instance of the XmlContext class.
		/// </summary>
		/// <param name="content">Any additional XML structure that you may need under the root element.</param>
		public XContextBase( params object[] content ) {
			this.XElement = new XElement( "stateMachineContext", content );

			this.XElement.Add( new XAttribute( "terminated", false ) );
		}

		/// <summary>
		/// Indicates that the state machine context has been terminated.
		/// </summary>
		/// <remarks>A state machine is only deemed terminated if a transitions target is a Terminate PseudoState.</remarks>
		public Boolean IsTerminated {
			set {
				XElement.Attribute( "terminated" ).Value = Convert.ToString( value );
			}

			get {
				return Convert.ToBoolean( XElement.Attribute( "terminated" ).Value );
			}
		}

		Vertex<TContext> IContext<TContext>.this[ Region<TContext> region ] {
			set {
				XElement element = this[ region ];
				XAttribute attribute = element.Attribute( "last" );

				if( attribute == null )
					element.Add( new XAttribute( "last", value.Name ) );
				else
					attribute.Value = Convert.ToString( value.Name );
			}

			get {
				var value = default( Vertex<TContext> );

				XElement element = this[ region ];
				XAttribute attribute = element.Attribute( "last" );

				if( attribute != null )
					value = region.Vertices.Single( vertex => vertex.Name == Convert.ToString( attribute.Value ) );

				return value;
			}
		}

		private XElement this[ Region<TContext> region ] {
			get {
				var value = this.XElement;

				foreach( var stateMachineElement in region.Ancestors ) {
					var element = value.Elements().SingleOrDefault( e => e.Attribute( "name" ).Value == stateMachineElement.Name );

					if( element == null )
						value.Add( element = new XElement( stateMachineElement.Type, new XAttribute( "name", stateMachineElement.Name ) ) );

					value = element;
				}

				return value;
			}
		}
	}
}