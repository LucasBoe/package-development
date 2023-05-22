using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SoundWrapper : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {
            if (!IsAudio(str))
                continue;
        }
        foreach (string str in deletedAssets)
        {
            if (!IsAudio(str))
                continue;
        }
    }
    public static bool IsAudio(string path)
    {
        var ending = path.Split('.').Last();

        switch (ending)
        {
            case "mp3":
            case "ogg":
            case "wav":
            case "aiff":
            case "aif":
            case "mod":
            case "it":
            case "s3m":
            case "xm":
                return true;
        }

        return false;
    }
}
