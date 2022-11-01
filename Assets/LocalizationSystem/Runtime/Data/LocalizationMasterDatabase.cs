using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
        private void OnEnable()
        {
            ILocalizableEvents.OnDestroyAction -= RemoveKey;
            ILocalizableEvents.OnDestroyAction += RemoveKey;
            ILocalizableEvents.OnCreateAction -= CreateKey;
            ILocalizableEvents.OnCreateAction += CreateKey;
        }


        private void OnDisable()
        {
            ILocalizableEvents.OnDestroyAction -= RemoveKey;
            ILocalizableEvents.OnCreateAction -= CreateKey;
        }
        private void CreateKey(string key)
        {
            Debug.Log("Auto-create entry for " + key);

            SimpleLocalizationSettings settings = SimpleLocalizationSettings.Resolve();
            if (settings == null || !settings.AutoCreateLocalizationKeys || settings.DefaultLanguage == null || settings.DefaultLanguage.HasEntry(key)) return;

            settings.DefaultLanguage.AddEntry(key, "");
        }

        private void RemoveKey(string key)
        {
            int matchIndex = -1;

            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Key == key)
                {
                    matchIndex = i;
                    break;
                }
            }

            if (matchIndex == -1)
            {
                Debug.Log("Deleted ILocalizeableText and found not match with " + key);
            }
            else
            {

                foreach (var item in languages)
                    item.TryRemoveEntry(key);

                keys.RemoveAt(matchIndex);

                Debug.Log("Deleted ILocalizeableText and removed all entries with key: " + key);
            }

        }

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

        internal LocalizationKey AddOrUpdateKey(LocalizationKey key)
        {
            foreach (LocalizationKey existingKey in keys)
            {
                if (existingKey.Key == key.Key)
                {
                    existingKey.Name = key.Name;
                    return existingKey;
                }
            }

            keys.Add(key);
            return key;
        }
#endif
    }

    [System.Serializable]
    public class LocalizationKey
    {
        public string Key;
        public string Name;
        public bool Is(LocalizationKey key) => key.Key == Key;
        public bool Is(string key) => key == Key;
    }
}
