using UnityEngine;

namespace State_Machine
{
    public class StateMachine
    {
        private IState _currentState;

        public IState CurrentState => _currentState;

        public void ChangeState(IState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
            
            Debug.Log($"Current state is {_currentState}");
        }

        public void Execute()
        {
            _currentState?.Execute();
        }
    }
}