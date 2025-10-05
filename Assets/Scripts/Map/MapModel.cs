using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Map
{
    public class MapModel
    {
        public Dictionary<Vector2Int, ITile> Tiles { get;  } = new();
        public event Action<Vector2Int, bool> OnTileOccupiedChanged;
        public event Action<Vector2Int, bool> OnTileReadyToOccupyChanged;

        private readonly Vector2Int[] _neighborOffsets = new[]
        {
            new Vector2Int(1, -1),   // (0, 1, -1)
            new Vector2Int(0, -1),   // (1, 0, -1)
            new Vector2Int(-1, 0),   // (1, -1, 0)
            new Vector2Int(-1, 1),   // (0, -1, 1)
            new Vector2Int(0, 1),    // (-1, 0, 1)
            new Vector2Int(1, 0),    // (-1, 1, 0)
        };

        public void OccupyTile(Vector2Int tileCoords)
        {
            if (Tiles.ContainsKey(tileCoords))
            {
                Tiles[tileCoords].SetOccupied(true);
                OnTileOccupiedChanged?.Invoke(tileCoords, true);
                UpdateReadyToOccupyStates();
            }
        }
        
        public void DeOccupyTile(Vector2Int tileCoords)
        {
            if (Tiles.ContainsKey(tileCoords))
            {
                Tiles[tileCoords].SetOccupied(false);
                OnTileOccupiedChanged?.Invoke(tileCoords, false);
                UpdateReadyToOccupyStates();
            }
        }
        

        private void UpdateReadyToOccupyStates()
        {
            foreach (var tile in Tiles)
            {
                if (!tile.Value.IsOccupied)
                {
                    bool wasReady = tile.Value.IsReadyToOccupy;
                    bool isReady = IsAdjacentToOccupiedTile(tile.Key);
                    
                    if (wasReady != isReady)
                    {
                        tile.Value.SetReadyToOccupy(isReady);
                        OnTileReadyToOccupyChanged?.Invoke(tile.Key, isReady);
                    }
                }
                else
                {
                    if (tile.Value.IsReadyToOccupy)
                    {
                        tile.Value.SetReadyToOccupy(false);
                        OnTileReadyToOccupyChanged?.Invoke(tile.Key, false);
                    }
                }
            }
        }

        private bool IsAdjacentToOccupiedTile(Vector2Int tileCoords)
        {
            foreach (var offset in _neighborOffsets)
            {
                var neighborCoords = tileCoords + offset;
                if (Tiles.ContainsKey(neighborCoords) && Tiles[neighborCoords].IsOccupied)
                {
                    return true;
                }
            }
            return false;
        }
    }
}