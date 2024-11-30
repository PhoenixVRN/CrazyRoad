using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Watermelon;

namespace Watermelon.VehicleFactory
{
    [CustomEditor(typeof(SpawnerTool))]
    public class SpawnerEditor : Editor
    {
        private const string SPAWN_DATA_PROPERTY_NAME = "spawnData";
        private const string CONTAINER_PROPERTY_NAME = "container";
        private const string CHANCE_PROPERTY_NAME = "chance";
        private const string PREFAB_PROPERTY_NAME = "prefab";
        private SerializedProperty spawnDataProperty;
        private SerializedProperty containerProperty;
        private SerializedProperty prefabProperty;
        private List<string> warnings;
        private List<string> errors;
        private SpawnerTool spawnerTool;
        private GameObject tempGameObject;

        float total;

        private void OnEnable()
        {
            spawnDataProperty = serializedObject.FindProperty(SPAWN_DATA_PROPERTY_NAME);
            containerProperty = serializedObject.FindProperty(CONTAINER_PROPERTY_NAME);
            spawnerTool = serializedObject.targetObject as SpawnerTool;
            warnings = new List<string>();
            errors = new List<string>();
            ValidateSpawnData();
        }

        private void ValidateSpawnData()
        {
            total = 0;
            warnings.Clear();
            errors.Clear();

            for (int i = 0; i < spawnDataProperty.arraySize; i++)
            {
                total += spawnDataProperty.GetArrayElementAtIndex(i).FindPropertyRelative(CHANCE_PROPERTY_NAME).floatValue;
                prefabProperty = spawnDataProperty.GetArrayElementAtIndex(i).FindPropertyRelative(PREFAB_PROPERTY_NAME);

                if (prefabProperty.objectReferenceValue == null)
                {
                    errors.Add("Spawn data index: " + i + ". Error null prefab. Please set prefab.");
                }
                else
                {
                    tempGameObject = prefabProperty.objectReferenceValue as GameObject;


                    if (tempGameObject.GetComponentInChildren<SavableItem>() == null)
                    {
                        errors.Add("Spawn data index: " + i + ". Error SavableItem component missing from prefab. Please add SavableItem to prefab otherwise this element won`t be saved.");
                    }

                    if (tempGameObject.GetComponentInChildren<Rigidbody>() == null && tempGameObject.GetComponentInChildren<Rigidbody2D>() == null)
                    {
                        errors.Add("Spawn data index: " + i + ". Error Rigidbody or Rigidbody2D component missing from prefab. Please add Rigidbody or Rigidbody2D to prefab otherwise this element won`t be affected by physics.");
                    }

                    if (tempGameObject.GetComponentInChildren<Collider>() == null && tempGameObject.GetComponentInChildren<Collider2D>() == null)
                    {
                        errors.Add("Spawn data index: " + i + ". Error collider component missing from prefab. Please add appropriate colllider to prefab otherwise this element won`t work correctly with rigidbody.");
                    }
                }
            }

            if (spawnDataProperty.arraySize == 0)
            {
                warnings.Add("Spawn data arraySize = 0. Spawn button unavailable.");
            }
            else if (total > 1f)
            {
                warnings.Add("Total spawn chance not equal 1f. Don`t forget to click \"Adjust chances\" when you finish assigning spawnData.");
            }
            else if (spawnDataProperty.arraySize == 1 && total != 1f)
            {
                warnings.Add("Total spawn chance not equal 1f. Don`t forget to click \"Adjust chances\" when you finish assigning spawnData.");
            }
            else if (spawnDataProperty.arraySize > 1 && total < 0.98f)
            {
                warnings.Add("Total spawn chance not equal 1f. Don`t forget to click \"Adjust chances\" when you finish assigning spawnData.");
            }
        }

        public override void OnInspectorGUI()
        {

            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(containerProperty);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(spawnDataProperty);

            if (EditorGUI.EndChangeCheck())
            {
                ValidateSpawnData();
            }

            foreach (string message in errors)
            {
                EditorGUILayout.HelpBox(message, MessageType.Error);
            }

            foreach (string message in warnings)
            {
                EditorGUILayout.HelpBox(message, MessageType.Warning);
            }

            EditorGUI.BeginDisabledGroup(spawnDataProperty.arraySize == 0);

            if (GUILayout.Button("Spawn"))
            {
                spawnerTool.Spawn();
            }

            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Adjust chances"))
            {
                spawnerTool.AdjustChances();
                GUI.FocusControl(null);
                serializedObject.Update();
                ValidateSpawnData();
            }

            if (Physics.simulationMode == SimulationMode.FixedUpdate)
            {
                if (GUILayout.Button("Start physics"))
                {
                    spawnerTool.StartPhysics();
                }
            }
            else
            {
                if (GUILayout.Button("Stop physics"))
                {
                    spawnerTool.StopPhysics();
                }

                EditorGUILayout.HelpBox("Don`t forget to stop physics because it affects unity global waribale \"Physics.autoSimulation\" .", MessageType.Info);
            }

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}