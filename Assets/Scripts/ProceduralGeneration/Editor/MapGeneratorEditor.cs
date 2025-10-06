using UnityEditor;
using UnityEngine;

namespace ProceduralGeneration.Editor
{
    /// <summary>
    /// Custom editor for MapGeneratorController
    /// Provides convenient buttons and visualization in inspector
    /// </summary>
    [CustomEditor(typeof(MapGeneratorController))]
    public class MapGeneratorEditor : UnityEditor.Editor
    {
        private MapGeneratorController _controller;

        private void OnEnable()
        {
            _controller = (MapGeneratorController)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Map Generation Tools", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // Generate button
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Generate New Map", GUILayout.Height(40)))
            {
                _controller.GenerateNewMap();
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(5);

            // Other buttons
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Random Seed", GUILayout.Height(30)))
            {
                _controller.GenerateWithRandomSeed();
            }

            if (GUILayout.Button("Show Stats", GUILayout.Height(30)))
            {
                _controller.ShowMapStatistics();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Clear button
            GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
            if (GUILayout.Button("Clear Map", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog(
                    "Clear Map",
                    "Are you sure you want to clear all generated tiles?",
                    "Yes",
                    "No"))
                {
                    _controller.ClearMap();
                }
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(10);

            // Help box
            EditorGUILayout.HelpBox(
                "1. Set up MapSettings, MapView, and GenerationConfig\n" +
                "2. Assign all tile prefabs\n" +
                "3. Click 'Generate New Map' to create a procedural level\n" +
                "4. Adjust config and regenerate for different results",
                MessageType.Info);
        }
    }
}

