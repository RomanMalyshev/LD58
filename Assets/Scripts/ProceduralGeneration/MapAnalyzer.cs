using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;

namespace ProceduralGeneration
{
    /// <summary>
    /// Analyzes generated maps for balance and playability
    /// Provides metrics and recommendations
    /// </summary>
    public static class MapAnalyzer
    {
        public class AnalysisResult
        {
            public int TotalTiles;
            public Dictionary<TileType, int> TileDistribution;
            public float ResourceDensity;
            public float CombatDensity;
            public bool IsBalanced;
            public List<string> Warnings;
            public List<string> Recommendations;
        }

        /// <summary>
        /// Analyze a generated map
        /// </summary>
        public static AnalysisResult Analyze(Dictionary<Vector3Int, TileType> map)
        {
            var result = new AnalysisResult
            {
                TotalTiles = map.Count,
                TileDistribution = new Dictionary<TileType, int>(),
                Warnings = new List<string>(),
                Recommendations = new List<string>()
            };

            // Count tile types
            foreach (var tileType in map.Values)
            {
                if (!result.TileDistribution.ContainsKey(tileType))
                    result.TileDistribution[tileType] = 0;
                result.TileDistribution[tileType]++;
            }

            // Calculate metrics
            CalculateResourceDensity(result);
            CalculateCombatDensity(result);
            CheckBalance(result);
            GenerateRecommendations(result);

            return result;
        }

        private static void CalculateResourceDensity(AnalysisResult result)
        {
            int resourceTiles = 0;
            resourceTiles += GetTileCount(result, TileType.Field);
            resourceTiles += GetTileCount(result, TileType.Forest);
            resourceTiles += GetTileCount(result, TileType.Mine);
            resourceTiles += GetTileCount(result, TileType.Shaft);
            resourceTiles += GetTileCount(result, TileType.Village);

            result.ResourceDensity = (float)resourceTiles / result.TotalTiles;
        }

        private static void CalculateCombatDensity(AnalysisResult result)
        {
            int combatTiles = 0;
            combatTiles += GetTileCount(result, TileType.EnemyCastle);
            combatTiles += GetTileCount(result, TileType.Camp);

            result.CombatDensity = (float)combatTiles / result.TotalTiles;
        }

        private static void CheckBalance(AnalysisResult result)
        {
            result.IsBalanced = true;

            // Check for player castle
            if (GetTileCount(result, TileType.PlayerCastle) != 1)
            {
                result.Warnings.Add("Map must have exactly 1 player castle!");
                result.IsBalanced = false;
            }

            // Check for enemy castles (CRITICAL - win condition!)
            int enemyCastles = GetTileCount(result, TileType.EnemyCastle);
            if (enemyCastles == 0)
            {
                result.Warnings.Add("❌ CRITICAL: Map has no enemy castles - NO WIN CONDITION!");
                result.IsBalanced = false;
            }
            else if (enemyCastles == 1)
            {
                result.Recommendations.Add($"Only {enemyCastles} enemy castle - game may be too short");
            }
            else if (enemyCastles > 4)
            {
                result.Warnings.Add($"Many enemy castles ({enemyCastles}) - may be too difficult!");
            }
            else
            {
                // Good number of castles
                Debug.Log($"[MapAnalyzer] ✓ Enemy castles: {enemyCastles} (balanced)");
            }

            // Check resource balance
            if (result.ResourceDensity < 0.3f)
            {
                result.Warnings.Add($"Low resource density ({result.ResourceDensity:P0}) - may be too difficult");
            }
            else if (result.ResourceDensity > 0.7f)
            {
                result.Warnings.Add($"High resource density ({result.ResourceDensity:P0}) - may be too easy");
            }

            // Check for mountains
            int mountains = GetTileCount(result, TileType.Mountain);
            float mountainPercentage = (float)mountains / result.TotalTiles;
            if (mountainPercentage > 0.25f)
            {
                result.Warnings.Add("Too many mountains - may block progression!");
            }

            // Check for variety
            if (result.TileDistribution.Count < 5)
            {
                result.Warnings.Add("Low tile variety - map may be repetitive");
            }
        }

        private static void GenerateRecommendations(AnalysisResult result)
        {
            // Resource recommendations
            if (GetTileCount(result, TileType.Field) < 3)
            {
                result.Recommendations.Add("Add more Fields for food production");
            }

            if (GetTileCount(result, TileType.Forest) < 3)
            {
                result.Recommendations.Add("Add more Forests for wood production");
            }

            if (GetTileCount(result, TileType.Village) == 0)
            {
                result.Recommendations.Add("Consider adding Villages for power generation");
            }

            // Event recommendations
            int events = GetTileCount(result, TileType.Chest) +
                        GetTileCount(result, TileType.Tavern) +
                        GetTileCount(result, TileType.Camp);
            
            if (events < 3)
            {
                result.Recommendations.Add("Add more event tiles (Chests, Taverns, Camps) for variety");
            }

            // Accessibility recommendations
            int rivers = GetTileCount(result, TileType.River);
            if (rivers > result.TotalTiles * 0.1f)
            {
                result.Recommendations.Add("Many rivers - ensure player has enough wood for bridges");
            }
        }

        private static int GetTileCount(AnalysisResult result, TileType type)
        {
            return result.TileDistribution.TryGetValue(type, out int count) ? count : 0;
        }

        /// <summary>
        /// Print analysis report to console
        /// </summary>
        public static void PrintReport(AnalysisResult result)
        {
            Debug.Log("=== MAP ANALYSIS REPORT ===");
            Debug.Log($"Total Tiles: {result.TotalTiles}");
            Debug.Log($"Resource Density: {result.ResourceDensity:P1}");
            Debug.Log($"Combat Density: {result.CombatDensity:P1}");
            Debug.Log($"Balanced: {(result.IsBalanced ? "✓ Yes" : "✗ No")}");
            
            Debug.Log("\n--- Tile Distribution ---");
            foreach (var kvp in result.TileDistribution.OrderByDescending(x => x.Value))
            {
                float percentage = (float)kvp.Value / result.TotalTiles * 100f;
                Debug.Log($"{kvp.Key}: {kvp.Value} ({percentage:F1}%)");
            }

            if (result.Warnings.Count > 0)
            {
                Debug.Log("\n⚠ WARNINGS:");
                foreach (var warning in result.Warnings)
                {
                    Debug.LogWarning(warning);
                }
            }

            if (result.Recommendations.Count > 0)
            {
                Debug.Log("\n💡 RECOMMENDATIONS:");
                foreach (var recommendation in result.Recommendations)
                {
                    Debug.Log($"  • {recommendation}");
                }
            }

            Debug.Log("=========================");
        }
    }
}

