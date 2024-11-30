﻿using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Watermelon
{
    public class SkinPickerWindow : EditorWindow
    {
        private HandlerData handlerData;
        private SerializedProperty property;

        private int tabIndex;

        private Vector2 scrollView;

        private GUIStyle boxStyle;

        private string selectedID;

        private static SkinPickerWindow window;

        public static void PickSkin(SerializedProperty property, System.Type filterType)
        {
            if(window != null)
            {
                window.Close();
            }

            window = EditorWindow.GetWindow<SkinPickerWindow>(true);
            window.titleContent = new GUIContent("Skins Picker");
            window.property = property;
            window.selectedID = property.stringValue;
            window.handlerData = new HandlerData(filterType);
            window.tabIndex = window.handlerData.GetTabIndex(window.selectedID);
            window.scrollView = window.CalculateScrollView();
            window.Show();
        }

        private Vector2 CalculateScrollView()
        {
            if(string.IsNullOrEmpty(selectedID))
                return Vector2.zero;

            //int elementIndex = System.Array.FindIndex(database.Skins, x => x.ID == selectedID);

            int elementIndex = 0;

            return new Vector2(0, elementIndex * 65);
        }

        private void OnEnable()
        {
            boxStyle = new GUIStyle(EditorCustomStyles.Skin.box);
            boxStyle.overflow = new RectOffset(0, 0, 0, 0);
        }

        private void OnGUI()
        {
            if(property == null)
            {
                Close();

                return;
            }

            if(handlerData.Tabs.Length > 1)
            {
                tabIndex = GUI.Toolbar(new Rect(0, 0, Screen.width, 30), tabIndex, handlerData.TabNames, EditorCustomStyles.tab);

                GUILayout.Space(30);
            }

            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(-14); 

            scrollView = EditorGUILayout.BeginScrollView(scrollView);

            //Draw existing items
            if (handlerData.Tabs.IsInRange(tabIndex))
            {
                HandlerData.Tab selectedTab = handlerData.Tabs[tabIndex];
                AbstractSkinsDatabase provider = selectedTab.SkinsProvider;
                int skinsCount = provider.SkinsCount;

                if (skinsCount > 0)
                {
                    for (int i = 0; i < skinsCount; i++)
                    {
                        ISkinData skin = provider.GetSkinData(i);
                        Color defaultColor = GUI.backgroundColor;

                        if (selectedID == skin.ID)
                        {
                            GUI.backgroundColor = Color.yellow;
                        }

                        Rect elementRect = EditorGUILayout.BeginVertical(boxStyle, GUILayout.MinHeight(58), GUILayout.ExpandHeight(false));

                        GUILayout.Space(58);

                        if (GUI.Button(elementRect, GUIContent.none, GUIStyle.none))
                        {
                            if (selectedID == skin.ID)
                            {
                                Close();

                                return;
                            }

                            selectedID = skin.ID;

                            SelectSkin(selectedID);
                        }

                        using (new EditorGUI.DisabledScope(disabled: true))
                        {
                            elementRect.x += 4;
                            elementRect.width -= 8;
                            elementRect.y += 4;
                            elementRect.height -= 8;

                            DrawElement(elementRect, skin, selectedTab.GetPreview(skin), i);
                        }

                        EditorGUILayout.EndVertical();

                        GUI.backgroundColor = defaultColor;
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Skins list is empty!", MessageType.Info);
                }
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void SelectSkin(string ID)
        {
            property.serializedObject.Update();
            property.stringValue = ID;
            property.serializedObject.ApplyModifiedProperties();
        }

        private void DrawElement(Rect rect, ISkinData skinData, Object previewObject, int index)
        {
            float defaultYPosition = rect.y;

            rect.width -= 60;

            Rect propertyPosition = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(propertyPosition, string.Format("Skin #{0}", (index + 1)));

            propertyPosition.y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.LabelField(propertyPosition, skinData.ID);

            propertyPosition.y += EditorGUIUtility.singleLineHeight + 2;

            Rect boxRect = new Rect(rect.x + propertyPosition.width + 2, defaultYPosition, 58, 58);
            GUI.Box(boxRect, GUIContent.none);

            Texture2D previewTexture = AssetPreview.GetAssetPreview(previewObject);
            if (previewTexture != null)
            {
                GUI.DrawTexture(new Rect(boxRect.x + 2, boxRect.y + 2, 55, 55), previewTexture);
            }
            else
            {
                GUI.DrawTexture(new Rect(boxRect.x + 2, boxRect.y + 2, 55, 55), EditorCustomStyles.GetMissingIcon());
            }
        }

        private class HandlerData
        {
            public bool IsInitialised { get; private set; }

            public string[] TabNames { get; private set; }
            public Tab[] Tabs { get; private set; }

            public HandlerData(System.Type filteredType)
            {
                if (filteredType != null)
                {
                    Tabs = new Tab[1];
                    Tabs[0] = new Tab(EditorSkinsProvider.GetSkinsProvider(filteredType));
                }
                else
                {
                    List<AbstractSkinsDatabase> skinProviders = EditorSkinsProvider.SkinsDatabases;
                    Tabs = new Tab[skinProviders.Count];
                    TabNames = new string[skinProviders.Count];

                    for (int i = 0; i < skinProviders.Count; i++)
                    {
                        Tabs[i] = new Tab(skinProviders[i]);
                        TabNames[i] = Tabs[i].Name;
                    }
                }

                IsInitialised = true;
            }

            public int GetTabIndex(string selectedID)
            {
                for(int i = 0; i < Tabs.Length; i++)
                {
                    if (Tabs[i].SkinsProvider.GetSkinData(selectedID) != null)
                        return i;
                }

                return 0;
            }

            public class Tab
            {
                public string Name { get; private set; }
                public AbstractSkinsDatabase SkinsProvider { get; private set; }

                private FieldInfo previewFieldInfo;

                public Tab(AbstractSkinsDatabase skinsProvider)
                {
                    SkinsProvider = skinsProvider;

                    Name = GetProviderName();

                    previewFieldInfo = SkinsProvider.SkinType.GetFields(ReflectionUtils.FLAGS_INSTANCE).First(x => x.GetCustomAttribute<SkinPreviewAttribute>() != null);
                }

                public Object GetPreview(object value)
                {
                    object preview = previewFieldInfo.GetValue(value);
                    if (preview != null)
                    {
                        return preview as Object;
                    }

                    return null;
                }

                private string GetProviderName()
                {
                    if (SkinsProvider == null)
                    {
                        return "NULL";
                    }

                    return SkinsProvider.GetType().Name
                        .Replace("Skins", "")
                        .Replace("Skin", "")
                        .Replace("Provider", "")
                        .Replace("Database", "")
                        .Replace("Data", "");
                }
            }
        }
    }
}
