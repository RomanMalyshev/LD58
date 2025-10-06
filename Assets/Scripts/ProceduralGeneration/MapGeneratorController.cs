using System.Collections.Generic;
using DefaultNamespace;
using RedBjorn.ProtoTiles;
using UnityEngine;

namespace ProceduralGeneration
{
    /// <summary>
    /// Main controller for procedural map generation
    /// Use this component to generate maps at runtime or in editor
    /// </summary>
    public class MapGeneratorController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MapSettings _mapSettings;
        [SerializeField] private MapView _mapView;
        [SerializeField] private MapGenerationConfig _generationConfig;

        [Header("Tile Prefabs")]
        [SerializeField] private GameObject _simplePrefab;
        [SerializeField] private GameObject _fieldPrefab;
        [SerializeField] private GameObject _villagePrefab;
        [SerializeField] private GameObject _enemyCastlePrefab;
        [SerializeField] private GameObject _playerCastlePrefab;
        [SerializeField] private GameObject _forestPrefab;
        [SerializeField] private GameObject _minePrefab;
        [SerializeField] private GameObject _shaftPrefab;
        [SerializeField] private GameObject _chestPrefab;
        [SerializeField] private GameObject _tavernPrefab;
        [SerializeField] private GameObject _campPrefab;
        [SerializeField] private GameObject _riverPrefab;
        [SerializeField] private GameObject _mountainPrefab;

        [Header("Generation Options")]
        [SerializeField] private bool _generateOnStart = false;
        [SerializeField] private bool _saveToMapSettings = false;

        private ProceduralMapGenerator _generator;
        private MapBuilder _builder;
        private Dictionary<Vector3Int, TileType> _lastGeneratedMap;

        private void Start()
        {
            if (_generateOnStart)
            {
                GenerateNewMap();
            }
        }

        /// <summary>
        /// Generate a new map
        /// </summary>
        [ContextMenu("Generate New Map")]
        public void GenerateNewMap()
        {
            if (!ValidateReferences())
            {
                Debug.LogError("[MapGeneratorController] Cannot generate map - missing references!");
                return;
            }

            Debug.Log("[MapGeneratorController] Starting map generation...");

            // Create generator
            _generator = new ProceduralMapGenerator(_generationConfig, _mapSettings);

            // Generate map data
            _lastGeneratedMap = _generator.GenerateMap();

            // Build physical map
            BuildGeneratedMap();

            Debug.Log("[MapGeneratorController] Map generation complete!");
        }

        /// <summary>
        /// Build the generated map in the scene
        /// </summary>
        private void BuildGeneratedMap()
        {
            if (_lastGeneratedMap == null || _lastGeneratedMap.Count == 0)
            {
                Debug.LogError("[MapGeneratorController] No map data to build!");
                return;
            }

            // Create builder
            _builder = new MapBuilder(_mapSettings, _mapView.transform);

            // Register all tile prefabs
            RegisterTilePrefabs();

            // Build the map
            _builder.BuildMap(_lastGeneratedMap);

            // Optionally save to MapSettings
            if (_saveToMapSettings)
            {
                _builder.SaveToMapSettings(_lastGeneratedMap);
            }

            // Refresh MapView
            if (_mapView != null)
            {
                _mapView.Awake();
            }
        }

        /// <summary>
        /// Register all tile prefabs with builder
        /// </summary>
        private void RegisterTilePrefabs()
        {
            if (_simplePrefab != null) _builder.RegisterTilePrefab(TileType.Simple, _simplePrefab);
            if (_fieldPrefab != null) _builder.RegisterTilePrefab(TileType.Field, _fieldPrefab);
            if (_villagePrefab != null) _builder.RegisterTilePrefab(TileType.Village, _villagePrefab);
            if (_enemyCastlePrefab != null) _builder.RegisterTilePrefab(TileType.EnemyCastle, _enemyCastlePrefab);
            if (_playerCastlePrefab != null) _builder.RegisterTilePrefab(TileType.PlayerCastle, _playerCastlePrefab);
            if (_forestPrefab != null) _builder.RegisterTilePrefab(TileType.Forest, _forestPrefab);
            if (_minePrefab != null) _builder.RegisterTilePrefab(TileType.Mine, _minePrefab);
            if (_shaftPrefab != null) _builder.RegisterTilePrefab(TileType.Shaft, _shaftPrefab);
            if (_chestPrefab != null) _builder.RegisterTilePrefab(TileType.Chest, _chestPrefab);
            if (_tavernPrefab != null) _builder.RegisterTilePrefab(TileType.Tavern, _tavernPrefab);
            if (_campPrefab != null) _builder.RegisterTilePrefab(TileType.Camp, _campPrefab);
            if (_riverPrefab != null) _builder.RegisterTilePrefab(TileType.River, _riverPrefab);
            if (_mountainPrefab != null) _builder.RegisterTilePrefab(TileType.Mountain, _mountainPrefab);
        }

        /// <summary>
        /// Validate that all required references are set
        /// </summary>
        private bool ValidateReferences()
        {
            bool valid = true;

            if (_mapSettings == null)
            {
                Debug.LogError("[MapGeneratorController] MapSettings not assigned!");
                valid = false;
            }

            if (_mapView == null)
            {
                Debug.LogError("[MapGeneratorController] MapView not assigned!");
                valid = false;
            }

            if (_generationConfig == null)
            {
                Debug.LogError("[MapGeneratorController] MapGenerationConfig not assigned!");
                valid = false;
            }

            // Check essential prefabs
            if (_playerCastlePrefab == null)
            {
                Debug.LogWarning("[MapGeneratorController] PlayerCastle prefab not assigned!");
            }

            if (_simplePrefab == null)
            {
                Debug.LogWarning("[MapGeneratorController] Simple tile prefab not assigned!");
            }

            return valid;
        }

        /// <summary>
        /// Clear all generated tiles
        /// </summary>
        [ContextMenu("Clear Map")]
        public void ClearMap()
        {
            if (_mapView == null) return;

            int childCount = _mapView.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                var child = _mapView.transform.GetChild(i);
                if (child.GetComponent<Map.Tile>() != null)
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            Debug.Log($"[MapGeneratorController] Cleared {childCount} tiles");
        }

        /// <summary>
        /// Get statistics about last generated map
        /// </summary>
        [ContextMenu("Show Map Statistics")]
        public void ShowMapStatistics()
        {
            if (_lastGeneratedMap == null || _lastGeneratedMap.Count == 0)
            {
                Debug.Log("[MapGeneratorController] No map generated yet");
                return;
            }

            // Use MapAnalyzer for comprehensive analysis
            var analysis = MapAnalyzer.Analyze(_lastGeneratedMap);
            MapAnalyzer.PrintReport(analysis);
        }

        /// <summary>
        /// Generate map with new random seed
        /// </summary>
        [ContextMenu("Generate with Random Seed")]
        public void GenerateWithRandomSeed()
        {
            if (_generationConfig != null)
            {
                _generationConfig.Seed = 0; // Random seed
            }
            GenerateNewMap();
        }
    }
}

