using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NeedToLoad : MonoBehaviour, IWaitForMeWhenLoading
{
    public bool DontNeedToWaitAnymore => imDone;
    private bool imDone = false;

    async void Start()
    {
        await Task.Delay(1000);
        imDone = true;
    }
}
