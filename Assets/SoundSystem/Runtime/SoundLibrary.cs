using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Audio;

#if UNITY_EDITOR
using UnityEditor;

namespace SoundSystem
{
#endif

    [CreateAssetMenu(fileName = "SoundLibrary", menuName = "SoundLibrary")]
    public class SoundLibrary : ScriptableObject
    {
        [SerializeField] public AudioMixerGroup audioMixerGroup;
        [SerializeField] private List<Sound> _sounds = new List<Sound>();
        public List<Sound> Sounds { get => _sounds; set => _sounds = value; }
        public static string[] EditorGetSoundOptions()
        {
            SoundLibrary library = GetAllInstances<SoundLibrary>()[0];
            return library.GetAllSounds();
        }

        internal Sound FindSound(string name)
        {
            foreach (Sound data in Sounds)
            {
                if (data.Name == name)
                    return data;
            }

            return null;
        }

        private string[] GetAllSounds()
        {
            List<string> result = new List<string>();
            foreach (Sound data in Sounds)
            {
                result.Add(data.Name);
            }

            return result.ToArray();
        }

        private static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return a;

        }

        public static UnityEngine.Object EditorGetSoundData(string name)
        {
            SoundLibrary library = GetAllInstances<SoundLibrary>()[0];
            foreach (Sound data in library.Sounds)
            {
                if (data.Name == name)
                    return data as System.Object as UnityEngine.Object;
            }

            Debug.Log("Sound " + name + " not found!");

            return null;
        }


#if UNITY_EDITOR
        [ContextMenu("Add New")]
        [Button]
        private void AddNewSoundEntry()
        {
            Sound damageType = ScriptableObject.CreateInstance<Sound>();
            damageType.name = "New Sound";
            damageType.Initialise(this);
            _sounds.Add(damageType);

            AssetDatabase.AddObjectToAsset(damageType, this);
            AssetDatabase.SaveAssets();

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(damageType);
        }

#endif

#if UNITY_EDITOR
        [ContextMenu("Delete all")]
        private void DeleteAllSoundEntrys()
        {
            for (int i = _sounds.Count; i-- > 0;)
            {
                Sound tmp = _sounds[i];

                _sounds.Remove(tmp);
                Undo.DestroyObjectImmediate(tmp);
            }
            AssetDatabase.SaveAssets();
        }

#endif
    }
}