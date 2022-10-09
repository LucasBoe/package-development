using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Simple.DialogueTree
{
    // Create a new type of Settings Asset.
    class SimpleDialogueSettings : ScriptableObject
    {
        public const string DIALOGUE_SETTINGS_PATH = "Assets/SimpleDialogueSettings.asset";

        [SerializeField]
        private int m_Number;

        [SerializeField]
        private string m_SomeString;

        [SerializeField]
        DialogueTreeTextProcessor textProcessor;

        public DialogueTreeTextProcessor TextProcessor { get => textProcessor; set => textProcessor = value; }

        internal static DialogueTreeTextProcessor Resolve()
        {
            SimpleDialogueSettings settings = GetOrCreateSettings();
            if (settings != null) return settings.TextProcessor;

            return null;
        }

        internal static SimpleDialogueSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<SimpleDialogueSettings>(DIALOGUE_SETTINGS_PATH);
            if (settings == null)
            {
                string[] guid = AssetDatabase.FindAssets("t:" + typeof(SimpleDialogueSettings), null);
                if (guid.Length > 0) settings = AssetDatabase.LoadAssetAtPath<SimpleDialogueSettings>(AssetDatabase.GUIDToAssetPath(guid[0]));
            }

            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<SimpleDialogueSettings>();
                settings.m_Number = 42;
                settings.m_SomeString = "The answer to the universe";
                AssetDatabase.CreateAsset(settings, DIALOGUE_SETTINGS_PATH);
                AssetDatabase.SaveAssets();
                Debug.LogError("No Settings found, creating new settings at " + DIALOGUE_SETTINGS_PATH + ".");
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }

    // Register a SettingsProvider using IMGUI for the drawing framework:
    static class SimpleDialogueSettingsIMGUIRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateSimpleDialogueSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider("Project/SimpleDialogueSettings", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "Simple Dialogue Settings",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    var settings = SimpleDialogueSettings.GetOrCreateSettings();
                    var serializedSettings = SimpleDialogueSettings.GetSerializedSettings();
                    EditorGUILayout.PropertyField(serializedSettings.FindProperty("m_Number"), new GUIContent("My Number"));
                    EditorGUILayout.PropertyField(serializedSettings.FindProperty("m_SomeString"), new GUIContent("My String"));

                    DialogueTreeTextProcessor processor = EditorGUILayout.ObjectField(settings.TextProcessor, typeof(DialogueTreeTextProcessor), true) as DialogueTreeTextProcessor;

                    if (processor != settings.TextProcessor)
                        settings.TextProcessor = processor;
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Number", "Some String" })
            };

            return provider;
        }
    }
}