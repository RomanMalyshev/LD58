using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using RedBjorn.ProtoTiles;
using UnityEngine;

namespace ProceduralGeneration
{
    /// <summary>
    /// Procedural map generator for hexagonal grid
    /// Generates balanced, playable maps with proper resource distribution
    /// </summary>
    public class ProceduralMapGenerator
    {
        private MapGenerationConfig _config;
        private MapSettings _mapSettings;
        private System.Random _random;
        
        private Dictionary<Vector3Int, TileType> _generatedTiles;
        private HashSet<Vector3Int> _occupiedPositions;
        private List<Vector3Int> _enemyCastlePositions;

        public ProceduralMapGenerator(MapGenerationConfig config, MapSettings mapSettings)
        {
            _config = config;
            _mapSettings = mapSettings;
            _generatedTiles = new Dictionary<Vector3Int, TileType>();
            _occupiedPositions = new HashSet<Vector3Int>();
            _enemyCastlePositions = new List<Vector3Int>();
        }

        /// <summary>
        /// Generate a complete map
        /// </summary>
        public Dictionary<Vector3Int, TileType> GenerateMap()
        {
            // Validate configuration BEFORE generation
            if (!MapValidation.ValidateConfig(_config, out string configError))
            {
                Debug.LogError($"[MapGenerator] Invalid configuration: {configError}");
                throw new System.Exception($"Map generation aborted: {configError}");
            }

            // Initialize random with seed
            int seed = _config.Seed == 0 ? Environment.TickCount : _config.Seed;
            _random = new System.Random(seed);
            
            Debug.Log($"[MapGenerator] Generating map with seed: {seed}");
            Debug.Log($"[MapGenerator] Enemy castles to place: {_config.EnemyCastleCount}");

            _generatedTiles.Clear();
            _occupiedPositions.Clear();
            _enemyCastlePositions.Clear();

            // Step 1: Generate all hex positions
            var allPositions = GenerateHexagonalGrid(_config.MapRadius);
            Debug.Log($"[MapGenerator] Generated {allPositions.Count} positions");

            // Step 2: Place player castle at center
            PlacePlayerCastle();

            // Step 3: Place enemy castles (MANDATORY - win condition!)
            PlaceEnemyCastles(allPositions);
            
            // CRITICAL: Ensure at least one enemy castle was placed
            if (_enemyCastlePositions.Count == 0)
            {
                Debug.LogError("[MapGenerator] CRITICAL ERROR: No enemy castles placed! Map is unwinnable!");
                throw new System.Exception("Map generation failed: No enemy castles placed. Increase map radius or reduce MinCastleDistance.");
            }

            // Step 4: Place special tiles (chests, taverns, camps)
            PlaceSpecialTiles(allPositions);

            // Step 5: Generate terrain and resources
            GenerateTerrain(allPositions);

            // Step 6: Ensure map is playable
            if (_config.EnsurePathToResources)
            {
                EnsureConnectivity();
            }

            // Step 7: Post-process (cluster mountains, etc.)
            if (_config.ClusterMountains)
            {
                ClusterSimilarTiles(TileType.Mountain);
            }

            Debug.Log($"[MapGenerator] Map generation complete! Total tiles: {_generatedTiles.Count}");
            
            // Final validation
            var validation = MapValidation.Validate(_generatedTiles);
            validation.LogResults();
            
            if (!validation.IsValid)
            {
                Debug.LogError("[MapGenerator] Generated map failed validation!");
                throw new System.Exception("Map generation produced invalid map. See errors above.");
            }
            
            return _generatedTiles;
        }

        /// <summary>
        /// Generate all positions in hexagonal grid up to radius
        /// </summary>
        private List<Vector3Int> GenerateHexagonalGrid(int radius)
        {
            var positions = new List<Vector3Int>();
            
            for (int q = -radius; q <= radius; q++)
            {
                int r1 = Mathf.Max(-radius, -q - radius);
                int r2 = Mathf.Min(radius, -q + radius);
                
                for (int r = r1; r <= r2; r++)
                {
                    int s = -q - r;
                    positions.Add(new Vector3Int(q, r, s));
                }
            }
            
            return positions;
        }

        /// <summary>
        /// Place player castle at center
        /// </summary>
        private void PlacePlayerCastle()
        {
            Vector3Int center = Vector3Int.zero;
            _generatedTiles[center] = TileType.PlayerCastle;
            _occupiedPositions.Add(center);
            
            // Surrounding tiles should be simple/safe terrain
            foreach (var neighbor in GetNeighbors(center))
            {
                if (!_occupiedPositions.Contains(neighbor))
                {
                    // Random choice between Simple, Field, and Forest for starting area
                    float roll = (float)_random.NextDouble();
                    TileType safeType = roll < 0.5f ? TileType.Simple : (roll < 0.75f ? TileType.Field : TileType.Forest);
                    _generatedTiles[neighbor] = safeType;
                    _occupiedPositions.Add(neighbor);
                }
            }
            
            Debug.Log("[MapGenerator] Player castle placed at center");
        }

