using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Steelbreeze.Behavior
{
	/// <summary>
	/// The root of a state machine; container for Regions
	/// </summary>
	public class StateMachine : Element
	{
		/// <summary>
		/// Function to name default regions for state machines.
		/// </summary>
		public static Func<StateMachine, String> DefaultRegionName = sm => sm.Name;

		/// <summary>
		/// Returns (and creates if necessary) the default region for a state machine.
		/// </summary>
		/// <param name="stateMachine">The state machine to get the default region for.</param>
		/// <returns></returns>
		public static implicit operator Region( StateMachine stateMachine )
		{
			return stateMachine.regions.SingleOrDefault( region => region.Name.Equals( DefaultRegionName( stateMachine ) ) ) ?? new Region( DefaultRegionName( stateMachine ), stateMachine );
		}

		internal ICollection<Region> regions = new HashSet<Region>();

		/// <summary>
		/// Creates a new instance of the StateMachine class
		/// </summary>
		/// <param name="name">The name of the state macchine</param>
		public StateMachine( String name ) : base( name, null ) { }

		/// <summary>
		/// Tests the state machine for completeness.
		/// </summary>
		/// <param name="context">The state machine state to test.</param>
		/// <returns>True if the all the child regions are complete.</returns>
		public bool IsComplete( IState context )
		{
			return context.IsTerminated || regions.All( region => region.IsComplete( context ) );
		}

		/// <summary>
		/// Initialises a state machine to its initial state
		/// </summary>
		/// <param name="context">The state machine state.</param>
		public void Initialise( IState context )
		{
			this.BeginEnter( context );
			this.EndEnter( context, false );
		}

		internal override void BeginExit( IState context )
		{
			foreach( var region in regions )
			{
				if( context.GetActive( region ) )
				{
					region.BeginExit( context );
					region.EndExit( context );
				}
			}
		}

		internal override void EndEnter( IState context, bool deepHistory )
		{
			foreach( var region in regions )
			{
				region.BeginEnter( context );
				region.EndEnter( context, deepHistory );
			}

			base.EndEnter( context, deepHistory );
		}

		/// <summary>
		/// Attempts to process a message against an orthogonal state.
		/// </summary>
		/// <param name="context">The state machine state.</param>
		/// <param name="message">The message to evaluate.</param>
		/// <returns>A boolean indicating if the message caused a state change.</returns>
		public bool Process( IState context, object message )
		{
			if( context.IsTerminated )
				return false;

			return regions.Aggregate( false, ( result, region ) => region.Process( context, message ) || result );
		}
	}
}