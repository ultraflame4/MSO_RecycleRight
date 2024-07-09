using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TrashNpcSO)), CanEditMultipleObjects]
public class TrashNpcSOInspector : Editor
{

    readonly string[] skip_fields = new[] { "recyclableConfig", "contaminantConfig" };

    readonly Color danger = new Color(1f, .4f, .4f);


    public TrashNpcFactory FindFactory()
    {
        string[] guids = AssetDatabase.FindAssets("t:TrashNpcFactory");
        if (guids.Length < 1)
        {
            return null;
        }
        if (guids.Length > 1)
        {
            Debug.LogWarning("Multiple TrashNpcFactory assets found. Will pick first one!. This will cause unintended side effects!");
        }
        var asset_path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<TrashNpcFactory>(asset_path);
    }


    public override void OnInspectorGUI()
    {
        TrashNpcSO trashNpcSO = (TrashNpcSO)target;



        CustomEditorTools.Utils.DrawInspectorExcept(serializedObject, skip_fields);



        if (trashNpcSO.trashNPCType == TrashNPCType.Recyclable)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("recyclableConfig"));
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("contaminantConfig"));


        // -------------------- CREATOR TOOLING ------------------



        List<string> errors_string = new List<string>();
        bool missingRecyclablePrefab = false;
        bool missingContaminantPrefab = false;

        TrashNpcFactory factory = FindFactory();
        if (factory == null)
        {
            errors_string.Add("TrashNpcFactory asset could not be found. Please create one.");
            Debug.LogError("TrashNpcFactory asset could not be found. Please create one.");
        }
        else
        {
            if (factory.templateRecyclablePrefab == null)
            {
                errors_string.Add("TrashNpcFactory asset is missing templateRecyclablePrefab");
                Debug.LogWarning("TrashNpcFactory asset is missing templateRecyclablePrefab");
            }
            if (factory.templateContaminantPrefab == null)
            {
                errors_string.Add("TrashNpcFactory asset is missing templateContaminantPrefab");
                Debug.LogWarning("TrashNpcFactory asset is missing templateContaminantPrefab");
            }
        }

        if (trashNpcSO.recyclableConfig.recyclablePrefab == null)
        {
            if (trashNpcSO.trashNPCType == TrashNPCType.Recyclable)
            {
                missingRecyclablePrefab = true;
                errors_string.Add("Missing recyclable prefab");
            }
        }

        if (trashNpcSO.contaminantConfig.contaminantPrefab == null)
        {
            missingContaminantPrefab = true;
            errors_string.Add("Missing contaminant prefab");
        }




        GUILayout.Space(100);

        GUILayout.Label("TrashNPC Creator", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        GUILayout.Label("Errors", EditorStyles.boldLabel);
        GUI.contentColor = danger;
        GUILayout.Label(String.Join("\n", errors_string), EditorStyles.textArea);
        GUI.contentColor = Color.white;

        if (missingRecyclablePrefab && GUILayout.Button("Create Recyclable prefab"))
        {

        }
        if (missingContaminantPrefab && GUILayout.Button("Create Contaminant prefab"))
        {

        }


        serializedObject.ApplyModifiedProperties();
    }
}