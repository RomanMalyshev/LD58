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

        [SerializeField] private Color _normalColor = new (0f, 0f, 0f, 0f);
        [SerializeField] private Color _occupiedColor = Color.red;
        [SerializeField] private Color _hoverColor = Color.yellow;
        [SerializeField] private Color _readyToOccupy = Color.green;
        [SerializeField] private Color _selectedColor = Color.blue;

        [SerializeField] private Color _normalTileColor =  new (0.9f, 0.9f, 0.9f, 0.85f);
        [SerializeField]private float _normalTileAlpha = 0.8f;
        [SerializeField]private float _accentTileAlpha = 1f;
        [SerializeField]private SpriteRenderer[] _tileSprites;
        [SerializeField]private SpriteRenderer[] _spritesForReorder;
        
        [SerializeField] private MouseTrigger _mouseTrigger;
        [SerializeField] private SpriteRenderer _spriteRenderer;

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
            SetTileColor(_normalTileColor);

        }

        private void SetTileColor(Color color)
        {
            foreach (var spriteRenderer in _tileSprites)
            {
                spriteRenderer.color = color;
            }
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
            for (var i = 0; i < _spritesForReorder.Length; i++)
            {
                var spriteRenderer = _spritesForReorder[i];
                spriteRenderer.sortingOrder = yCount - _tilePosition.y -i-4;
            }
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
            IsOccupied = isOccupied;
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
                
                SetTileColor(Color.white);
                _spriteRenderer.color = _hoverColor;
            }
            else if (_isSelected)
            {
                SetTileColor(Color.white);
                _spriteRenderer.color = _selectedColor;
            }
            else if (IsOccupied)
            {
                _spriteRenderer.color = _occupiedColor;
            }
            else if (_isReadyToOccupy)
            {
                _spriteRenderer.color = _readyToOccupy;
            }
            else
            {
                SetTileColor(_normalTileColor);
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

        public void ResetUpgradeLevel()
        {
            _upgradeLevel = 0;
        }
    }
}