using System;
using System.Collections.Generic;
using VContainer;
using VContainer.Unity;

namespace CodeBase.Infrastructure.FSM
{
    public class StateMachine : IStateMachine
    {
        private readonly Dictionary<Type, IExitableState> _states = new Dictionary<Type, IExitableState>();
        
        private IExitableState _activeState;
        private LifetimeScope _parentScope;

        public StateMachine(LifetimeScope parentScope)
        {
            _parentScope = parentScope;
        }

        public void RegisterState<TState>() where TState : IExitableState
        {
            TState state = _parentScope.CreateChild(builder =>
                builder.Register<TState>(Lifetime.Transient)).Container.Resolve<TState>();

            _states.Add(typeof(TState), state);
        }

        public void Enter<TState>() where TState : class, IState
        {
            IState state = ChangeState<TState>();
            state.Enter();
        }

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            _activeState?.Exit();

            TState state = GetState<TState>();
            _activeState = state;

            return state;
        }

        private TState GetState<TState>() where TState : class, IExitableState =>
            _states[typeof(TState)] as TState;
    }

    public interface IStateMachine
    {
        void Enter<TState>() where TState : class, IState;
        void RegisterState<TState>() where TState : IExitableState;
    }
}