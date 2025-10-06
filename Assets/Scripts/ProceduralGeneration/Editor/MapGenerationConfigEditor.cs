using UnityEditor;
using UnityEngine;

namespace ProceduralGeneration.Editor
{
    /// <summary>
    /// Custom editor for MapGenerationConfig
    /// Shows percentage validation and preset buttons
    /// </summary>
    [CustomEditor(typeof(MapGenerationConfig))]
    public class MapGenerationConfigEditor : UnityEditor.Editor
    {
        private MapGenerationConfig _config;

        private void OnEnable()
        {
            _config = (MapGenerationConfig)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(10);

            // Calculate total percentage
            float totalPercentage = _config.SimpleTerrainPercentage +
                                   _config.MountainPercentage +
                                   _config.RiverPercentage +
                                   _config.FieldPercentage +
                                   _config.ForestPercentage +
                                   _config.VillagePercentage +
                                   _config.MinePercentage +
                                   _config.ShaftPercentage;

            // Display percentage validation
            EditorGUILayout.LabelField("Percentage Validation", EditorStyles.boldLabel);
            
            Color oldColor = GUI.color;
            if (Mathf.Abs(totalPercentage - 100f) < 0.1f)
            {
                GUI.color = Color.green;
                EditorGUILayout.HelpBox($"Total: {totalPercentage:F1}% ✓ Valid", MessageType.Info);
            }
            else
            {
                GUI.color = Color.red;
                EditorGUILayout.HelpBox($"Total: {totalPercentage:F1}% ✗ Must equal 100%", MessageType.Warning);
            }
            GUI.color = oldColor;

            EditorGUILayout.Space(10);

            // Preset buttons
            EditorGUILayout.LabelField("Presets", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Balanced"))
            {
                ApplyBalancedPreset();
            }

            if (GUILayout.Button("Resource Rich"))
            {
                ApplyResourceRichPreset();
            }

            if (GUILayout.Button("Challenging"))
            {
                ApplyChallengingPreset();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Exploration"))
            {
                ApplyExplorationPreset();
            }

            if (GUILayout.Button("Combat Focus"))
            {
                ApplyCombatPreset();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ApplyBalancedPreset()
        {
            Undo.RecordObject(_config, "Apply Balanced Preset");
            
            _config.SimpleTerrainPercentage = 30f;
            _config.MountainPercentage = 10f;
            _config.RiverPercentage = 5f;
            _config.FieldPercentage = 15f;
            _config.ForestPercentage = 15f;
            _config.VillagePercentage = 10f;
            _config.MinePercentage = 8f;
            _config.ShaftPercentage = 7f;
            
            _config.EnemyCastleCount = 2;
            _config.ChestCount = 3;
            _config.TavernCount = 2;
            _config.CampCount = 2;
            
            _config.ResourceCentralization = 0.3f;
            _config.DangerProgression = 0.5f;

            EditorUtility.SetDirty(_config);
            Debug.Log("Applied Balanced preset");
        }

        private void ApplyResourceRichPreset()
        {
            Undo.RecordObject(_config, "Apply Resource Rich Preset");
            
            _config.SimpleTerrainPercentage = 20f;
            _config.MountainPercentage = 5f;
            _config.RiverPercentage = 5f;
            _config.FieldPercentage = 20f;
            _config.ForestPercentage = 20f;
            _config.VillagePercentage = 10f;
            _config.MinePercentage = 10f;
            _config.ShaftPercentage = 10f;
            
            _config.EnemyCastleCount = 1;
            _config.ChestCount = 5;
            _config.TavernCount = 3;
            _config.CampCount = 1;
            
            _config.ResourceCentralization = 0.2f;
            _config.DangerProgression = 0.3f;

            EditorUtility.SetDirty(_config);
            Debug.Log("Applied Resource Rich preset");
        }

        private void ApplyChallengingPreset()
        {
            Undo.RecordObject(_config, "Apply Challenging Preset");
            
            _config.SimpleTerrainPercentage = 25f;
            _config.MountainPercentage = 20f;
            _config.RiverPercentage = 10f;
            _config.FieldPercentage = 12f;
            _config.ForestPercentage = 12f;
            _config.VillagePercentage = 8f;
            _config.MinePercentage = 7f;
            _config.ShaftPercentage = 6f;
            
            _config.EnemyCastleCount = 3;
            _config.ChestCount = 2;
            _config.TavernCount = 1;
            _config.CampCount = 3;
            
            _config.ResourceCentralization = 0.5f;
            _config.DangerProgression = 0.8f;

            EditorUtility.SetDirty(_config);
            Debug.Log("Applied Challenging preset");
        }

        private void ApplyExplorationPreset()
        {
            Undo.RecordObject(_config, "Apply Exploration Preset");
            
            _config.SimpleTerrainPercentage = 35f;
            _config.MountainPercentage = 8f;
            _config.RiverPercentage = 7f;
            _config.FieldPercentage = 12f;
            _config.ForestPercentage = 13f;
            _config.VillagePercentage = 8f;
            _config.MinePercentage = 9f;
            _config.ShaftPercentage = 8f;
            
            _config.EnemyCastleCount = 1;
            _config.ChestCount = 6;
            _config.TavernCount = 4;
            _config.CampCount = 3;
            
            _config.ResourceCentralization = 0.2f;
            _config.DangerProgression = 0.4f;

            EditorUtility.SetDirty(_config);
            Debug.Log("Applied Exploration preset");
        }

        private void ApplyCombatPreset()
        {
            Undo.RecordObject(_config, "Apply Combat Preset");
            
            _config.SimpleTerrainPercentage = 25f;
            _config.MountainPercentage = 15f;
            _config.RiverPercentage = 5f;
            _config.FieldPercentage = 10f;
            _config.ForestPercentage = 10f;
            _config.VillagePercentage = 15f;
            _config.MinePercentage = 10f;
            _config.ShaftPercentage = 10f;
            
            _config.EnemyCastleCount = 4;
            _config.ChestCount = 2;
            _config.TavernCount = 1;
            _config.CampCount = 4;
            
            _config.ResourceCentralization = 0.4f;
            _config.DangerProgression = 0.9f;

            EditorUtility.SetDirty(_config);
            Debug.Log("Applied Combat Focus preset");
        }
    }
}

