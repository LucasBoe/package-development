using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simple
{ 
    public class ScriptableTextObject : ScriptableObject
    {
        [ReadOnly] public string guid;

        public void GenerateGUID()
        {
            guid = UnityEditor.GUID.Generate().ToString();
        }
    }
}
