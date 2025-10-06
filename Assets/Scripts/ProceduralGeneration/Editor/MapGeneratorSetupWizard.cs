using UnityEditor;
using UnityEngine;

namespace ProceduralGeneration.Editor
{
    /// <summary>
    /// Setup wizard for quick map generator configuration
    /// Creates all necessary assets and configures the scene
    /// </summary>
    public class MapGeneratorSetupWizard : EditorWindow
    {
        private string _configName = "MyMapGenConfig";
        private int _mapRadius = 5;
        private PresetType _presetType = PresetType.Balanced;

        private enum PresetType
        {
            Balanced,
            ResourceRich,
            Challenging,
            Exploration,
            Combat
        }

        [MenuItem("Tools/Map Generator/Setup Wizard")]
        public static void ShowWindow()
        {
            var window = GetWindow<MapGeneratorSetupWizard>("Map Generator Setup");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Map Generator Setup Wizard", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "This wizard will create a new MapGenerationConfig asset with your chosen settings.",
                MessageType.Info);

            GUILayout.Space(10);

            // Configuration name
            EditorGUILayout.LabelField("Configuration Settings", EditorStyles.boldLabel);
            _configName = EditorGUILayout.TextField("Config Name:", _configName);
            _mapRadius = EditorGUILayout.IntSlider("Map Radius:", _mapRadius, 1, 10);
            _presetType = (PresetType)EditorGUILayout.EnumPopup("Preset:", _presetType);

            GUILayout.Space(20);

            // Create button
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Create Configuration", GUILayout.Height(40)))
            {
                CreateConfiguration();
            }
            GUI.backgroundColor = Color.white;

            GUILayout.Space(10);

            // Help
            EditorGUILayout.HelpBox(
                "After creating the config:\n" +
                "1. Add MapGeneratorController to your scene\n" +
                "2. Assign the created config\n" +
                "3. Assign MapSettings and MapView\n" +
                "4. Assign tile prefabs\n" +
                "5. Click 'Generate New Map'",
                MessageType.Info);
        }

        private void CreateConfiguration()
        {
            // Create folder if doesn't exist
            string folderPath = "Assets/Configs";
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder("Assets", "Configs");
            }

            // Create config asset
            var config = ScriptableObject.CreateInstance<MapGenerationConfig>();
            config.MapRadius = _mapRadius;

            // Apply preset
            ApplyPreset(config, _presetType);

            // Save asset
            string assetPath = $"{folderPath}/{_configName}.asset";
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            AssetDatabase.CreateAsset(config, assetPath);
            AssetDatabase.SaveAssets();

            // Select and ping
            EditorGUIUtility.PingObject(config);
            Selection.activeObject = config;

            Debug.Log($"[Setup Wizard] Created MapGenerationConfig at {assetPath}");
            EditorUtility.DisplayDialog(
                "Success!",
                $"Configuration created at:\n{assetPath}\n\n" +
                "Next steps:\n" +
                "1. Add MapGeneratorController to scene\n" +
                "2. Assign this config and other references\n" +
                "3. Generate your map!",
                "OK");
        }

        private void ApplyPreset(MapGenerationConfig config, PresetType preset)
        {
            switch (preset)
            {
                case PresetType.Balanced:
                    config.SimpleTerrainPercentage = 30f;
                    config.MountainPercentage = 10f;
                    config.RiverPercentage = 5f;
                    config.FieldPercentage = 15f;
                    config.ForestPercentage = 15f;
                    config.VillagePercentage = 10f;
                    config.MinePercentage = 8f;
                    config.ShaftPercentage = 7f;
                    config.EnemyCastleCount = 2;
                    config.ChestCount = 3;
                    config.TavernCount = 2;
                    config.CampCount = 2;
                    config.ResourceCentralization = 0.3f;
                    config.DangerProgression = 0.5f;
                    break;

                case PresetType.ResourceRich:
                    config.SimpleTerrainPercentage = 20f;
                    config.MountainPercentage = 5f;
                    config.RiverPercentage = 5f;
                    config.FieldPercentage = 20f;
                    config.ForestPercentage = 20f;
                    config.VillagePercentage = 10f;
                    config.MinePercentage = 10f;
                    config.ShaftPercentage = 10f;
                    config.EnemyCastleCount = 1;
                    config.ChestCount = 5;
                    config.TavernCount = 3;
                    config.CampCount = 1;
                    config.ResourceCentralization = 0.2f;
                    config.DangerProgression = 0.3f;
                    break;

                case PresetType.Challenging:
                    config.SimpleTerrainPercentage = 25f;
                    config.MountainPercentage = 20f;
                    config.RiverPercentage = 10f;
                    config.FieldPercentage = 12f;
                    config.ForestPercentage = 12f;
                    config.VillagePercentage = 8f;
                    config.MinePercentage = 7f;
                    config.ShaftPercentage = 6f;
                    config.EnemyCastleCount = 3;
                    config.ChestCount = 2;
                    config.TavernCount = 1;
                    config.CampCount = 3;
                    config.ResourceCentralization = 0.5f;
                    config.DangerProgression = 0.8f;
                    break;

                case PresetType.Exploration:
                    config.SimpleTerrainPercentage = 35f;
                    config.MountainPercentage = 8f;
                    config.RiverPercentage = 7f;
                    config.FieldPercentage = 12f;
                    config.ForestPercentage = 13f;
                    config.VillagePercentage = 8f;
                    config.MinePercentage = 9f;
                    config.ShaftPercentage = 8f;
                    config.EnemyCastleCount = 1;
                    config.ChestCount = 6;
                    config.TavernCount = 4;
                    config.CampCount = 3;
                    config.ResourceCentralization = 0.2f;
                    config.DangerProgression = 0.4f;
                    break;

                case PresetType.Combat:
                    config.SimpleTerrainPercentage = 25f;
                    config.MountainPercentage = 15f;
                    config.RiverPercentage = 5f;
                    config.FieldPercentage = 10f;
                    config.ForestPercentage = 10f;
                    config.VillagePercentage = 15f;
                    config.MinePercentage = 10f;
                    config.ShaftPercentage = 10f;
                    config.EnemyCastleCount = 4;
                    config.ChestCount = 2;
                    config.TavernCount = 1;
                    config.CampCount = 4;
                    config.ResourceCentralization = 0.4f;
                    config.DangerProgression = 0.9f;
                    break;
            }

            // Common settings
            config.EnsurePathToResources = true;
            config.ClusterMountains = true;
            config.DistributeResourcesEvenly = true;
            config.MinCastleDistance = 4;
            config.Seed = 0;
        }
    }
}

