/*
 * Finite state machine library
 * Copyright (c) 2014-5 Steelbreeze Limited
 * Licensed under the MIT and GPL v3 licences
 * http://www.steelbreeze.net/state.cs
 */
namespace Steelbreeze.StateMachines.Model
{
    public class Visitor<TInstance> where TInstance : IInstance<TInstance>
    {
        public virtual void VisitElement(NamedElement element) { }

        public virtual void VisitRegion(Region<TInstance> region)
        {
            this.VisitElement(region);

            foreach (var vertex in region.Vertices)
            {
                vertex.Accept(this);
            }
        }

        public virtual void VisitVertex(Vertex<TInstance> vertex)
        {
            this.VisitElement(vertex);

            foreach (var transition in vertex.Outgoing)
            {
                transition.Accept(this);
            }
        }

        public virtual void VisitPseudoState(PseudoState<TInstance> pseudoState)
        {
            this.VisitVertex(pseudoState);
        }

        public virtual void VisitState(State<TInstance> state)
        {
            this.VisitVertex(state);

            foreach (var region in state.Regions)
            {
                region.Accept(this);
            }
        }

        public virtual void VisitFinalState(FinalState<TInstance> finalState)
        {
            this.VisitState(finalState);
        }

        public virtual void VisitStateMachine(StateMachine<TInstance> stateMachine)
        {
            this.VisitState(stateMachine);
        }

        public virtual void VisitTransition(Transition<TInstance> transition) { }
    }

    public class Visitor<TInstance, TArg> where TInstance : IInstance<TInstance>
    {
        public virtual void VisitElement(NamedElement element, TArg arg) { }

        public virtual void VisitRegion(Region<TInstance> region, TArg arg)
        {
            this.VisitElement(region, arg);

            foreach (var vertex in region.Vertices)
            {
                vertex.Accept(this, arg);
            }
        }

        public virtual void VisitVertex(Vertex<TInstance> vertex, TArg arg)
        {
            this.VisitElement(vertex, arg);

            foreach (var transition in vertex.Outgoing)
            {
                transition.Accept(this, arg);
            }
        }

        public virtual void VisitPseudoState(PseudoState<TInstance> pseudoState, TArg arg)
        {
            this.VisitVertex(pseudoState, arg);
        }

        public virtual void VisitState(State<TInstance> state, TArg arg)
        {
            this.VisitVertex(state, arg);

            foreach (var region in state.Regions)
            {
                region.Accept(this, arg);
            }
        }

        public virtual void VisitFinalState(FinalState<TInstance> finalState, TArg arg)
        {
            this.VisitState(finalState, arg);
        }

        public virtual void VisitStateMachine(StateMachine<TInstance> stateMachine, TArg arg)
        {
            this.VisitState(stateMachine, arg);
        }

        public virtual void VisitTransition(Transition<TInstance> transition, TArg arg) { }
    }
}