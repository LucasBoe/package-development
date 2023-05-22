using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SetupHandler : MonoBehaviour
{
    void Awake() => StartSetup();

    public async void StartSetup()
    {
        MonoBehaviour[] monoBehaviours = FindObjectsOfType<MonoBehaviour>();

        var finishedCounter = 0;
        var neededCounter = 0;

        var waitables = monoBehaviours.OfType<IOnWaitableSetup>();

        foreach (var monoBehaviour in monoBehaviours)

        foreach (IOnWaitableSetup wait in waitables)
        {
            neededCounter++;
            wait.OnWaitableSetup(() => finishedCounter++);
        }

        while (finishedCounter != neededCounter)
            await Task.Yield();

        Debug.Log($"finishedCounter = { finishedCounter }");
        Debug.Log($"neededCounter = { neededCounter }");

        foreach (IOnSetupFinished finish in monoBehaviours.OfType<IOnSetupFinished>())
        {
            finish.OnSetupFinished();
        }

        await Task.Delay(1000);
    }
}
