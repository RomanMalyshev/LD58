using System;
using System.Collections.Generic;
using UnityEngine;

namespace View.Map
{
    public class MapView
    {
        private Dictionary<Vector2Int, TileView> _tileViews = new Dictionary<Vector2Int, TileView>();

        public MapView(Dictionary<Vector3Int, TileView> map)
        {
            foreach (var coordToTile in map)
            {
                var coords = new Vector2Int(coordToTile.Key.y, coordToTile.Key.z);
                var tileView = coordToTile.Value;
                
                _tileViews[coords] = tileView;
                tileView.OnClicked += () => HandleTileClicked(coords);
                tileView.OnHoverEnter += () => HandleTileHoverEnter(coords);
                tileView.OnHoverExit += () => HandleTileHoverExit(coords);
            }
        }

        public event Action<Vector2Int> OnTileClicked;
        public event Action<Vector2Int> OnTileHoverEnter;
        public event Action<Vector2Int> OnTileHoverExit;
        
        private void HandleTileClicked(Vector2Int coords)
        {
            Debug.Log("HandleTileClicked: " + coords);
            OnTileClicked?.Invoke(coords);
        }

        private void HandleTileHoverEnter(Vector2Int coords)
        {
            OnTileHoverEnter?.Invoke(coords);
        }

        private void HandleTileHoverExit(Vector2Int coords)
        {
            OnTileHoverExit?.Invoke(coords);
        }

        public void UpdateTileOccupiedState(Vector2Int coords, bool isOccupied)
        {
            if (_tileViews.ContainsKey(coords))
            {
                _tileViews[coords].SetOccupiedVisual(isOccupied);
            }
        }

        public void UpdateTileReadyToOccupyState(Vector2Int coords, bool isReadyToOccupy)
        {
            if (_tileViews.ContainsKey(coords))
            {
                _tileViews[coords].SetReadyToOccupyVisual(isReadyToOccupy);
            }
        }

        public void SetTilesInteractable(bool interactable)
        {
            foreach (var tileView in _tileViews.Values)
            {
                tileView.SetInteractable(interactable);
            }
        }

        public void HighlightTile(Vector2Int coords, Color color)
        {
            if (_tileViews.ContainsKey(coords))
            {
                _tileViews[coords].SetColor(color);
            }
        }

        public void ClearTileHighlight(Vector2Int coords)
        {
            if (_tileViews.ContainsKey(coords))
            {
                _tileViews[coords].ResetColor();
            }
        }

        public void ClearAllHighlights()
        {
            foreach (var tileView in _tileViews.Values)
            {
                tileView.ResetColor();
            }
        }
    }
}