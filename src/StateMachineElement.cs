/* State v5 finite state machine library
 * Copyright (c) 2014 Steelbreeze Limited
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
		public abstract String Type { get; }
	
		/// <summary>
		/// Returns the elements parent.
		/// </summary>
		public abstract StateMachineElement<TContext> Parent { get; }

		/// <summary>
		/// Returns the elements ancestors.
		/// </summary>
		public IEnumerable<StateMachineElement<TContext>> Ancestors { get { if( this.Parent != null ) foreach( var namedElement in this.Parent.Ancestors ) yield return namedElement; yield return this; } } // yield! please...

		internal Action<TContext, Object, Boolean> Leave;
		internal Action<TContext, Object, Boolean> BeginEnter;
		internal Action<TContext, Object, Boolean> EndEnter;
		internal Action<TContext, Object, Boolean> Enter;

		internal StateMachineElement( String name, StateMachineElement<TContext> parent ) : base( name, parent ) { }

		internal void Reset() {
			this.Leave = null;
			this.BeginEnter = null;
			this.EndEnter = null;
			this.Enter = null;
		}

		internal virtual void BootstrapElement( Boolean deepHistoryAbove ) {
#if DEBUG
			this.Leave += ( context, message, history ) => Console.WriteLine( "{0} leave {1}", context, this.QualifiedName );
			this.BeginEnter += ( context, message, history ) => Console.WriteLine( "{0} enter {1}", context, this.QualifiedName );
#endif
			this.Enter = this.BeginEnter + this.EndEnter;
		}

		internal abstract void BootstrapTransitions();

		internal virtual void BootstrapEnter( ref Action<TContext, Object, Boolean> traverse, StateMachineElement<TContext> next ) {
			traverse += this.BeginEnter;
		}
	}
}
