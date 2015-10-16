/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
namespace Steelbreeze.StateMachines.Model {
	/// <summary>
	/// Represents an element within a model that has a name.
	/// </summary>
	public class NamedElement {
		/// <summary>
		/// The token used as a namespace seperator when generating fully qualified names.
		/// </summary>
		public static string NamespaceSeparator;

		/// <summary>
		/// The name of the element.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// The fully qualitied name of the element.
		/// </summary>
		public readonly string QualifiedName;

		static NamedElement () {
			NamespaceSeparator = ".";
		}

		internal NamedElement (string name, NamedElement parent) {
			this.Name = name;
			this.QualifiedName = parent != null ? (parent.QualifiedName + NamespaceSeparator + name) : name;
		}

		/// <summary>
		/// Returns the fully qualified name of the element.
		/// </summary>
		/// <returns></returns>
		public override string ToString () {
			return this.QualifiedName;
		}
	}
}