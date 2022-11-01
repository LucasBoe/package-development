using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Simple.Localization
{
    // Create a new type of Settings Asset.
    public class SimpleLocalizationSettings : ScriptableObject
    {
        public const string LOCALIZATION_SETTINGS_PATH = "Assets/SimpleLocalizationSettings.asset";

        [SerializeField] LocalizationLanguageDatabase defaultLanguage;
        [SerializeField] public bool AutoCreateLocalizationKeys;
        public LocalizationLanguageDatabase DefaultLanguage { get => defaultLanguage; set => defaultLanguage = value; }

        public static SimpleLocalizationSettings Resolve() => GetOrCreateSettings();
        internal static SimpleLocalizationSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<SimpleLocalizationSettings>(LOCALIZATION_SETTINGS_PATH);
            if (settings == null)
            {
                string[] guid = AssetDatabase.FindAssets("t:" + typeof(SimpleLocalizationSettings), null);
                if (guid.Length > 0) settings = AssetDatabase.LoadAssetAtPath<SimpleLocalizationSettings>(AssetDatabase.GUIDToAssetPath(guid[0]));
            }

            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<SimpleLocalizationSettings>();
                AssetDatabase.CreateAsset(settings, LOCALIZATION_SETTINGS_PATH);
                AssetDatabase.SaveAssets();
                Debug.LogError("No Settings found, creating new settings at " + LOCALIZATION_SETTINGS_PATH + ".");
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }

    // Register a SettingsProvider using IMGUI for the drawing framework:
    static class SimpleLocalizationSettingsIMGUIRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateSimpleLocalizationSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider("Project/SimpleLocalizationSettings", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "Simple Localization Settings",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    var settings = SimpleLocalizationSettings.GetOrCreateSettings();
                    var settingsSerealized = SimpleLocalizationSettings.GetSerializedSettings();

                    LocalizationLanguageDatabase defaultLanguage = EditorGUILayout.ObjectField("Default Language", settings.DefaultLanguage, typeof(LocalizationLanguageDatabase), true) as LocalizationLanguageDatabase;
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(settingsSerealized.FindProperty("AutoCreateLocalizationKeys"), new GUIContent(""), GUILayout.Width(20));
                    settingsSerealized.ApplyModifiedProperties();
                    EditorGUILayout.LabelField("Automaticall create new entries for localization when a ILocalizeable Object is created");
                    GUILayout.EndHorizontal();

                    if (settings.DefaultLanguage == null)
                    {
                        EditorGUILayout.HelpBox("It is strongly reccomended to select a default language to use it as a base for localizing to other languages.", MessageType.Warning);
                    }

                    if (defaultLanguage != settings.DefaultLanguage)
                        settings.DefaultLanguage = defaultLanguage;
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Number", "Some String" })
            };

            return provider;
        }
    }
}