        /// <summary>
        /// Place enemy castles at strategic positions
        /// </summary>
        private void PlaceEnemyCastles(List<Vector3Int> allPositions)
        {
            var availablePositions = allPositions
                .Where(pos => !_occupiedPositions.Contains(pos))
                .Where(pos => GetDistanceFromCenter(pos) >= _config.MinCastleDistance)
                .ToList();

            int castlesPlaced = 0;
            int maxAttempts = 100;
            int attempts = 0;

            while (castlesPlaced < _config.EnemyCastleCount && attempts < maxAttempts)
            {
                attempts++;
                
                if (availablePositions.Count == 0) break;

                // Pick random position
                var position = availablePositions[_random.Next(availablePositions.Count)];

                // Check distance from other castles
                bool tooClose = _enemyCastlePositions.Any(castle => 
                    GetDistance(position, castle) < _config.MinCastleDistance);

                if (!tooClose)
                {
                    _generatedTiles[position] = TileType.EnemyCastle;
                    _occupiedPositions.Add(position);
                    _enemyCastlePositions.Add(position);
                    castlesPlaced++;
                    
                    Debug.Log($"[MapGenerator] Enemy castle placed at {position}");

                    // Clear area around enemy castle (make accessible)
                    foreach (var neighbor in GetNeighbors(position))
                    {
                        if (!_occupiedPositions.Contains(neighbor))
                        {
                            _generatedTiles[neighbor] = TileType.Simple;
                            _occupiedPositions.Add(neighbor);
                        }
                    }
                }

                availablePositions.Remove(position);
            }

            Debug.Log($"[MapGenerator] Placed {castlesPlaced} enemy castles (requested: {_config.EnemyCastleCount})");
            
            // Warning if couldn't place all requested castles
            if (castlesPlaced < _config.EnemyCastleCount)
            {
                Debug.LogWarning($"[MapGenerator] Could only place {castlesPlaced}/{_config.EnemyCastleCount} enemy castles. " +
                                "Consider increasing map radius or decreasing MinCastleDistance.");
            }
        }

        /// <summary>
        /// Place special event tiles
        /// </summary>
        private void PlaceSpecialTiles(List<Vector3Int> allPositions)
        {
            PlaceTileType(TileType.Chest, _config.ChestCount, allPositions, 2);
            PlaceTileType(TileType.Tavern, _config.TavernCount, allPositions, 2);
            PlaceTileType(TileType.Camp, _config.CampCount, allPositions, 2);
        }

        /// <summary>
        /// Place specific tile type count times
        /// </summary>
        private void PlaceTileType(TileType type, int count, List<Vector3Int> allPositions, int minDistance)
        {
            var availablePositions = allPositions
                .Where(pos => !_occupiedPositions.Contains(pos))
                .Where(pos => GetDistanceFromCenter(pos) >= minDistance)
                .ToList();

            int placed = 0;
            int maxAttempts = count * 10;
            int attempts = 0;

            while (placed < count && attempts < maxAttempts && availablePositions.Count > 0)
            {
                attempts++;
                var position = availablePositions[_random.Next(availablePositions.Count)];
                
                _generatedTiles[position] = type;
                _occupiedPositions.Add(position);
                placed++;
                
                availablePositions.Remove(position);
            }

            Debug.Log($"[MapGenerator] Placed {placed} {type} tiles");
        }

        /// <summary>
        /// Generate terrain and resource tiles
        /// </summary>
        private void GenerateTerrain(List<Vector3Int> allPositions)
        {
            foreach (var position in allPositions)
            {
                if (_occupiedPositions.Contains(position))
                    continue;

                // Calculate distance-based modifiers
                float distanceFromCenter = GetDistanceFromCenter(position);
                float normalizedDistance = distanceFromCenter / _config.MapRadius;

                // Determine tile type based on config and position
                TileType tileType = DetermineTileType(position, normalizedDistance);
                
                _generatedTiles[position] = tileType;
            }
        }

        /// <summary>
        /// Determine tile type based on position and generation rules
        /// </summary>
        private TileType DetermineTileType(Vector3Int position, float normalizedDistance)
        {
            // Apply resource centralization
            bool allowResources = true;
            if (_config.ResourceCentralization > 0)
            {
                float resourceChance = 1f - (normalizedDistance * _config.ResourceCentralization);
                if (_random.NextDouble() > resourceChance)
                {
                    allowResources = false;
                }
            }

            // Get base tile type
            TileType tileType = _config.GetRandomTileType(_random, allowResources);

            // Apply danger progression (more mountains near edges)
            if (_config.DangerProgression > 0 && normalizedDistance > 0.5f)
            {
                float dangerChance = (normalizedDistance - 0.5f) * 2f * _config.DangerProgression;
                if (_random.NextDouble() < dangerChance && tileType != TileType.Mountain)
                {
                    // Chance to convert to mountain in outer regions
                    if (_random.NextDouble() < 0.3f)
                    {
                        tileType = TileType.Mountain;
                    }
                }
            }

            return tileType;
        }

