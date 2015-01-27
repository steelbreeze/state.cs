/* State v5 finite state machine library
 * http://www.steelbreeze.net/state.cs
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// A state models a situation during which some (usually implicit) invariant condition holds.
	/// </summary>
	/// <typeparam name="TContext">The type of the state machine instance.</typeparam>
	/// <remarks>
	/// The invariant may represent a static situation such as an object waiting for some external event to occur.
	/// </remarks>
	public class State<TContext> : Vertex<TContext> where TContext : IContext<TContext> {
		/// <summary>
		/// The name of the type without generic considerations
		/// </summary>
		public override string Type { get { return "state"; } }

		/// <summary>
		/// True if the State is a simple State.
		/// </summary>
		/// <remarks>
		/// A simple State is one that has no child Regions.
		/// </remarks>
		public Boolean IsSimple { get { return this.regions == null || this.regions.Length == 0; } }

		/// <summary>
		/// True if the State is a composite State.
		/// </summary>
		/// <remarks>
		/// A composite State is one that has one or more child Regions.
		/// </remarks>
		public Boolean IsComposite { get { return this.regions != null && this.regions.Length > 0; } }

		/// <summary>
		/// True if the State is an orthogonal State.
		/// </summary>
		/// <remarks>
		/// A composite State is one that has more than one child Regions.
		/// </remarks>
		public Boolean IsOrthogonal { get { return this.regions != null && this.regions.Length > 1; } }

		/// <summary>
		/// The child Regions where the State is composite.
		/// </summary>
		public IEnumerable<Region<TContext>> Regions { get { return this.regions; } }

		internal Region<TContext>[] regions;

		private event Action<Object, TContext> exit;
		private event Action<Object, TContext> entry;

		/// <summary>
		/// Creates a new instance of the State class.
		/// </summary>
		/// <param name="name">The name of the state.</param>
		/// <param name="parent">The parent Region.</param>
		public State( String name, Region<TContext> parent )
			: base( name, parent, Transition<TContext>.State ) {
			Trace.Assert( name != null, "States must have a name" );
			Trace.Assert( parent != null, "States must have a parent Region" );
		}

		// Constructor used by FinalState
		internal State( String name, Region<TContext> parent, Func<Transition<TContext>[], Object, TContext, Transition<TContext>> selector ) : base( name, parent, selector ) { }

		/// <summary>
		/// Sets optional exit behavior that is called when leaving the State.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that triggered the state transition.</typeparam>
		/// <param name="behavior">One or more actions that take both the state machine context and the triggering message as parameters.</param>
		/// <returns>Returns the State itself.</returns>
		/// <remarks>If the type of the triggering message does not match TMessage the behavior will not be called.</remarks>
		public State<TContext> Exit<TMessage>( params Action<TMessage, TContext>[] behavior ) where TMessage : class {
			foreach( var exit in behavior )
				this.exit += ( message, context ) => { if( message is TMessage ) exit( message as TMessage, context ); };

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Sets optional exit behavior that is called when leaving the State.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that triggered the state transition.</typeparam>
		/// <param name="behavior">One or more actions that takes the triggering message as a parameter.</param>
		/// <returns></returns>
		/// <remarks>If the type of the triggering message does not match TMessage the behavior will not be called.</remarks>
		public State<TContext> Exit<TMessage>( params Action<TMessage>[] behavior ) where TMessage : class {
			foreach( var exit in behavior )
				this.exit += ( message, context ) => { if( message is TMessage ) exit( message as TMessage ); };

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Sets optional exit behabiour that is called when leaving the State.
		/// </summary>
		/// <param name="behavior">One or more actions that takes the state machine context as a parameter.</param>
		/// <returns>Returns the State itself.</returns>
		public State<TContext> Exit( params Action<TContext>[] behavior ) {
			foreach( var exit in behavior )
				this.exit += ( message, context ) => exit( context );

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Sets optional exit behabiour that is called when leaving the State.
		/// </summary>
		/// <param name="behavior">One or more actions that take no parameters.</param>
		/// <returns>Returns the State itself.</returns>
		public State<TContext> Exit( params Action[] behavior ) {
			foreach( var exit in behavior )
				this.exit += ( message, context ) => exit();

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Sets optional entry behavior that is called when entering the State.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that triggered the state transition.</typeparam>
		/// <param name="behavior">One or more actions that take both the state machine context and the triggering message as parameters.</param>
		/// <returns>Returns the State itself.</returns>
		/// <remarks>If the type of the triggering message does not match TMessage the behavior will not be called.</remarks>
		public State<TContext> Entry<TMessage>( params Action<TMessage, TContext>[] behavior ) where TMessage : class {
			foreach( var entry in behavior )
				this.entry += ( message, context ) => { if( message is TMessage ) entry( message as TMessage, context ); };

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Sets optional entry behavior that is called when entering the State.
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that triggered the state transition.</typeparam>
		/// <param name="behavior">One or more actions that takes the triggering message as a parameter.</param>
		/// <returns>Returns the State itself.</returns>
		/// <remarks>If the type of the triggering message does not match TMessage the behavior will not be called.</remarks>
		public State<TContext> Entry<TMessage>( params Action<TMessage>[] behavior ) where TMessage : class {
			foreach( var entry in behavior )
				this.entry += ( message, context ) => { if( message is TMessage ) entry( message as TMessage ); };

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Sets optional entry behavior that is called when entering the State.
		/// </summary>
		/// <param name="behavior">One or more actions that takes the state machine context as a parameter.</param>
		/// <returns>Returns the State itself.</returns>
		public State<TContext> Entry( params Action<TContext>[] behavior ) {
			foreach( var entry in behavior )
				this.entry += ( message, context ) => entry( context );

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Sets optional entry behavior that is called when entering the State.
		/// </summary>
		/// <param name="behavior">One or more actions that takes no parameters.</param>
		/// <returns>Returns the State itself.</returns>
		public State<TContext> Entry( params Action[] behavior ) {
			foreach( var entry in behavior )
				this.entry += ( message, context ) => entry();

			this.Root.Clean = false;

			return this;
		}

		/// <summary>
		/// Creates an internal transition.
		/// </summary>
		/// <typeparam name="TMessage">The type of the messaage that the internal transition will react to.</typeparam>
		/// <param name="guard">The guard condition that must be met for hte transition to be traversed.</param>
		/// <returns>The internal transition.</returns>
		/// <remarks>
		/// An internal transition does not exit or enter any states, however the transitions effect will be invoked if the guard condition of the transition is met
		/// </remarks>
		public Transition<TContext> When<TMessage>( Func<TMessage, TContext, Boolean> guard ) where TMessage : class {
			return this.To( null ).When( guard );
		}

		/// <summary>
		/// Creates an internal trantiion.
		/// </summary>
		/// <typeparam name="TMessage">The type of the messaage that the internal transition will react to.</typeparam>
		/// <param name="guard">The guard condition that must be met for hte transition to be traversed.</param>
		/// <returns>The internal transition.</returns>
		public Transition<TContext> When<TMessage>( Func<TMessage, Boolean> guard ) where TMessage : class {
			return this.To( null ).When( guard );
		}

		/// <summary>
		/// Invokes the Exit behavior upon exiting a State.
		/// </summary>
		/// <param name="context">The state machine context.</param>
		/// <param name="message">The message that triggered the state transition.</param>
		/// <param name="history">A flag denoting if history semantics were in play during the transition.</param>
		protected virtual void OnExit( Object message, TContext context, Boolean history ) {
			this.exit( message, context );
		}

		/// <summary>
		/// Invokes the Entry behavior upon entering a State.
		/// </summary>
		/// <param name="context">The state machine context.</param>
		/// <param name="message">The message that triggered the state transition.</param>
		/// <param name="history">A flag denoting if history semantics were in play during the transition.</param>
		protected virtual void OnEntry( Object message, TContext context, Boolean history ) {
			this.entry( message, context );
		}

		internal override Boolean IsComplete( TContext context ) {
			return this.IsSimple || this.regions.All( region => region.IsComplete( context ) );
		}

		internal virtual void Add( Region<TContext> region ) {
			if( this.regions == null )
				this.regions = new Region<TContext>[ 1 ] { region };
			else {
				Trace.Assert( this.regions.Where( r => r.Name == region.Name ).Count() == 0, "Regions must have a unique name within the scope of their parent State" );

				var regions = new Region<TContext>[ this.regions.Length + 1 ];

				this.regions.CopyTo( regions, 0 );

				regions[ this.regions.Length ] = region;

				this.regions = regions;
			}

			region.Root.Clean = false;
		}

		internal override void BootstrapElement( Boolean deepHistoryAbove ) {
			if( this.IsComposite ) {
				foreach( var region in this.regions ) {
					region.Reset();
					region.BootstrapElement( deepHistoryAbove );

					this.Leave += ( message, context, history ) => region.Leave( message, context, history );
					this.EndEnter += region.Enter;
				}
			}

			base.BootstrapElement( deepHistoryAbove );

			if( this.exit != null )
				this.Leave += this.OnExit;

			if( this.entry != null )
				this.BeginEnter += this.OnEntry;

			this.BeginEnter += ( message, context, history ) => context[ this.Region ] = this;

			this.Enter = this.BeginEnter + this.EndEnter;
		}

		internal override void BootstrapTransitions() {
			if( this.IsComposite )
				foreach( var region in this.regions )
					region.BootstrapTransitions();

			base.BootstrapTransitions();
		}

		internal override void BootstrapEnter( ref Action<Object, TContext, Boolean> traverse, Element<TContext> next ) {
			base.BootstrapEnter( ref traverse, next );

			if( this.IsOrthogonal )
				foreach( var region in this.regions )
					if( region != next )
						traverse += region.Enter;
		}

		internal override Boolean Evaluate( Object message, TContext context ) {
			var processed = false;

			if( this.IsComposite )
				for( int i = 0, l = this.regions.Length; i < l; ++i )
					if( this.regions[ i ].Evaluate( message, context ) )
						processed = true;

			if( !processed )
				processed = base.Evaluate( message, context );

			if( processed == true && message != this )
				this.EvaluateCompletions( this, context, false );

			return processed;
		}
	}
}
