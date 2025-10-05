using System;
using System.Collections.Generic;
using RedBjorn.ProtoTiles;
using UnityEngine;
using Configs;

namespace Map
{
    public class MapController
    {
        public Action OnSelectedTile;
        public int MaxRadius {get; private set; }
        private Dictionary<Vector2Int, Tile> _coordToTiles = new();
        private Dictionary<int, List<Tile>> _radiusToTiles = new();
        private List<Tile> _occupiedTiles = new();
        private List<Tile> _availableForOccupyTiles = new();
        private List<Tile> _canBeUpgradeTiles = new();
        
        private TilesConfig _tilesConfig;
        
        private Tile _selectedTile;

        private readonly Vector2Int[] _neighborOffsets = new[]
        {
            new Vector2Int(1, -1), // (0, 1, -1)
            new Vector2Int(0, -1), // (1, 0, -1)
            new Vector2Int(-1, 0), // (1, -1, 0)
            new Vector2Int(-1, 1), // (0, -1, 1)
            new Vector2Int(0, 1), // (-1, 0, 1)
            new Vector2Int(1, 0), // (-1, 1, 0)
        };

        public MapController(MapSettings mapEditorSettings, MapView mapEditorView, TilesConfig tilesConfig)
        {
            _tilesConfig = tilesConfig;
            
            MapEntity mapEntity = new MapEntity(mapEditorSettings, mapEditorView);
            foreach (var tile in mapEntity.Tiles)
            {
                //TODO: create transfer function
                //if (tile.Value.Preset.Id == "Forest")
                //  mapModel.Tiles[new Vector2Int(tile.Key.y, tile.Key.z)] = new EmptyTile();
            }
            
            foreach (var tile in mapEditorView.Tiles)
            {
                _coordToTiles[new Vector2Int(tile.Key.y, tile.Key.z)] = tile.Value;
                
                // Initialize tile config
                var config = _tilesConfig.GetConfigByType(tile.Value.TileType);
                if (config != null)
                {
                    tile.Value.InitializeConfig(config);
                }
                
                tile.Value.OnClicked += () =>
                {
                    
                    
                    
                };
            }

            _radiusToTiles = mapEditorView.GetTilesByRadius(Vector3Int.zero);
            Debug.Log(_radiusToTiles.Count);
            MaxRadius = _radiusToTiles.Count;
            //player start with center tile
            OccupyTile(Vector2Int.zero);
        }

        public void SetTilesInteractionState(bool state)
        {
            foreach (var tile in _coordToTiles.Values)
            {
                tile.SetInteractable(state);
            }
        }

        public void SetTilesForOccupyInteractionState(bool state)
        {
            UpdateAvailableForOccupyTiles();
            foreach (var tile in _availableForOccupyTiles)
            {
                tile.SetInteractable(state);
            }
        }

        private void UpdateAvailableForOccupyTiles()
        {
            _availableForOccupyTiles.Clear();
            foreach (var occupiedTile in _occupiedTiles)
            {
                foreach (var offset in _neighborOffsets)
                {
                    var position =occupiedTile.GetTilePosition();
                    var neighborCoords = new Vector2Int(position.y, position.z) + offset;
                    var neighborTile = _coordToTiles.ContainsKey(neighborCoords) ? _coordToTiles[neighborCoords] : null;
                    
                    // Check if tile exists, is not occupied, and can be occupied
                    if (neighborTile != null && 
                        !neighborTile.IsOccupied && 
                        neighborTile.Config != null && 
                        neighborTile.Config.CanBeOccupied)
                    {
                        neighborTile.SetReadyToOccupy(true);
                        _availableForOccupyTiles.Add(neighborTile);
                    }
                }
            }
        }

        public void HideAtRadius(int radius)
        {
            _radiusToTiles[radius].ForEach(tile => tile.gameObject.SetActive(false));
        }
        
        public void ShowAtRadius(int radius)
        {
            _radiusToTiles[radius].ForEach(tile => tile.gameObject.SetActive(true));
        }


        public void OccupyTile(Vector2Int tileCoords)
        {
            _coordToTiles[tileCoords].SetOccupiedVisual(true);
            _occupiedTiles.Add(_coordToTiles[tileCoords]);
        }

        public void DeOccupyTile(Vector2Int tileCoords)
        {
            _coordToTiles[tileCoords].SetOccupiedVisual(false);
            _occupiedTiles.Remove(_coordToTiles[tileCoords]);
        }

        public List<Tile> GetOccupiedTiles()
        {
            return _occupiedTiles;
        }

        public List<Tile> GetUpgradeableTiles()
        {
            _canBeUpgradeTiles.Clear();
            foreach (var tile in _occupiedTiles)
            {
                if (tile.Config != null && tile.Config.CanBeUpgraded)
                {
                    _canBeUpgradeTiles.Add(tile);
                }
            }
            return _canBeUpgradeTiles;
        }

        public Tile GetTileAt(Vector2Int coords)
        {
            return _coordToTiles.ContainsKey(coords) ? _coordToTiles[coords] : null;
        }
    
    }
}