using System;
using DefaultNamespace;
using UnityEngine;

namespace ProceduralGeneration
{
    /// <summary>
    /// Configuration for procedural map generation
    /// </summary>
    [CreateAssetMenu(fileName = "MapGenerationConfig", menuName = "Configs/Map Generation Config")]
    public class MapGenerationConfig : ScriptableObject
    {
        [Header("Map Size")]
        [Tooltip("Radius of the generated map (0 = center only, 1 = center + 1 ring, etc.)")]
        [Range(1, 10)]
        public int MapRadius = 5;

        [Header("Terrain Distribution (Percentages)")]
        [Range(0f, 100f)] public float SimpleTerrainPercentage = 30f;
        [Range(0f, 100f)] public float MountainPercentage = 15f;
        [Range(0f, 100f)] public float RiverPercentage = 5f;

        [Header("Resource Tiles Distribution")]
        [Range(0f, 100f)] public float FieldPercentage = 15f;
        [Range(0f, 100f)] public float ForestPercentage = 15f;
        [Range(0f, 100f)] public float VillagePercentage = 10f;
        [Range(0f, 100f)] public float MinePercentage = 5f;
        [Range(0f, 100f)] public float ShaftPercentage = 5f;

        [Header("Special Tiles")]
        [Tooltip("Enemy castles are WIN CONDITION - must be at least 1!")]
        [Range(1, 5)] public int EnemyCastleCount = 2;
        [Range(2, 8)] public int MinCastleDistance = 4;
        [Range(0, 10)] public int ChestCount = 3;
        [Range(0, 5)] public int TavernCount = 2;
        [Range(0, 5)] public int CampCount = 2;

        [Header("Generation Rules")]
        [Tooltip("Ensure no mountains block access to important tiles")]
        public bool EnsurePathToResources = true;
        
        [Tooltip("Mountains tend to form clusters")]
        public bool ClusterMountains = true;
        
        [Tooltip("Resources are distributed evenly across map")]
        public bool DistributeResourcesEvenly = true;
        
        [Tooltip("Random seed for generation (0 = random)")]
        public int Seed = 0;

        [Header("Balance Settings")]
        [Range(0f, 1f)]
        [Tooltip("Higher = more resources near center")]
        public float ResourceCentralization = 0.3f;
        
        [Range(0f, 1f)]
        [Tooltip("Higher = more dangerous tiles near edges")]
        public float DangerProgression = 0.7f;

        /// <summary>
        /// Validate percentages sum
        /// </summary>
        public void OnValidate()
        {
            float totalPercentage = SimpleTerrainPercentage + MountainPercentage + RiverPercentage +
                                   FieldPercentage + ForestPercentage + VillagePercentage +
                                   MinePercentage + ShaftPercentage;

            if (Mathf.Abs(totalPercentage - 100f) > 0.1f)
            {
                Debug.LogWarning($"MapGenerationConfig: Total percentage is {totalPercentage}%, should be 100%");
            }
        }

        /// <summary>
        /// Get tile type based on probability weights
        /// </summary>
        public TileType GetRandomTileType(System.Random random, bool allowResources = true)
        {
            float roll = (float)(random.NextDouble() * 100f);
            float cumulative = 0f;

            // Simple terrain
            cumulative += SimpleTerrainPercentage;
            if (roll < cumulative) return TileType.Simple;

            // Mountains
            cumulative += MountainPercentage;
            if (roll < cumulative) return TileType.Mountain;

            // Rivers
            cumulative += RiverPercentage;
            if (roll < cumulative) return TileType.River;

            if (!allowResources) return TileType.Simple;

            // Fields
            cumulative += FieldPercentage;
            if (roll < cumulative) return TileType.Field;

            // Forests
            cumulative += ForestPercentage;
            if (roll < cumulative) return TileType.Forest;

            // Villages
            cumulative += VillagePercentage;
            if (roll < cumulative) return TileType.Village;

            // Mines
            cumulative += MinePercentage;
            if (roll < cumulative) return TileType.Mine;

            // Shafts
            cumulative += ShaftPercentage;
            if (roll < cumulative) return TileType.Shaft;

            // Default fallback
            return TileType.Simple;
        }
    }
}

