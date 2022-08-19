
using UnityEngine;
using UnityEditor;

namespace CreatureSpawning
{
    [CustomEditor(typeof(CreatureSpawner))]
    public class CreatureSpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Spawn Creature"))
            {
                CreatureSpawner.Instance.Spawn(
                    CreatureSpawner.Instance.speciesToSpawn, 
                    CreatureSpawner.Instance.x,
                    CreatureSpawner.Instance.y
                );
            }
        }
    }
}
