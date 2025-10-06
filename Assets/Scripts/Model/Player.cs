using System;

namespace Model
{
    public class Player
    {
        public event Action<int, int, int, int, int, int> OnResourcesChanged;
        public event Action<int, int> OnGameStatsChanged; // (currentTurn, capturedCastles)
        public event Action OnIncomeChanged; // Triggered when income needs to be recalculated

        private int _influence;
        private int _power;
        private int _food;
        private int _gold;
        private int _metal;
        private int _wood;

        private int _currentTurn;
        private int _capturedCastles;
        private int _tilesCaptured;

        //Resources
        public int Influence
        {
            get => _influence;
            set
            {
                _influence = value;
                NotifyResourcesChanged();
            }
        }

        public int Power
        {
            get => _power;
            set
            {
                _power = value;
                NotifyResourcesChanged();
            }
        }

        public int Food
        {
            get => _food;
            set
            {
                _food = value;
                NotifyResourcesChanged();
            }
        }

        public int Gold
        {
            get => _gold;
            set
            {
                _gold = value;
                NotifyResourcesChanged();
            }
        }

        public int Metal
        {
            get => _metal;
            set
            {
                _metal = value;
                NotifyResourcesChanged();
            }
        }

        public int Wood
        {
            get => _wood;
            set
            {
                _wood = value;
                NotifyResourcesChanged();
            }
        }

        public int CurrentTurn
        {
            get => _currentTurn;
            set
            {
                _currentTurn = value;
                NotifyGameStatsChanged();
            }
        }

        public int CapturedCastles
        {
            get => _capturedCastles;
            set
            {
                _capturedCastles = value;
                NotifyGameStatsChanged();
            }
        }

        public int TilesCaptured
        {
            get => _tilesCaptured;
            set
            {
                _tilesCaptured = value;
            }
        }

        private void NotifyResourcesChanged()
        {
            OnResourcesChanged?.Invoke(_influence, _power, _food, _gold, _metal, _wood);
            OnIncomeChanged?.Invoke(); // Income may change when resources change
        }

        private void NotifyGameStatsChanged()
        {
            OnGameStatsChanged?.Invoke(_currentTurn, _capturedCastles);
        }

        public void ResetToDefault(int defaultInfluence, int defaultPower, int defaultFood, 
                                   int defaultGold, int defaultMetal, int defaultWood)
        {
            _influence = defaultInfluence;
            _power = defaultPower;
            _food = defaultFood;
            _gold = defaultGold;
            _metal = defaultMetal;
            _wood = defaultWood;
            
            _currentTurn = 1;
            _capturedCastles = 0;
            _tilesCaptured = 0;
            
            NotifyResourcesChanged();
            NotifyGameStatsChanged();
        }

        public void TriggerIncomeUpdate()
        {
            OnIncomeChanged?.Invoke();
        }
    }
}