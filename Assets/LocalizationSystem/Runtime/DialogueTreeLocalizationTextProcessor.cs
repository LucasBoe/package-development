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
    public override string GetText(ILocalizeableText line)
    {
        if (!line.IsLocalized) return line.TextValue;

        if (!TryGetActiveLanguage()) return null;

        string text = activeLanguage.GetLocalizedText(line.TextValue);
        if (text != "") return text;

        return base.GetText(line);
    }

    public override SerializedProperty GetProperty(ILocalizeableText text)
    {
        if (!text.IsLocalized) return text.TextProperty;

        if (!TryGetActiveLanguage()) return null;

        return activeLanguage.GetObject(text.TextValue);
    }

    private bool TryGetActiveLanguage()
    {
        if (activeLanguage != null) return true;

        Debug.LogError("No active language selected in DialogueTreeLocalizationTextProcessor");
        return false;
    }
}
