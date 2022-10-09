using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface ILocalizeableText
{
    void SetLocalized(bool localized, string key = "");
    bool IsLocalized { get; }
    string Guid { get; }
    string TextValue { get; }
    SerializedProperty TextProperty { get; }
}