        /// <summary>
        /// Ensure all important tiles are accessible
        /// </summary>
        private void EnsureConnectivity()
        {
            var importantTiles = _generatedTiles
                .Where(kvp => IsImportantTile(kvp.Value))
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var tile in importantTiles)
            {
                if (!IsConnectedToCenter(tile))
                {
                    CreatePathToCenter(tile);
                }
            }
        }

        /// <summary>
        /// Check if tile type is important (should be accessible)
        /// </summary>
        private bool IsImportantTile(TileType type)
        {
            return type == TileType.EnemyCastle || 
                   type == TileType.Chest || 
                   type == TileType.Tavern || 
                   type == TileType.Camp;
        }

        /// <summary>
        /// Check if position is connected to center
        /// </summary>
        private bool IsConnectedToCenter(Vector3Int target)
        {
            var visited = new HashSet<Vector3Int>();
            var queue = new Queue<Vector3Int>();
            queue.Enqueue(Vector3Int.zero);
            visited.Add(Vector3Int.zero);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                
                if (current == target)
                    return true;

                foreach (var neighbor in GetNeighbors(current))
                {
                    if (!_generatedTiles.ContainsKey(neighbor))
                        continue;
                        
                    if (visited.Contains(neighbor))
                        continue;

                    // Can't pass through mountains
                    if (_generatedTiles[neighbor] == TileType.Mountain)
                        continue;

                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }

            return false;
        }

        /// <summary>
        /// Create a path from target to center by removing mountain obstacles
        /// </summary>
        private void CreatePathToCenter(Vector3Int target)
        {
            var current = target;
            var visited = new HashSet<Vector3Int>();

            while (current != Vector3Int.zero && visited.Count < 1000)
            {
                visited.Add(current);
                
                // Find closest neighbor to center
                var neighbors = GetNeighbors(current)
                    .Where(n => _generatedTiles.ContainsKey(n))
                    .OrderBy(n => GetDistanceFromCenter(n))
                    .ToList();

                if (neighbors.Count == 0) break;

                var next = neighbors[0];

                // If mountain, convert to simple terrain
                if (_generatedTiles[next] == TileType.Mountain)
                {
                    _generatedTiles[next] = TileType.Simple;
                    Debug.Log($"[MapGenerator] Cleared mountain at {next} to create path");
                }

                current = next;
            }
        }

        /// <summary>
        /// Cluster similar tiles together (e.g., mountains)
        /// </summary>
        private void ClusterSimilarTiles(TileType targetType)
        {
            var tilesToModify = new Dictionary<Vector3Int, TileType>();

            foreach (var kvp in _generatedTiles)
            {
                if (kvp.Value != targetType)
                    continue;

                var neighbors = GetNeighbors(kvp.Key);
                foreach (var neighbor in neighbors)
                {
                    if (!_generatedTiles.ContainsKey(neighbor))
                        continue;

                    var neighborType = _generatedTiles[neighbor];
                    
                    // Don't cluster over important tiles
                    if (IsImportantTile(neighborType) || 
                        neighborType == TileType.PlayerCastle)
                        continue;

                    // 30% chance to convert neighbor to same type
                    if (_random.NextDouble() < 0.3f)
                    {
                        tilesToModify[neighbor] = targetType;
                    }
                }
            }

            // Apply modifications
            foreach (var kvp in tilesToModify)
            {
                _generatedTiles[kvp.Key] = kvp.Value;
            }

            Debug.Log($"[MapGenerator] Clustered {tilesToModify.Count} {targetType} tiles");
        }

        /// <summary>
        /// Get all neighbors of a hex tile
        /// </summary>
        private List<Vector3Int> GetNeighbors(Vector3Int position)
        {
            return new List<Vector3Int>
            {
                position + new Vector3Int(1, -1, 0),
                position + new Vector3Int(1, 0, -1),
                position + new Vector3Int(0, 1, -1),
                position + new Vector3Int(-1, 1, 0),
                position + new Vector3Int(-1, 0, 1),
                position + new Vector3Int(0, -1, 1)
            };
        }

        /// <summary>
        /// Get distance from center using cube coordinates
        /// </summary>
        private int GetDistanceFromCenter(Vector3Int position)
        {
            return GetDistance(position, Vector3Int.zero);
        }

        /// <summary>
        /// Get distance between two hex positions
        /// </summary>
        private int GetDistance(Vector3Int a, Vector3Int b)
        {
            return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2;
        }
    }
}

