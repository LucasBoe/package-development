using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Simple.Localization
{
    [CreateAssetMenu(menuName = "Simple Localization System/Master Database")]
    public class LocalizationMasterDatabase : ScriptableObject
    {
        [SerializeField] List<LocalizationKey> keys = new List<LocalizationKey>();
        [SerializeField] List<LocalizationLanguageDatabase> languages = new List<LocalizationLanguageDatabase>();
        public List<LocalizationKey> Keys => keys;
        public List<LocalizationLanguageDatabase> Languages => languages;


#if UNITY_EDITOR
        internal void AddLanguage(LocalizationLanguageDatabase localizationLanguageDatabase)
        {
            if (!languages.Contains(localizationLanguageDatabase))
                languages.Add(localizationLanguageDatabase);

            if (AssetDatabase.GetAssetPath(this) == AssetDatabase.GetAssetPath(localizationLanguageDatabase))
                return;

            AssetDatabase.AddObjectToAsset(localizationLanguageDatabase, this);
            AssetDatabase.SaveAssets();

            EditorUtility.SetDirty(localizationLanguageDatabase);
            EditorUtility.SetDirty(this);
        }

        internal void RemoveLanguage(LocalizationLanguageDatabase localizationLanguageDatabase)
        {
            languages.Remove(localizationLanguageDatabase);
        }

        internal LocalizationKey AddKey(string key)
        {
            LocalizationKey newKey = new LocalizationKey() { Key = key };
            keys.Add(newKey);
            return newKey;
        }
#endif
    }

    [System.Serializable]
    public class LocalizationKey
    {
        public string Key;
        public bool Is(LocalizationKey key) => key.Key == Key;
    }
}
