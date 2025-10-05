using Model.Map;
using UnityEngine;
using View.Map;

namespace Presenters
{
    public class MapPresenter
    {
        private readonly MapModel _model;
        private readonly MapView _view;

        public MapPresenter(MapModel model, MapView view)
        {
            _model = model;
            _view = view;
            
            _view.OnTileClicked += HandleTileClicked;
            _view.OnTileHoverEnter += HandleTileHoverEnter;
            _view.OnTileHoverExit += HandleTileHoverExit;
            _model.OnTileOccupiedChanged += HandleTileOccupiedChanged;
            _model.OnTileReadyToOccupyChanged += HandleTileReadyToOccupyChanged;
            
            InitializeView();
            
            _model.OccupyTile(Vector2Int.zero); //player always start with (0,0)
        }

        private void InitializeView()
        {
            
            foreach (var tile in _model.Tiles)
            {
                _view.UpdateTileOccupiedState(tile.Key, tile.Value.IsOccupied);
                _view.UpdateTileReadyToOccupyState(tile.Key, tile.Value.IsReadyToOccupy);
            }
        }

        private void HandleTileClicked(Vector2Int tileCoords)
        {
            if (_model.Tiles.ContainsKey(tileCoords))
            {
                var tile = _model.Tiles[tileCoords];
                
                if (tile.IsOccupied)
                {
                    _model.DeOccupyTile(tileCoords);
                }
                else
                {
                    _model.OccupyTile(tileCoords);
                }
            }
        }

        private void HandleTileHoverEnter(Vector2Int tileCoords)
        {
            Debug.Log($"Hover enter on tile: {tileCoords}");
        }

        private void HandleTileHoverExit(Vector2Int tileCoords)
        {
            Debug.Log($"Hover exit from tile: {tileCoords}");
        }

        private void HandleTileOccupiedChanged(Vector2Int tileCoords, bool isOccupied)
        {
            _view.UpdateTileOccupiedState(tileCoords, isOccupied);
        }

        private void HandleTileReadyToOccupyChanged(Vector2Int tileCoords, bool isReadyToOccupy)
        {
            _view.UpdateTileReadyToOccupyState(tileCoords, isReadyToOccupy);
        }

        public void Cleanup()
        {
           
            _view.OnTileClicked -= HandleTileClicked;
            _view.OnTileHoverEnter -= HandleTileHoverEnter;
            _view.OnTileHoverExit -= HandleTileHoverExit;
            _model.OnTileOccupiedChanged -= HandleTileOccupiedChanged;
            _model.OnTileReadyToOccupyChanged -= HandleTileReadyToOccupyChanged;
        }
    }
}

