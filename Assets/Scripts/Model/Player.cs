using System;

namespace Model
{
    public class Player
    {
        public event Action<int, int, int, int, int, int> OnResourcesChanged;

        private int _influence;
        private int _power;
        private int _food;
        private int _gold;
        private int _metal;
        private int _wood;

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

        private void NotifyResourcesChanged()
        {
            OnResourcesChanged?.Invoke(_influence, _power, _food, _gold, _metal, _wood);
        }
    }
}