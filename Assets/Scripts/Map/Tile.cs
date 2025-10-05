using System;
using UnityEngine;
using View.Map;
using DefaultNamespace;
using Configs;

namespace Map
{
    public class Tile : MonoBehaviour
    {
        public event Action OnClicked;
        public event Action OnHoverEnter;
        public event Action OnHoverExit;

        [SerializeField] private Vector3Int _tilePosition;
        [SerializeField] private TileType _tileType;

        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _occupiedColor = Color.red;
        [SerializeField] private Color _hoverColor = Color.yellow;
        [SerializeField] private Color _readyToOccupy = Color.green;
        [SerializeField] private Color _selectedColor = Color.blue;

        [SerializeField] private MouseTrigger _mouseTrigger;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private bool _isOccupied;
        private bool _isHovered;
        private bool _isReadyToOccupy;
        private bool _isSelected;
        private bool _isInteractable = true;
        private int _upgradeLevel = 0;
        private TileConfig _config;

        public bool IsOccupied { get; set; }
        public bool IsReadyToOccupy { get; set; }
        public TileType TileType => _tileType;
        public int UpgradeLevel => _upgradeLevel;
        public TileConfig Config => _config;

        private void Awake()
        {
            _mouseTrigger.OnClicked += OnMouseDown;
            _mouseTrigger.OnHoverEnter += OnMouseEnter;
            _mouseTrigger.OnHoverExit += OnMouseExit;
        }

        public void SetPosition(Vector3Int tilePos)
        {
            _tilePosition = tilePos;
        }

        public Vector3Int GetTilePosition()
        {
            return _tilePosition;
        }

        public void UpdateOrderLayer(int yCount)
        {
            _spriteRenderer.sortingOrder = yCount - _tilePosition.y;
        }

        public void OnMouseEnter()
        {
            if (!_isInteractable) return;

            _isHovered = true;
            OnHoverEnter?.Invoke();
            UpdateVisual();
        }

        public void OnMouseExit()
        {
            if (!_isInteractable) return;

            _isHovered = false;
            OnHoverExit?.Invoke();
            UpdateVisual();
        }

        public void OnMouseDown()
        {
            if (!_isInteractable) return;

            OnClicked?.Invoke();
        }

        public void SetInteractable(bool interactable)
        {
            _isInteractable = interactable;
            if (!_isInteractable && _isHovered)
            {
                _isHovered = false;
                UpdateVisual();
            }

            if (_isInteractable)
                UpdateVisual();
        }

        public void SetOccupiedVisual(bool isOccupied)
        {
            _isOccupied = isOccupied;
            UpdateVisual();
        }

        public void SetReadyToOccupyVisual(bool isReadyToOccupy)
        {
            _isReadyToOccupy = isReadyToOccupy;
            UpdateVisual();
        }


        private void UpdateVisual()
        {
            if (_isHovered)
            {
                _spriteRenderer.color = _hoverColor;
            }
            else if (_isSelected)
            {
                _spriteRenderer.color = _selectedColor;
            }
            else if (_isOccupied)
            {
                _spriteRenderer.color = _occupiedColor;
            }
            else if (_isReadyToOccupy)
            {
                _spriteRenderer.color = _readyToOccupy;
            }
            else
            {
                _spriteRenderer.color = _normalColor;
            }
        }

        public void SetReadyToOccupy(bool isReady)
        {
            _isReadyToOccupy = isReady;
            UpdateVisual();
        }

        public void InitializeConfig(TileConfig config)
        {
            _config = config;
        }

        public void UpgradeTile()
        {
            if (_config != null && _config.CanBeUpgraded)
            {
                _upgradeLevel++;
            }
        }

        public int GetTotalFoodIncome()
        {
            if (_config == null) return 0;
            return _config.IncomeFood + (_config.UpgradeFoodBonus * _upgradeLevel);
        }

        public int GetTotalPowerIncome()
        {
            if (_config == null) return 0;
            return _config.IncomePower + (_config.UpgradePowerBonus * _upgradeLevel);
        }

        public int GetTotalWoodIncome()
        {
            if (_config == null) return 0;
            return _config.IncomeWood + (_config.UpgradeWoodBonus * _upgradeLevel);
        }

        public int GetTotalGoldIncome()
        {
            if (_config == null) return 0;
            return _config.IncomeGold + (_config.UpgradeGoldBonus * _upgradeLevel);
        }

        public int GetTotalMetalIncome()
        {
            if (_config == null) return 0;
            return _config.IncomeMetal + (_config.UpgradeMetalBonus * _upgradeLevel);
        }

        public void SetSelected(bool isSelected)
        {
            _isSelected = isSelected;
            UpdateVisual();
        }
    }
}