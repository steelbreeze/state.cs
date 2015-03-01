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
	/// <typeparam name="TContext">The type of the state machine instance.</typeparam>
	public abstract class Element<TContext> where TContext : IContext<TContext> {
		/// <summary>
		/// The string used to seperate names within a fully qualified namespace.
		/// </summary>
		public static String NamespaceSeperator { get; set; }

		/// <summary>
		/// Initialises the static members of NamedElement.
		/// </summary>
		static Element() {
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
		public abstract Element<TContext> Parent { get; }

		/// <summary>
		/// The parent state machine that this element forms a part of.
		/// </summary>
		public StateMachine<TContext> Root { get; protected set; }

		/// <summary>
		/// Returns the elements ancestors.
		/// </summary>
		public IEnumerable<Element<TContext>> Ancestors { get { if( this.Parent != null ) foreach( var namedElement in this.Parent.Ancestors ) yield return namedElement; yield return this; } } // yield! please...

		internal Action<Object, TContext, Boolean> Leave;
		internal Action<Object, TContext, Boolean> BeginEnter;
		internal Action<Object, TContext, Boolean> EndEnter;
		internal Action<Object, TContext, Boolean> Enter;

		internal Element( String name, Element<TContext> parent ) {
			this.Name = name;
			this.QualifiedName = parent != null ? parent.QualifiedName + NamespaceSeperator + name : name;

			if( parent != null )
				this.Root = parent.Root;
		}

		internal void Reset() {
			this.Leave = null;
			this.BeginEnter = null;
			this.EndEnter = null;
			this.Enter = null;
		}

		/// <summary>
		/// Tests the element to determine if it is part of the current active state confuguration
		/// </summary>
		/// <param name="context">The state machine context.</param>
		/// <returns>True if the element is active.</returns>
		internal protected abstract Boolean IsActive( IContext<TContext> context );

		internal virtual void BootstrapElement( Boolean deepHistoryAbove ) {
#if DEBUG
			this.Leave += ( message, context, history ) => Console.WriteLine( "{0} leave {1}", context, this.QualifiedName );
			this.BeginEnter += ( message, context, history ) => Console.WriteLine( "{0} enter {1}", context, this.QualifiedName );
#endif
			this.Enter = this.BeginEnter + this.EndEnter;
		}

		internal abstract void BootstrapTransitions();

		internal virtual void BootstrapEnter( ref Action<Object, TContext, Boolean> traverse, Element<TContext> next ) {
			traverse += this.BeginEnter;
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
