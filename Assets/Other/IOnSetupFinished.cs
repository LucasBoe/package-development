using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnSetupFinished
{
    public abstract void OnSetupFinished();
}

public interface IOnWaitableSetup
{
    public abstract void OnWaitableSetup(System.Action finished);
}
