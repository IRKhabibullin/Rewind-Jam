using UnityEditor;
using UnityEngine;
using static CraneController;

[CustomPropertyDrawer(typeof(CraneInstruction))]
public class CraneInstructionDrawer : PropertyDrawer
{
    int propertyRect = 112;
    int propertyOffset = 10;
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Calculate rects
        var nameRect = new Rect(position.x, position.y, propertyRect, position.height);
        var targetRect = new Rect(position.x + propertyRect + propertyOffset, position.y, position.width - propertyRect - propertyOffset, position.height);

        var actionName = property.FindPropertyRelative("name");

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);
        if ((CraneAction)actionName.intValue == CraneAction.PickUp || (CraneAction)actionName.intValue == CraneAction.Deliver)
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
            return (arrayProp.arraySize + 2.5f) * elementHeight;
        }
        return elementHeight;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty arrayProp = property.FindPropertyRelative("data");
        arrayProp.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, elementHeight), arrayProp.isExpanded, label, true);
        EditorGUI.BeginProperty(position, label, property);

        if (arrayProp.isExpanded)
        {
            // Instructions size
            EditorGUI.PropertyField(new Rect(position.x, position.y + elementHeight, position.width, elementHeight), arrayProp.FindPropertyRelative("Array.size"));

            Handles.color = Color.black;
            Handles.DrawLine(new Vector2(position.x, position.y + elementHeight * 2.25f), new Vector2(position.width + 15, position.y + elementHeight * 2.25f));
            // List of instructions
            Rect nextItemPosition = new Rect(position.x, position.y + elementHeight * 2.5f, position.width, elementHeight);
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