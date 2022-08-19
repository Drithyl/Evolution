
using UnityEngine;
using UnityEditor;

/// Code by: https://www.patrykgalach.com/2020/01/20/readonly-attribute-in-unity-editor/
/// Tweaks by me.

/// <summary>
/// Read Only attribute to mark properties as
/// non-editable in Unity's Inspector.
/// </summary>
public class ReadOnlyAttribute : PropertyAttribute { }


/// <summary>
/// This class contains a custom drawer for the ReadOnly attribute.
/// </summary>
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    /// <summary>
    /// Unity method for drawing GUI in Editor
    /// </summary>
    /// <param name="position">Position.</param>
    /// <param name="property">Property.</param>
    /// <param name="label">Label.</param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Saving previous GUI enabled value
        bool previousGUIState = GUI.enabled;

        // Disabling edit for property
        GUI.enabled = false;

        // Drawing Property in Inspector
        EditorGUI.PropertyField(position, property, label);

        // Restablishing old GUI enabled value
        GUI.enabled = previousGUIState;
    }
}
