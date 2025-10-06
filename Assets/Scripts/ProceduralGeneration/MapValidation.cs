using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace ProceduralGeneration
{
    /// <summary>
    /// Validation utility to ensure generated maps are playable
    /// CRITICAL: Every map MUST have at least one enemy castle (win condition)
    /// </summary>
    public static class MapValidation
    {
        /// <summary>
        /// Validate that map meets minimum playability requirements
        /// </summary>
        public static ValidationResult Validate(Dictionary<Vector3Int, TileType> map)
        {
            var result = new ValidationResult
            {
                IsValid = true,
                Errors = new List<string>(),
                Warnings = new List<string>()
            };

            // CRITICAL: Check for player castle
            int playerCastles = CountTileType(map, TileType.PlayerCastle);
            if (playerCastles == 0)
            {
                result.IsValid = false;
                result.Errors.Add("CRITICAL: No player castle found!");
            }
            else if (playerCastles > 1)
            {
                result.IsValid = false;
                result.Errors.Add($"CRITICAL: Multiple player castles ({playerCastles}) - must be exactly 1!");
            }

            // CRITICAL: Check for enemy castles (WIN CONDITION)
            int enemyCastles = CountTileType(map, TileType.EnemyCastle);
            if (enemyCastles == 0)
            {
                result.IsValid = false;
                result.Errors.Add("CRITICAL: No enemy castles found - NO WIN CONDITION!");
            }
            else
            {
                result.Warnings.Add($"✓ Enemy castles: {enemyCastles}");
            }

            // Warning: Check for minimum tiles
            if (map.Count < 7)
            {
                result.Warnings.Add($"Very small map ({map.Count} tiles) - may not be interesting");
            }

            return result;
        }

        /// <summary>
        /// Validate configuration before generation
        /// </summary>
        public static bool ValidateConfig(MapGenerationConfig config, out string error)
        {
            error = null;

            // CRITICAL: Enemy castle count must be at least 1
            if (config.EnemyCastleCount < 1)
            {
                error = "EnemyCastleCount must be at least 1 (win condition required)!";
                return false;
            }

            // Check map radius
            if (config.MapRadius < 1)
            {
                error = "MapRadius must be at least 1!";
                return false;
            }

            // Check percentages sum to 100%
            float totalPercentage = config.SimpleTerrainPercentage +
                                   config.MountainPercentage +
                                   config.RiverPercentage +
                                   config.FieldPercentage +
                                   config.ForestPercentage +
                                   config.VillagePercentage +
                                   config.MinePercentage +
                                   config.ShaftPercentage;

            if (Mathf.Abs(totalPercentage - 100f) > 0.1f)
            {
                error = $"Tile percentages must sum to 100% (current: {totalPercentage:F1}%)";
                return false;
            }

            return true;
        }

        private static int CountTileType(Dictionary<Vector3Int, TileType> map, TileType type)
        {
            int count = 0;
            foreach (var tileType in map.Values)
            {
                if (tileType == type) count++;
            }
            return count;
        }

        public class ValidationResult
        {
            public bool IsValid;
            public List<string> Errors;
            public List<string> Warnings;

            public void LogResults()
            {
                if (IsValid)
                {
                    Debug.Log("[MapValidation] ✓ Map validation PASSED");
                }
                else
                {
                    Debug.LogError("[MapValidation] ✗ Map validation FAILED!");
                }

                foreach (var error in Errors)
                {
                    Debug.LogError($"[MapValidation] ERROR: {error}");
                }

                foreach (var warning in Warnings)
                {
                    Debug.Log($"[MapValidation] {warning}");
                }
            }
        }
    }
}

