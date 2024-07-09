using System.Linq;
using UnityEditor;

[CustomEditor(typeof(TrashNpcSO)), CanEditMultipleObjects]
public class TrashNpcSOInspector : Editor
{

    readonly string[] skip_fields = new[] { "recyclableConfig", "contaminantConfig" };

    public override void OnInspectorGUI()
    {
        TrashNpcSO trashNpcSO = (TrashNpcSO)target;
        CustomEditorTools.Utils.DrawInspectorExcept(serializedObject, skip_fields);



        if (trashNpcSO.trashNPCType == TrashNPCType.Recyclable)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("recyclableConfig"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("contaminantConfig"));

        serializedObject.ApplyModifiedProperties();
    }
}