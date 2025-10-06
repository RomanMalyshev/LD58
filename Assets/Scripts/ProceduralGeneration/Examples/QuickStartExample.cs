using UnityEngine;

namespace ProceduralGeneration.Examples
{
    /// <summary>
    /// Example script showing how to use the map generator from code
    /// </summary>
    public class QuickStartExample : MonoBehaviour
    {
        [Header("Required References")]
        [SerializeField] private MapGeneratorController _mapGenerator;

        [Header("Runtime Generation Settings")]
        [SerializeField] private bool _generateOnAwake = false;
        [SerializeField] private KeyCode _regenerateKey = KeyCode.R;
        [SerializeField] private KeyCode _showStatsKey = KeyCode.T;

        private void Awake()
        {
            if (_generateOnAwake && _mapGenerator != null)
            {
                _mapGenerator.GenerateNewMap();
            }
        }

        private void Update()
        {
            if (_mapGenerator == null) return;

            // Regenerate map on key press
            if (Input.GetKeyDown(_regenerateKey))
            {
                Debug.Log("[Example] Regenerating map...");
                _mapGenerator.GenerateWithRandomSeed();
            }

            // Show statistics on key press
            if (Input.GetKeyDown(_showStatsKey))
            {
                Debug.Log("[Example] Showing map statistics...");
                _mapGenerator.ShowMapStatistics();
            }
        }

        /// <summary>
        /// Example: Generate map with specific seed
        /// </summary>
        public void GenerateMapWithSeed(int seed)
        {
            // This would require exposing the config reference
            // For now, you'd set it in the inspector before calling GenerateNewMap
            Debug.Log($"[Example] To use seed {seed}, set it in MapGenerationConfig, then:");
            _mapGenerator.GenerateNewMap();
        }

        /// <summary>
        /// Example: Programmatic generation
        /// </summary>
        public void GenerateProgrammatically()
        {
            // This example shows the manual process
            // Normally you'd just use MapGeneratorController.GenerateNewMap()
            
            Debug.Log("[Example] For programmatic generation, see ProceduralMapGenerator class");
            Debug.Log("[Example] Or simply call mapGeneratorController.GenerateNewMap()");
        }
    }
}

