/* State v5 finite state machine library
 * Copyright (c) 2014 Steelbreeze Limited
 * Licensed under MIT and GPL v3 licences
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Steelbreeze.Behavior.StateMachines {
	/// <summary>
	/// A container of vertices within a state machine model.
	/// </summary>
	/// <typeparam name="TContext">The type of the state machine instance.</typeparam>
	/// <remarks>
	/// All state machines and composite states will contain at least one Region; orthogonal composite states will contain more than one.
	/// </remarks>
	public sealed class Region<TContext> : StateMachineElement<TContext> where TContext : IContext<TContext> {
		#region Static members
		/// <summary>
		/// The name used for default regions.
		/// </summary>
		/// <remarks>
		/// Used when a region is created implicitly; typically when a StateMachine or State is passed where a Region is expected.
		/// </remarks>
		public static String DefaultName { get; set; }

		// Initialise the static members.
		static Region() {
			DefaultName = "default";
		}

		/// <summary>
		/// Provides an implicit conversion from a StateMachine to a Region, initialising a new instance of the Region class if required under this State.
		/// </summary>
		/// <param name="stateMachine">The StateMachine to return the default region for.</param>
		/// <returns>The default Region of the StateMAchine.</returns>
		/// <remarks>
		/// The default Region of a StateMachine is one that has the same name as defined by Region.DefaultName.
		/// </remarks>
		public static implicit operator Region<TContext>( StateMachine<TContext> stateMachine ) {
			return ( stateMachine.regions == null ? null : stateMachine.regions.SingleOrDefault( r => r.Name == Region<TContext>.DefaultName ) ) ?? new Region<TContext>( Region<TContext>.DefaultName, stateMachine );
		}

		/// <summary>
		/// Provides an implicit conversion from a State to a Region, initialising a new instance of the Region class if required under this State.
		/// </summary>
		/// <param name="state">The State to return the default region for.</param>
		/// <returns>The default Region of the composite State.</returns>
		/// <remarks>
		/// The default Region of a composite State is one that has the same name as defined by Region.DefaultName.
		/// </remarks>
		public static implicit operator Region<TContext>( State<TContext> state ) {
			if( state.regions != null ) {
				var region = state.regions.SingleOrDefault( r => r.Name == Region<TContext>.DefaultName );

				if( region != null )
					return region;
			}

			return new Region<TContext>( Region<TContext>.DefaultName, state );
		}
		#endregion
		/// <summary>
		/// Returns the Region's parent element.
		/// </summary>
		public override StateMachineElement<TContext> Parent { get { return this.parent; } }

		/// <summary>
		/// The child Vertices
		/// </summary>
		public IEnumerable<Vertex<TContext>> Vertices { get { return this.vertices; } }

		internal readonly StateMachine<TContext> Root;
		internal PseudoState<TContext> Initial = null;

		private readonly HashSet<Vertex<TContext>> vertices = new HashSet<Vertex<TContext>>();
		private readonly StateMachineElement<TContext> parent;

		/// <summary>
		/// Initialises a new instance of the Region class within a StateMachine.
		/// </summary>
		/// <param name="name">The name of the Region.</param>
		/// <param name="parent">The parent StateMachine.</param>
		public Region( String name, StateMachine<TContext> parent )
			: base( name, parent ) {
			Trace.Assert( name != null, "Regions must have a name" );
			Trace.Assert( parent != null, "Regions must have a parent" );

			this.Root = parent;
			this.parent = parent;

			parent.Add( this );
		}

		/// <summary>
		/// Initialises a new instance of the Region class within a composite State.
		/// </summary>
		/// <param name="name">The name of the Region.</param>
		/// <param name="parent">The parent composite State.</param>
		/// <remarks>
		/// Adding a Region to a State makes the State a composite State.
		/// </remarks>
		public Region( String name, State<TContext> parent )
			: base( name, parent ) {
			Trace.Assert( name != null, "Regions must have a name" );
			Trace.Assert( parent != null, "Regions must have a parent" );

			this.Root = parent.Root;
			this.parent = parent;

			parent.Add( this );
		}

		internal void Add( Vertex<TContext> vertex ) {
			Trace.Assert( vertex != null, "Cannot add a null vertex" );
			Trace.Assert( this.vertices.Where( v => v.Name == vertex.Name ).Count() == 0, "Vertices must have a unique name within the scope of their parent Region" );

			this.vertices.Add( vertex );

			vertex.Root.Clean = false;
		}

		internal Boolean IsComplete( TContext context ) {
			return context[ this ].IsFinal;
		}

		internal override void BootstrapElement( Boolean deepHistoryAbove ) {
			foreach( var vertex in this.vertices ) {
				vertex.Reset();
				vertex.BootstrapElement( deepHistoryAbove || ( this.Initial != null && this.Initial.Kind == PseudoStateKind.DeepHistory ) );
			}

			this.Leave += ( context, message, history ) => { var current = context[ this ]; if( current.Leave != null ) current.Leave( context, message, history ); };

			if( deepHistoryAbove || this.Initial == null || this.Initial.IsHistory )
				this.EndEnter += ( context, message, history ) => ( history || this.Initial.IsHistory ? context[ this ] ?? this.Initial : this.Initial ).Enter( context, message, history || this.Initial.Kind == PseudoStateKind.DeepHistory );
			else this.EndEnter += this.Initial.Enter;

			base.BootstrapElement( deepHistoryAbove );
		}

		internal override void BootstrapTransitions() {
			foreach( var vertex in this.vertices )
				vertex.BootstrapTransitions();
		}

		internal Boolean Evaluate( TContext context, Object message ) {
			return context[ this ].Evaluate( context, message );
		}
	}
}