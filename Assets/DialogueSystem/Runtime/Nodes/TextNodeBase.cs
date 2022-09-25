using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextNodeBase : ScriptableObject
{
    /// <value>
    /// The GUID of the Node View, used to get the Node View that this Node is associated with.
    /// </value>
    [ReadOnly] public string guid;

    public void GenerateGUID()
    {
        guid = UnityEditor.GUID.Generate().ToString();
    }
}
