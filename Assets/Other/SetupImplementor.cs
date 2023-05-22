using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupImplementor : MonoBehaviour, IOnSetupFinished
{
    public void OnSetupFinished()
    {
        Debug.Log("Yeah!");
    }
}
