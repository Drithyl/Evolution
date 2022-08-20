
using UnityEditor;
using UnityEngine;

namespace TerrainGeneration
{
    [CustomEditor(typeof(TerrainGenerator))]
    public class TerrainGeneratorEditor : Editor
    {
        TerrainGenerator terrainGenerator;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Refresh"))
            {
                terrainGenerator.GenerateTerrain();
            }
        }

        void OnEnable()
        {
            terrainGenerator = (TerrainGenerator)target;
        }
    }
}
