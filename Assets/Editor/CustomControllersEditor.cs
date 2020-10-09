using UnityEditor;
using UnityEngine;
using static CraneController;

[CustomPropertyDrawer(typeof(CraneInstruction))]
public class CraneInstructionDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Calculate rects
        var nameRect = new Rect(position.x, position.y, 100, position.height);
        var targetRect = new Rect(position.x + 110, position.y, position.width - 110, position.height);

        var actionName = property.FindPropertyRelative("name");

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);
        if ((CraneAction)actionName.intValue == CraneAction.Grab)
        {
            EditorGUI.PropertyField(targetRect, property.FindPropertyRelative("position"), GUIContent.none);
        } else
        {
            EditorGUI.PropertyField(targetRect, property.FindPropertyRelative("target"), GUIContent.none);
        }

        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(CraneInstructions))]
public class CraneInstructionsDrawer : PropertyDrawer
{
    int elementHeight = 21;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty arrayProp = property.FindPropertyRelative("data");
        if (arrayProp.isExpanded)
        {
            return (arrayProp.arraySize + 1) * elementHeight;
        }
        return elementHeight;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty arrayProp = property.FindPropertyRelative("data");
        arrayProp.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, elementHeight), arrayProp.isExpanded, GUIContent.none);
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        if (arrayProp.isExpanded)
        {
            // Instructions size
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, elementHeight), arrayProp.FindPropertyRelative("Array.size"));

            // List of instructions
            Rect nextItemPosition = new Rect(position.x, position.y + elementHeight, position.width, elementHeight);
            for (int i = 0; i < arrayProp.arraySize; i++)
            {
                SerializedProperty value = arrayProp.GetArrayElementAtIndex(i);
                EditorGUI.PropertyField(nextItemPosition, value, GUIContent.none);
                nextItemPosition = new Rect(position.x, nextItemPosition.y + elementHeight, position.width, elementHeight);
            }
        }

        EditorGUI.EndProperty();
        EditorGUI.EndFoldoutHeaderGroup();
    }
}