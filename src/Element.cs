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
	public abstract class Element<TInstance> where TInstance : IActiveStateConfiguration<TInstance> {
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
		/// The name of the type without generic considerations
		/// </summary>
		public abstract String Type { get; }

		/// <summary>
		/// Returns the elements parent.
		/// </summary>
		public abstract Element<TInstance> Parent { get; }

		/// <summary>
		/// The parent state machine that this element forms a part of.
		/// </summary>
		public StateMachine<TInstance> Root { get; protected set; }

		/// <summary>
		/// Returns the elements ancestors.
		/// </summary>
		public IEnumerable<Element<TInstance>> Ancestors { get { if (this.Parent != null) foreach (var namedElement in this.Parent.Ancestors) yield return namedElement; yield return this; } } // yield! please...

		internal Action<Object, TInstance, Boolean> Leave;
		internal Action<Object, TInstance, Boolean> BeginEnter;
		internal Action<Object, TInstance, Boolean> EndEnter;
		internal Action<Object, TInstance, Boolean> Enter;

		internal Element (String name, Element<TInstance> parent) {
			this.Name = name;
			this.QualifiedName = parent != null ? parent.QualifiedName + NamespaceSeperator + name : name;

			if (parent != null)
				this.Root = parent.Root;
		}

		internal void Reset () {
			this.Leave = null;
			this.BeginEnter = null;
			this.EndEnter = null;
			this.Enter = null;
		}

		/// <summary>
		/// Tests the element to determine if it is part of the current active state confuguration
		/// </summary>
		/// <param name="instance">The state machine instance.</param>
		/// <returns>True if the element is active.</returns>
		internal protected abstract Boolean IsActive (IActiveStateConfiguration<TInstance> instance);

		internal virtual void BootstrapElement (Boolean deepHistoryAbove) {
#if DEBUG
			this.Leave += (message, instance, history) => Console.WriteLine ("{0} leave {1}", instance, this.QualifiedName);
			this.BeginEnter += (message, instance, history) => Console.WriteLine ("{0} enter {1}", instance, this.QualifiedName);
#endif
			this.Enter = this.BeginEnter + this.EndEnter;
		}

		internal abstract void BootstrapTransitions ();

		internal virtual void BootstrapEnter (ref Action<Object, TInstance, Boolean> traverse, Element<TInstance> next) {
			traverse += this.BeginEnter;
		}

		/// <summary>
		/// Returns a string representation of the element.
		/// </summary>
		/// <returns>Returns the fully qualified name of the element.</returns>
		public override String ToString () {
			return this.QualifiedName;
		}
	}
}