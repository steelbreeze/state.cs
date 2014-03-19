// Copyright © 2014 Steelbreeze Limited.
// This file is part of state.cs.
//
// state.cs is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published
// by the Free Software Foundation, either version 3 of the License,
// or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// The root of a state machine; container for Regions
	/// </summary>
	public class StateMachine<TState> : Element<TState> where TState : IState<TState>
	{
		/// <summary>
		/// Function to name default regions for state machines.
		/// </summary>
		public static Func<StateMachine<TState>, String> DefaultRegionName = sm => sm.Name;

		/// <summary>
		/// Returns (and creates if necessary) the default region for a state machine.
		/// </summary>
		/// <param name="stateMachine">The state machine to get the default region for.</param>
		/// <returns></returns>
		public static implicit operator Region<TState>( StateMachine<TState> stateMachine )
		{
			return stateMachine.regions.SingleOrDefault( region => region.Name.Equals( DefaultRegionName( stateMachine ) ) ) ?? new Region<TState>( DefaultRegionName( stateMachine ), stateMachine );
		}

		internal readonly ICollection<Region<TState>> regions = new HashSet<Region<TState>>();

		/// <summary>
		/// Creates a new instance of the StateMachine class
		/// </summary>
		/// <param name="name">The name of the state macchine</param>
		public StateMachine( String name ) : base( name, null ) { }

		/// <summary>
		/// Creates a new child Region within the StateMachine
		/// </summary>
		/// <param name="name">The name of the Region</param>
		/// <returns>The new child Region</returns>
		public Region<TState> CreateRegion( String name )
		{
			return new Region<TState>( name, this );
		}

		/// <summary>
		/// Creates a new child PseudoState within the StateMachine
		/// </summary>
		/// <param name="name">The name of the PseudoState</param>
		/// <param name="kind">The kind of the PseudoState</param>
		/// <returns>The new child PseudoState</returns>
		public PseudoState<TState> CreatePseudoState( String name, PseudoStateKind kind )
		{
			return new PseudoState<TState>( name, kind, this );
		}

		/// <summary>
		/// Creates a new child SimpleState within the StateMachine
		/// </summary>
		/// <param name="name">The name of the SimpleState</param>
		/// <returns>The new child SimpleState</returns>
		public SimpleState<TState> CreateSimpleState( String name )
		{
			return new SimpleState<TState>( name, this );
		}

		/// <summary>
		/// Creates a new child CompositeState within the StateMachine
		/// </summary>
		/// <param name="name">The name of the CompositeState</param>
		/// <returns>The new child CompositeState</returns>
		public CompositeState<TState> CreateCompositeState( String name )
		{
			return new CompositeState<TState>( name, this );
		}

		/// <summary>
		/// Creates a new child OrthogonalState within the StateMachine
		/// </summary>
		/// <param name="name">The name of the OrthogonalState</param>
		/// <returns>The new child Orthogonaltate</returns>
		public OrthogonalState<TState> CreateOrthogonalState( String name )
		{
			return new OrthogonalState<TState>( name, this );
		}

		/// <summary>
		/// Creates a new child FinalState within the StateMachine
		/// </summary>
		/// <param name="name">The name of the FinalState</param>
		/// <returns>The new child FinalState</returns>
		public FinalState<TState> CreateFinalState( String name )
		{
			return new FinalState<TState>( name, this );
		}

		/// <summary>
		/// Creates a new transition between PseudoStates within the StateMachine
		/// </summary>
		/// <param name="source">The source PseudoState to transition from</param>
		/// <param name="target">The target PseudoState to transition to</param>
		/// <param name="guard">The optional guard condition to evaluate prior to traversal</param>
		/// <returns>The new transition</returns>
		public Transition<TState> CreateTransition( PseudoState<TState> source, PseudoState<TState> target, Func<Boolean> guard = null )
		{
			return new Transition<TState>( source, target, guard );
		}

		/// <summary>
		/// Creates a new completion transition from a PseudoState to a SimpleState (or subclass thereof) within the StateMachine
		/// </summary>
		/// <param name="source">The source PseudoState to transition from</param>
		/// <param name="target">The target SimpleState (or subclass thereof) to transition to</param>
		/// <param name="guard">The optional guard condition to evaluate prior to traversal</param>
		/// <returns>The new transition</returns>
		public Transition<TState> CreateTransition( PseudoState<TState> source, SimpleState<TState> target, Func<Boolean> guard = null )
		{
			return new Transition<TState>( source, target, guard );
		}

		/// <summary>
		/// Creates a new completion transition from a SimpleState (or subclass thereof) to a PseudoState within the StateMachine
		/// </summary>
		/// <param name="source">The source SimpleState (or subclass thereof) to transition from</param>
		/// <param name="target">The target PseudoState to transition to</param>
		/// <param name="guard">The optional guard condition to evaluate prior to traversal</param>
		/// <returns>The new transition</returns>
		public Transition<TState> CreateTransition( SimpleState<TState> source, PseudoState<TState> target, Func<Boolean> guard = null )
		{
			return new Transition<TState>( source, target, guard );
		}

		/// <summary>
		/// Creates a new completion transition between SimpleStates (or subclasses thereof) within the StateMachine
		/// </summary>
		/// <param name="source">The source SimpleState (or subclass thereof) to transition from</param>
		/// <param name="target">The target SimpleState (or subclass thereof) to transition to</param>
		/// <param name="guard">The optional guard condition to evaluate prior to traversal</param>
		/// <returns>The new transition</returns>
		public Transition<TState> CreateTransition( SimpleState<TState> source, SimpleState<TState> target, Func<Boolean> guard = null )
		{
			return new Transition<TState>( source, target, guard );
		}

		/// <summary>
		/// Creates a new transition from a SimpleState (or subclass thereof) to a PseudoState within the StateMachine
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that may trigger the transition</typeparam>
		/// <param name="source">The source SimpleState (or subclass thereof) to transition from</param>
		/// <param name="target">The target PseudoState to transition to</param>
		/// <param name="guard">The optional guard condition to evaluate prior to traversal</param>
		/// <returns>The new transition</returns>
		public Transition<TState, TMessage> CreateTransition<TMessage>( SimpleState<TState> source, PseudoState<TState> target, Func<TMessage, Boolean> guard = null ) where TMessage : class
		{
			return new Transition<TState, TMessage>( source, target, guard );
		}

		/// <summary>
		/// Creates a new transition between SimpleStates (or subclasses thereof) within the StateMachine
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that may trigger the transition</typeparam>
		/// <param name="source">The source SimpleState (or subclass thereof) to transition from</param>
		/// <param name="target">The target SimpleState (or subclass thereof) to transition to</param>
		/// <param name="guard">The optional guard condition to evaluate prior to traversal</param>
		/// <returns>The new transition</returns>
		public Transition<TState, TMessage> CreateTransition<TMessage>( SimpleState<TState> source, SimpleState<TState> target, Func<TMessage, Boolean> guard = null ) where TMessage : class
		{
			return new Transition<TState, TMessage>( source, target, guard );
		}

		/// <summary>
		/// Creates a new internal transition within a SimpleState (or subclass thereof) of the StateMachine
		/// </summary>
		/// <typeparam name="TMessage">The type of the message that may trigger the transition</typeparam>
		/// <param name="source">The SimpleState (or subclass thereof) to transition from</param>
		/// <param name="guard">The optional guard condition to evaluate prior to traversal</param>
		/// <returns>The new transition</returns>
		public Transition<TState, TMessage> CreateTransition<TMessage>( SimpleState<TState> source, Func<TMessage, Boolean> guard = null ) where TMessage : class
		{
			return new Transition<TState, TMessage>( source, guard );
		}

		/// <summary>
		/// Creates a new Else completion transition between PseudoStates within a StateMachine
		/// </summary>
		/// <param name="source">The source PseudoState to transition from</param>
		/// <param name="target">The target PseudoState to transition to</param>
		/// <returns>The new transition</returns>
		public Transition<TState> CreateElse( PseudoState<TState> source, PseudoState<TState> target )
		{
			return new Transition<TState>.Else( source, target );
		}

		/// <summary>
		/// Creates a new Else completion transition from a PseudoState to a SimpleState (or subclass thereof) within a StateMachine
		/// </summary>
		/// <param name="source">The source PseudoState to transition from</param>
		/// <param name="target">The target SimpleState (or subclass thereof) to transition to</param>
		/// <returns>The new transition</returns>
		public Transition<TState> CreateElse( PseudoState<TState> source, SimpleState<TState> target )
		{
			return new Transition<TState>.Else( source, target );
		}

		/// <summary>
		/// Tests the state machine for completeness.
		/// </summary>
		/// <param name="state">The state machine state to test.</param>
		/// <returns>True if the all the child regions are complete.</returns>
		public bool IsComplete( TState state )
		{
			if( !state.IsTerminated )
				foreach( var region in this.regions )
					if( !region.IsComplete( state ) )
						return false;

			return true;
		}

		/// <summary>
		/// Initialises a state machine to its initial state
		/// </summary>
		/// <param name="state">The state machine state.</param>
		public void Initialise( TState state )
		{
			this.BeginEntry( state );
			this.EndEntry( state, false );
		}

		internal override void BeginExit( TState state )
		{
			foreach( var region in regions )
			{
				if( state.GetActive( region ) )
				{
					region.BeginExit( state );
					region.EndExit( state );
				}
			}
		}

		internal override void EndEntry( TState state, bool deepHistory )
		{
			foreach( var region in regions )
			{
				region.BeginEntry( state );
				region.EndEntry( state, deepHistory );
			}

			base.EndEntry( state, deepHistory );
		}

		/// <summary>
		/// Attempts to process a message against an orthogonal state.
		/// </summary>
		/// <param name="state">The state machine state.</param>
		/// <param name="message">The message to evaluate.</param>
		/// <returns>A boolean indicating if the message caused a state change.</returns>
		public bool Process( TState state, object message )
		{
			if( state.IsTerminated )
				return false;

			return regions.Aggregate( false, ( result, region ) => region.Process( state, message ) || result );
		}
	}
}