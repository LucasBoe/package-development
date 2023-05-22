using UnityEngine;
using UnityEditor;
 
public class CreateSoundFromSoundFile
{
    // Adding a new context menu item
    [MenuItem("Assets/Create Sound", true)]
    static bool ValidateLogSelectedTransformName()
    {
        if (Selection.count == 0)
            return false;

        else if (Selection.count > 1)
        {
            foreach (var obj in Selection.objects)
            {
                if (obj as AudioClip == null)
                    return false;
            }
            return true;
        }

        return (Selection.activeObject as AudioClip) != null;
    }

    // Put menu item at top near other "Create" options
    [MenuItem("Assets/Create Sound", false, 10)] //10
    private static void EmptyParent(MenuCommand menuCommand)
    {
        // Use selected item as our context (otherwise does nothing because of above)
        //GameObject selected = menuCommand.context as GameObject;
        GameObject selected = Selection.activeObject as GameObject;

        // Create a empty game object with same name
        GameObject go = new GameObject(selected.name);

        // adjust hierarchy accordingly
        GameObjectUtility.SetParentAndAlign(go, selected.transform.parent.gameObject);
        go.transform.position = selected.transform.position;
        go.transform.rotation = selected.transform.rotation;
        GameObjectUtility.SetParentAndAlign(selected, go);

        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Parented " + go.name);

        // Yea!
        Debug.Log("Created a Parent for " + selected.name + ".");
    }
}
