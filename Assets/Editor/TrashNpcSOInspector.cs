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


    public TrashNpcFactorySO FindFactory()
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(TrashNpcFactorySO).Name}");
        if (guids.Length < 1)
        {
            return null;
        }
        if (guids.Length > 1)
        {
            Debug.LogWarning("Multiple TrashNpcFactory assets found. Will pick first one!. This will cause unintended side effects!");
        }
        var asset_path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<TrashNpcFactorySO>(asset_path);
    }


    public override void OnInspectorGUI()
    {
        TrashNpcSO trashNpcData = (TrashNpcSO)target;



        CustomEditorTools.Utils.DrawInspectorExcept(serializedObject, skip_fields);



        if (trashNpcData.trashNPCType != TrashNPCType.OnlyContaminant)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("recyclableConfig"));
        }
        if (trashNpcData.trashNPCType != TrashNPCType.OnlyRecyclable){
            EditorGUILayout.PropertyField(serializedObject.FindProperty("contaminantConfig"));
        }


        // -------------------- CREATOR TOOLING ------------------



        List<string> errors_string = new List<string>();
        bool missingRecyclablePrefab = false;
        bool missingContaminantPrefab = false;

        TrashNpcFactorySO factory = FindFactory();
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

        if (trashNpcData.recyclableConfig.recyclablePrefab == null)
        {
            if (trashNpcData.trashNPCType == TrashNPCType.Recyclable)
            {
                missingRecyclablePrefab = true;
                errors_string.Add("Missing recyclable prefab");
            }
        }

        if (trashNpcData.contaminantConfig.contaminantPrefab == null && trashNpcData.trashNPCType != TrashNPCType.OnlyRecyclable)
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
            var filepath = EditorUtility.SaveFilePanel("Create Recyclable NPC", "", $"{trashNpcData.name}_Recyclable.prefab", "prefab");
            trashNpcData.recyclableConfig.recyclablePrefab = factory.CreateRecyclable(filepath, trashNpcData);
        }
        if (missingContaminantPrefab && GUILayout.Button("Create Contaminant prefab"))
        {
            var filepath = EditorUtility.SaveFilePanel("Create Contaminant NPC", "", $"{trashNpcData.name}_Contaminant.prefab", "prefab");
            trashNpcData.contaminantConfig.contaminantPrefab = factory.CreateContaminant(filepath, trashNpcData);
        }


        serializedObject.ApplyModifiedProperties();
    }
}