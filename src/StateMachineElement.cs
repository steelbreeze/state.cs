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
	public abstract class StateMachineElement<TContext> : NamedElement where TContext : IContext<TContext> {
		/// <summary>
		/// The name of the type without generic considerations
		/// </summary>
		public abstract String Type { get; }
	
		/// <summary>
		/// Returns the elements parent.
		/// </summary>
		public abstract StateMachineElement<TContext> Parent { get; }

		/// <summary>
		/// The parent state machine that this element forms a part of.
		/// </summary>
		public StateMachine<TContext> Root { get; protected set; }

		/// <summary>
		/// Returns the elements ancestors.
		/// </summary>
		public IEnumerable<StateMachineElement<TContext>> Ancestors { get { if( this.Parent != null ) foreach( var namedElement in this.Parent.Ancestors ) yield return namedElement; yield return this; } } // yield! please...

		internal Action<Object, TContext, Boolean> Leave;
		internal Action<Object, TContext, Boolean> BeginEnter;
		internal Action<Object, TContext, Boolean> EndEnter;
		internal Action<Object, TContext, Boolean> Enter;

		internal StateMachineElement( String name, StateMachineElement<TContext> parent ) : base( name, parent ) {
			if( parent != null )
				this.Root = parent.Root;
		}

		internal void Reset() {
			this.Leave = null;
			this.BeginEnter = null;
			this.EndEnter = null;
			this.Enter = null;
		}

		internal virtual void BootstrapElement( Boolean deepHistoryAbove ) {
#if DEBUG
			this.Leave += ( message, context, history ) => Console.WriteLine( "{0} leave {1}", context, this.QualifiedName );
			this.BeginEnter += ( message, context, history ) => Console.WriteLine( "{0} enter {1}", context, this.QualifiedName );
#endif
			this.Enter = this.BeginEnter + this.EndEnter;
		}

		internal abstract void BootstrapTransitions();

		internal virtual void BootstrapEnter( ref Action<Object, TContext, Boolean> traverse, StateMachineElement<TContext> next ) {
			traverse += this.BeginEnter;
		}
	}
}
