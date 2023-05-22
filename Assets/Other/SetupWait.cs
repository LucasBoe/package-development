using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupWait : MonoBehaviour, IOnWaitableSetup
{
    System.Action action = null;
    public void OnWaitableSetup(Action finished)
    {
        Debug.Log("I'm not done yet!");
        action = finished;
    }

    [ContextMenu("SetDone")]
    void SetDone()
    {
        if (action != null)
            action?.Invoke();
    }
}
