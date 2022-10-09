using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Simple.Localization
{
    [CreateAssetMenu(menuName = "Simple Localization System/Language Database")]
    public class LocalizationLanguageDatabase : ScriptableObject
    {
        [SerializeField] private LocalizationMasterDatabase master;
        public string Name;
        public List<LocalizationKeyValuePair> pairs = new List<LocalizationKeyValuePair>();
        public Dictionary<LocalizationKey, string> Entries
        {
            get
            {
                Dictionary<LocalizationKey, string> entries = new Dictionary<LocalizationKey, string>();
                foreach (LocalizationKeyValuePair item in pairs)
                {
                    entries.Add(item.Key, item.Value);
                }

                return entries;
            }
        }

        internal SerializedProperty GetObject(string key)
        {
            SerializedObject serializedDatabase = new SerializedObject(this);
            SerializedProperty serializedPairs = serializedDatabase.FindProperty("pairs");

            for (int i = 0; i < pairs.Count; i++)
            {
                if (pairs[i].Key.Key == key)
                {
                    return serializedPairs.GetArrayElementAtIndex(i).FindPropertyRelative("Value");
                }
            }

            Debug.LogError("Tried to Serialize not translated text.");

            return null;
        }

        public string GetLocalizedText(string key)
        {
            foreach (var item in pairs)
            {
                if (item.Key.Key == key)
                    return item.Value;
            }

            return "";
        }

        public bool HasEntry(string key)
        {
            return pairs.Where(p => p.Key.Key == key).Count() > 0;
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            name = Name;
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(this);
        }

        private void OnEnable()
        {
            if (Application.isPlaying) return;

            master = GetFirstInstanceOf<LocalizationMasterDatabase>();

            if (master == null)
            {
                Debug.LogError("No Master Database Found. Make sure to create on before.");
                return;
            }

            master.AddLanguage(this);
        }

        [ContextMenu("Delete Language")]
        private void DeleteThis()
        {
            master.RemoveLanguage(this);
            Undo.DestroyObjectImmediate(this);
            AssetDatabase.SaveAssets();
        }

        private static T GetFirstInstanceOf<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info

            if (guids.Length == 0)
                return null;

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        public bool HasEntry(LocalizationKey key) => pairs.Where(p => p.Key.Is(key)).Count() > 0;
        public LocalizationKeyValuePair GetEntry(LocalizationKey key) => pairs.Where(p => p.Key.Is(key)).FirstOrDefault();
        public void AddEntry(LocalizationKey key, string value) => pairs.Add(new LocalizationKeyValuePair() { Key = master.AddOrUpdateKey(key), Value = value });
        public void AddEntry(string key, string value) => pairs.Add(new LocalizationKeyValuePair() { Key = master.AddKey(key), Value = value });
        public void SetEntry(LocalizationKey key, string value)
        {
            foreach (var pair in pairs)
            {
                if (pair.Key.Is(key))
                    pair.Value = value;
            }
            //pairs.Where(p => p.Key == key).Select(p => p.Value = textAfter);
        }

        public void TryRemoveEntry(string key)
        {
            LocalizationKeyValuePair match = null;
            foreach (var pair in pairs)
            {
                if (pair.Key.Is(key))
                    match = pair;
            }

            if (match != null)
                pairs.Remove(match);
        }

#endif
    }

    [System.Serializable]
    public class LocalizationKeyValuePair
    {
        public LocalizationKey Key;
        public string Value;
    }
}