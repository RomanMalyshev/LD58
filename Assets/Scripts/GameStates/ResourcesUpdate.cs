using System;
using Model;
using State_Machine;
using Map;

namespace GameStates
{
    public class ResourcesUpdate : IState
    {
        public Action OnResourcesUpdated;

        private Player _player;
        private MapController _map;
        private bool _isComplete;

        public ResourcesUpdate(Player playerModel, MapController map)
        {
            _player = playerModel;
            _map = map;
        }

        public void Enter()
        {
            _isComplete = false;
            UpdateResources();
        }

        public void Execute()
        {
            if (_isComplete)
            {
                OnResourcesUpdated?.Invoke();
            }
        }

        public void Exit()
        {
            _isComplete = false;
        }

        private void UpdateResources()
        {
            var occupiedTiles = _map.GetOccupiedTiles();
            var income = ResourceCalculator.CalculateTotalIncome(occupiedTiles);

            // Apply income
            _player.Food += income.Food;
            _player.Power += income.Power;
            _player.Wood += income.Wood;
            _player.Gold += income.Gold;
            _player.Metal += income.Metal;

            // Apply food consumption (Power × 1)
            int foodConsumption = ResourceCalculator.CalculateFoodConsumption(_player.Power);
            _player.Food -= foodConsumption;

            _isComplete = true;
        }
    }
}