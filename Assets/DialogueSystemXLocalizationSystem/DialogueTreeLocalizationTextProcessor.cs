using Simple.DialogueTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simple.Localization;
using Simple.DialogueTree.Nodes;
using UnityEditor;

[CreateAssetMenu]
public class DialogueTreeLocalizationTextProcessor : DialogueTreeTextProcessor
{
    [SerializeField] LocalizationLanguageDatabase activeLanguage;
    public override string FindText(ILocalizeableText line)
    {
        if (!line.IsLocalized) return line.Text;

        if (!TryGetActiveLanguage()) return null;

        string text = activeLanguage.GetLocalizedText(line.Text);
        if (text != "") return text;

        return base.FindText(line);
    }

    public override SerializedProperty FindProperty(ILocalizeableText text)
    {
        if (!text.IsLocalized) return text.Property;

        if (!TryGetActiveLanguage()) return null;

        return activeLanguage.GetObject(text.Text);
    }

    private bool TryGetActiveLanguage()
    {
        if (activeLanguage != null) return true;

        Debug.LogError("No active language selected in DialogueTreeLocalizationTextProcessor");
        return false;
    }
}
