using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Simple.DialogueTree;
using Simple.Localization;
using System.Linq;

[CustomEditor(typeof(DialogueTree))]
public class DialogueTreeEditor : Editor
{
    bool isCreating = false;
    private LocalizationLanguageDatabase selectedLanguageDatabase;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (isCreating)
        {
            selectedLanguageDatabase = EditorGUILayout.ObjectField(selectedLanguageDatabase, typeof(LocalizationLanguageDatabase), true) as LocalizationLanguageDatabase;
            if (selectedLanguageDatabase != null && GUILayout.Button("Create Localization Keys"))
            {
                isCreating = false;

                DialogueTree tree = target as DialogueTree;

                List<ILocalizeableText> localizableTexts = new List<ILocalizeableText>();

                localizableTexts.AddRange(tree.GetNodes().Where(n => (n as ILocalizeableText) != null).Select(n => n as ILocalizeableText));

                foreach (var choice in tree.GetNodes().Where(n => (n as Simple.DialogueTree.Nodes.Choice) != null).Select(n => n as Simple.DialogueTree.Nodes.Choice))
                {
                    foreach (var opt in choice.Options)
                    {
                        localizableTexts.Add(opt);
                    }
                }

                foreach (var text in localizableTexts)
                {
                    Debug.Log(text);

                    string key = text.GetGUID();
                    if (!selectedLanguageDatabase.HasEntry(key))
                    {
                        selectedLanguageDatabase.AddEntry(key, text.GetValue());
                        text.SetIsLocalized(true, key);
                    }
                }
            }
        }
        else
        {
            if (GUILayout.Button("Create Localization Keys"))
            {
                isCreating = true;
            }
        }
    }
}
