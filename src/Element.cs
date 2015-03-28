/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Collections.Generic;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// A structural element of a state machine model.
	/// </summary>
	/// <typeparam name="TInstance">The type of the state machine instance.</typeparam>
	public abstract class Element<TInstance> where TInstance : class, IActiveStateConfiguration<TInstance> {
		#region Static members
		/// <summary>
		/// The string used to seperate names within a fully qualified namespace.
		/// </summary>
		public static String NamespaceSeperator { get; set; }

		/// <summary>
		/// Initialises the static members of NamedElement.
		/// </summary>
		static Element () {
			NamespaceSeperator = ".";
		}
		#endregion

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
		/// Creates a new instance of a subtype of the Element class.
		/// </summary>
		/// <param name="name">The name of the element.</param>
		/// <param name="parent">The parent of the element.</param>
		internal protected Element (String name, Element<TInstance> parent = null) {
			this.Name = name;
			this.QualifiedName = parent != null ? parent.QualifiedName + NamespaceSeperator + name : name;
		}

		/// <summary>
		/// Returns the elements parent element.
		/// </summary>
		public abstract Element<TInstance> Parent { get; }

		/// <summary>
		/// Returns the state machine model root element.
		/// </summary>
		public virtual StateMachine<TInstance> Root {
			get {
				return this.Parent.Root;
			}
		}

		/// <summary>
		/// Returns the elements ancestors back from the state machines root element.
		/// </summary>
		public IEnumerable<Element<TInstance>> Ancestors {
			get {
				if (this.Parent != null)
					foreach (var namedElement in this.Parent.Ancestors) // yield! would be great...
						yield return namedElement;

				yield return this;
			}
		}

		/// <summary>
		/// Tests the element to determine if it is part of the current active state confuguration
		/// </summary>
		/// <param name="instance">The state machine instance.</param>
		/// <returns>True if the element is active.</returns>
		/// <remarks>An element is part of the active state configuration if it has been entered but not yet exited.</remarks>
		public abstract Boolean IsActive (TInstance instance);

		/// <summary>
		/// Tests the element to determine if it is deemed to be complete.
		/// </summary>
		/// <param name="instance">The state machine instance.</param>
		/// <returns>True if the element is complete.</returns>
		public abstract Boolean IsComplete (TInstance instance);

		/// <summary>
		/// Accepts a visitor
		/// </summary>
		/// <param name="visitor">The visitor to visit.</param>
		/// <param name="param">A parameter passed to the visitor when visiting elements.</param>
		/// <remarks>
		/// A visitor will walk the state machine model from this element to all child elements including transitions calling the approritate visit method on the visitor.
		/// </remarks>
		public abstract void Accept<TParam> (Visitor<TInstance, TParam> visitor, TParam param);

		/// <summary>
		/// Returns a string representation of the element.
		/// </summary>
		/// <returns>Returns the fully qualified name of the element.</returns>
		public override String ToString () {
			return this.QualifiedName;
		}
	}
}