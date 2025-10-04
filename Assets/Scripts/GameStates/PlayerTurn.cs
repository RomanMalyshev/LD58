using Model.Map;
using State_Machine;
using UnityEngine;

namespace GameStates
{
    public class PlayerTurn : IState
    {
        private readonly MapModel _mapModel;

        public PlayerTurn(MapModel mapModel)
        {
            _mapModel = mapModel;
        }

        public void Enter()
        {
           
        }

        public void Execute()
        {

        }

        public void Exit()
        {

        }
    }
}