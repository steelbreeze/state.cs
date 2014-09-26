/* State v5 finite state machine library
 * Copyright (c) 2014 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.ComponentModel;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// An element within a model that may have a name.
	/// </summary>
	public abstract class NamedElement {
		/// <summary>
		/// The string used to seperate names within a fully qualified namespace.
		/// </summary>
		public static String NamespaceSeperator { get; set; }

		/// <summary>
		/// Initialises the static members of NamedElement.
		/// </summary>
		static NamedElement() {
			NamespaceSeperator = ".";
		}

		/// <summary>
		/// The name of the NamedElement
		/// </summary>
		public readonly String Name;

		/// <summary>
		/// The fully qualified name of the element.
		/// </summary>
		/// <remarks>A name that allows the NamedElement to be identified within a hierarchy of nested Namespaces.
		/// It is constructed from the names of the containing NamedElements starting at the root of the hierarchy and ending with the name of the NamedElement itself.
		/// This is a derived attribute which is cached on initialisation.</remarks>
		public readonly String QualifiedName;

		/// <summary>
		/// Creates a new instance of a named element.
		/// </summary>
		/// <param name="name">The name of the element</param>
		/// <param name="parent">The parent element of the new element</param>
		public NamedElement( String name, NamedElement parent ) {
			this.Name = name;
			this.QualifiedName = parent != null ? parent.QualifiedName + NamespaceSeperator + name : name;
		}

		/// <summary>
		/// Returns a string representation of the element.
		/// </summary>
		/// <returns>Returns the fully qualified name of the element.</returns>
		public override String ToString() {
			return this.QualifiedName;
		}
	}
}
