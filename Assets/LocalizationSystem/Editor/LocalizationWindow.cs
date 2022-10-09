using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace Simple.Localization
{

    [EditorWindowTitle(title = "Localization", useTypeNameAsIconName = false)]
    public class LocalizationWindow : EditorWindow
    {
        private LocalizationMasterDatabase database;
        private List<LocalizationLanguageDatabase> activeLanguages;

        private int activeTab = 0;
        private Vector2 scrollValue;

        [MenuItem("Tools/Localization")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(LocalizationWindow));
        }

        private void OnEnable() => FindDatabase();

        void OnGUI()
        {
            if (database == null)
            {
                EditorGUILayout.HelpBox("No database found, please provide one.", MessageType.Error);
                if (GUILayout.Button("Update"))
                    FindDatabase();
                return;
            }


            SimpleLocalizationSettings settings = SimpleLocalizationSettings.Resolve();
            if (settings.DefaultLanguage == null)
                EditorGUILayout.HelpBox("You have no default language selected, please select one in the project Settings.", MessageType.Warning);

            EditorGUILayout.HelpBox("currently selected database: " + database.name + (settings.DefaultLanguage != null ? " & default language: " + settings.DefaultLanguage.Name : ""), MessageType.Info);


            activeTab = GUILayout.Toolbar(activeTab, new string[] { "Translations", "Dialogues" });

            if (activeTab == 0)
                TranslationTabGUI();
            else
                DialoguesTabGUI(settings);
        }

        private void TranslationTabGUI()
        {
            int keyCollumWidth = ((int)position.width);
            int languagecollumnWidth = 0;

            EditorGUILayout.BeginHorizontal();

            bool noLanguageSelected = activeLanguages.Count == 0;

            if (!noLanguageSelected)
            {
                int languageCollumCount = activeLanguages.Count;
                keyCollumWidth = Mathf.Max(100, (int)position.width / languageCollumCount / 2);
                languagecollumnWidth = Mathf.FloorToInt(((float)position.width - keyCollumWidth) / languageCollumCount);

                GUILayout.Label("Key", GUILayout.Width(keyCollumWidth));

                for (int i = 0; i < activeLanguages.Count; i++)
                {
                    int width = languagecollumnWidth - 40;

                    GUILayout.BeginHorizontal(GUILayout.Width(i == activeLanguages.Count - 1 ? width : languagecollumnWidth));
                    GUILayout.Label(activeLanguages[i].name);

                    if (GUILayout.Button("x", GUILayout.Width(20)))
                        activeLanguages.RemoveAt(i);

                    GUILayout.EndHorizontal();
                }

            }

            if (noLanguageSelected) EditorGUILayout.HelpBox("please click + to display additional languages." + database.name, MessageType.Info);

            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                for (int i = 0; i < database.Languages.Count; i++)
                {
                    var language = database.Languages[i];

                    if (!activeLanguages.Contains(language))
                    {
                        activeLanguages.Add(language);
                        break;
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.BeginVertical();

            foreach (LocalizationKey key in database.Keys)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(key.Key, GUILayout.Width(keyCollumWidth));

                foreach (LocalizationLanguageDatabase language in activeLanguages)
                {
                    bool exists = language.Entries != null && language.HasEntry(key);

                    if (exists)
                    {
                        string textBefore = language.GetEntry(key).Value;
                        string textAfter = GUILayout.TextArea(textBefore, GUILayout.Width(languagecollumnWidth));

                        if (textAfter != textBefore)
                            language.SetEntry(key, textAfter);
                    }
                    else
                    {
                        if (GUILayout.Button("<Add Entry>", GUILayout.Width(languagecollumnWidth)))
                            language.AddEntry(key, "");
                    }
                }


                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        private void DialoguesTabGUI(SimpleLocalizationSettings settings)
        {
            scrollValue = GUILayout.BeginScrollView(scrollValue);
            foreach (var item in AssetDatabase.FindAssets("t:ScriptableObject", new string[] { "Assets" }))
            {
                string path = AssetDatabase.GUIDToAssetPath(item);
                ILocalizeableTextContainer textContainer = AssetDatabase.LoadAssetAtPath(path, typeof(ILocalizeableTextContainer)) as ILocalizeableTextContainer;

                if (textContainer != null)
                {
                    ILocalizeableText[] texts = textContainer.GetAllChilds();

                    float total = texts.Length;
                    ILocalizeableText[] notLocalized = texts.Where(t => !t.IsLocalized).ToArray();
                    float localized = total - notLocalized.Length;

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField(textContainer.ContainerName + " > Total Texts: " + total + " / " + localized + " localized >> " + Mathf.RoundToInt(localized / total * 100) + "%");



                    if (localized < total && GUILayout.Button("Create " + (total - localized) + " additional Keys"))
                    {
                        if (settings.DefaultLanguage != null)
                        {
                            foreach (ILocalizeableText text in notLocalized)
                            {
                                string key = text.GetGUID();
                                if (!settings.DefaultLanguage.HasEntry(key))
                                {
                                    settings.DefaultLanguage.AddEntry(key, text.GetValue());
                                    text.SetIsLocalized(true, key);
                                }
                            }
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();

        }

        private void FindDatabase()
        {
            database = GetFirstInstanceOf<LocalizationMasterDatabase>();
            activeLanguages = new List<LocalizationLanguageDatabase>(database.Languages);
        }

        private static T GetFirstInstanceOf<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);

            if (guids.Length == 0)
                return null;

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
    }
}