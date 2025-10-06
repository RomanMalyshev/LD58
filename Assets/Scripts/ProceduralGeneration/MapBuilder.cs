using System.Collections.Generic;
using DefaultNamespace;
using RedBjorn.ProtoTiles;
using UnityEngine;

namespace ProceduralGeneration
{
    /// <summary>
    /// Builds Unity map from generated tile data
    /// Instantiates tile prefabs and configures MapSettings
    /// </summary>
    public class MapBuilder
    {
        private MapSettings _mapSettings;
        private Transform _mapParent;
        private Dictionary<TileType, GameObject> _tilePrefabs;

        public MapBuilder(MapSettings mapSettings, Transform mapParent)
        {
            _mapSettings = mapSettings;
            _mapParent = mapParent;
            _tilePrefabs = new Dictionary<TileType, GameObject>();
        }

        /// <summary>
        /// Register tile prefab for a tile type
        /// </summary>
        public void RegisterTilePrefab(TileType type, GameObject prefab)
        {
            _tilePrefabs[type] = prefab;
        }

        /// <summary>
        /// Build map from generated tile data
        /// </summary>
        public void BuildMap(Dictionary<Vector3Int, TileType> generatedTiles)
        {
            Debug.Log($"[MapBuilder] Building map with {generatedTiles.Count} tiles");

            // Clear existing tiles
            ClearExistingTiles();

            // Create new tiles
            int tilesCreated = 0;
            foreach (var kvp in generatedTiles)
            {
                Vector3Int position = kvp.Key;
                TileType tileType = kvp.Value;

                if (CreateTile(position, tileType))
                {
                    tilesCreated++;
                }
            }

            Debug.Log($"[MapBuilder] Successfully created {tilesCreated} tiles");
        }

        /// <summary>
        /// Create a single tile at position
        /// </summary>
        private bool CreateTile(Vector3Int position, TileType tileType)
        {
            // Get prefab for this tile type
            if (!_tilePrefabs.TryGetValue(tileType, out GameObject prefab))
            {
                Debug.LogWarning($"[MapBuilder] No prefab registered for {tileType}");
                return false;
            }

            if (prefab == null)
            {
                Debug.LogWarning($"[MapBuilder] Prefab for {tileType} is null");
                return false;
            }

            // Calculate world position
            Vector3 worldPos = _mapSettings.ToWorld(position, _mapSettings.Edge);

            // Instantiate tile
            GameObject tileGO = Object.Instantiate(prefab, worldPos, Quaternion.identity, _mapParent);
            tileGO.name = $"Tile_{tileType}_{position}";

            // Configure tile component
            var tileComponent = tileGO.GetComponent<Map.Tile>();
            if (tileComponent != null)
            {
                tileComponent.SetPosition(position);
            }
            else
            {
                Debug.LogWarning($"[MapBuilder] Tile prefab {prefab.name} doesn't have Tile component");
            }

            return true;
        }

        /// <summary>
        /// Clear all existing tiles from map
        /// </summary>
        private void ClearExistingTiles()
        {
            if (_mapParent == null) return;

            int childCount = _mapParent.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                var child = _mapParent.GetChild(i);
                if (child.GetComponent<Map.Tile>() != null)
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }

            Debug.Log($"[MapBuilder] Cleared {childCount} existing tiles");
        }

        /// <summary>
        /// Save generated map to MapSettings asset
        /// </summary>
        public void SaveToMapSettings(Dictionary<Vector3Int, TileType> generatedTiles)
        {
#if UNITY_EDITOR
            // Clear existing tile data
            _mapSettings.Tiles.Clear();
            _mapSettings.Presets.Clear();

            // Create presets for each tile type used
            var usedTypes = new HashSet<TileType>();
            foreach (var tileType in generatedTiles.Values)
            {
                usedTypes.Add(tileType);
            }

            var typeToPresetId = new Dictionary<TileType, string>();
            foreach (var tileType in usedTypes)
            {
                if (_tilePrefabs.TryGetValue(tileType, out GameObject prefab))
                {
                    var preset = new TilePreset
                    {
                        Id = UnityEditor.GUID.Generate().ToString(),
                        Type = tileType.ToString(),
                        TileType = tileType,
                        MapColor = GetColorForTileType(tileType),
                        Prefabs = new List<GameObject> { prefab }
                    };
                    
                    _mapSettings.Presets.Add(preset);
                    typeToPresetId[tileType] = preset.Id;
                }
            }

            // Create tile data
            foreach (var kvp in generatedTiles)
            {
                if (typeToPresetId.TryGetValue(kvp.Value, out string presetId))
                {
                    var tileData = new TileData
                    {
                        TilePos = kvp.Key,
                        Id = presetId,
                        PrefabIndex = 0,
                        MovableArea = 0
                    };
                    
                    _mapSettings.Tiles.Add(tileData);
                }
            }

            UnityEditor.EditorUtility.SetDirty(_mapSettings);
            UnityEditor.AssetDatabase.SaveAssets();
            
            Debug.Log($"[MapBuilder] Saved {_mapSettings.Tiles.Count} tiles to MapSettings");
#endif
        }

        /// <summary>
        /// Get visual color for tile type
        /// </summary>
        private Color GetColorForTileType(TileType type)
        {
            return type switch
            {
                TileType.Simple => new Color(0.9f, 0.9f, 0.8f, 0.5f),
                TileType.Field => new Color(1f, 0.9f, 0.3f, 0.5f),
                TileType.Village => new Color(0.8f, 0.6f, 0.4f, 0.5f),
                TileType.EnemyCastle => new Color(0.8f, 0.2f, 0.2f, 0.5f),
                TileType.PlayerCastle => new Color(0.2f, 0.5f, 1f, 0.5f),
                TileType.Forest => new Color(0.2f, 0.7f, 0.3f, 0.5f),
                TileType.Mine => new Color(1f, 0.8f, 0.2f, 0.5f),
                TileType.Shaft => new Color(0.5f, 0.5f, 0.6f, 0.5f),
                TileType.Chest => new Color(1f, 0.7f, 0.2f, 0.5f),
                TileType.Tavern => new Color(0.9f, 0.6f, 0.3f, 0.5f),
                TileType.Camp => new Color(0.7f, 0.4f, 0.2f, 0.5f),
                TileType.River => new Color(0.3f, 0.5f, 0.9f, 0.5f),
                TileType.Mountain => new Color(0.4f, 0.4f, 0.4f, 0.5f),
                _ => Color.white
            };
        }
    }
}

