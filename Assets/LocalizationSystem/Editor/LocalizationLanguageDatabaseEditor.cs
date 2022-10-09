using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Simple.Localization
{
    [CustomEditor(typeof(LocalizationLanguageDatabase))]
    public class LocalizationLanguageDatabaseEditor : Editor
    {
        LocalizationLanguageDatabase languageDatabase;
        private void OnEnable()
        {
            languageDatabase = target as LocalizationLanguageDatabase;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (languageDatabase.Entries == null) return;

            foreach (KeyValuePair<LocalizationKey, string> pair in languageDatabase.Entries)
            {
                string newValue = EditorGUILayout.TextField(pair.Key.Key, pair.Value);

                if (pair.Value != newValue)
                    languageDatabase.SetEntry(pair.Key, newValue);
            }
        }
    }
}
