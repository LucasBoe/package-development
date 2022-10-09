using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface ILocalizeableText
{
    void SetIsLocalized(bool localized, string key = "");
    string GetGUID();
    string GetValue();
    bool IsLocalized { get; }
    string Text { get; }
    SerializedProperty Property { get; }
}
