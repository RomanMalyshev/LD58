using System;
using UnityEngine;

namespace View.Map
{
    public class TileView : MonoBehaviour
    {
        public event Action OnClicked;
        public event Action OnHoverEnter;
        public event Action OnHoverExit;

        [SerializeField] private Vector3Int _tilePosition;

        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _occupiedColor = Color.red;
        [SerializeField] private Color _hoverColor = Color.yellow;
        [SerializeField] private Color _readyToOccupy = Color.green;

        [SerializeField] private MouseTrigger _mouseTrigger;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private bool _isOccupied;
        private bool _isHovered;
        private bool _isReadyToOccupy;
        private bool _hasCustomColor;
        private Color _customColor;
        private bool _isInteractable = true;

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

        /// <summary>
        /// Установить произвольный цвет тайла (перекрывает все другие состояния)
        /// </summary>
        public void SetColor(Color color)
        {
            _hasCustomColor = true;
            _customColor = color;
            _spriteRenderer.color = color;
        }

        /// <summary>
        /// Сбросить цвет к нормальному состоянию
        /// </summary>
        public void ResetColor()
        {
            _hasCustomColor = false;
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            // Если установлен кастомный цвет, используем его
            if (_hasCustomColor)
            {
                _spriteRenderer.color = _customColor;
                return;
            }

            if (_isHovered)
            {
                _spriteRenderer.color = _hoverColor;
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
    }
}