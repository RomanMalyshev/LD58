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

        private void Awake()
        {
            _mouseTrigger.OnClicked += OnMouseDown;
            _mouseTrigger.OnHoverEnter += OnMouseEnter;
            _mouseTrigger.OnHoverExit += OnMouseExit;
            _spriteRenderer.sortingOrder = 20 - _tilePosition.y;
        }

        public void SetPosition(Vector3Int tilePos)
        {
            _tilePosition = tilePos;
        }

        public Vector3Int GetTilePosition()
        {
            return _tilePosition;
        }

        public void OnMouseEnter()
        {
            _isHovered = true;
            OnHoverEnter?.Invoke();
            UpdateVisual();
        }

        public void OnMouseExit()
        {
            _isHovered = false;
            OnHoverExit?.Invoke();
            UpdateVisual();
        }

        public void OnMouseDown()
        {
            OnClicked?.Invoke();
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