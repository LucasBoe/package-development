using Loading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] LoadingHandler loading;

    private void OnEnable()
    {
        loading.OnLoadingProgress += OnLoadingProgress;
        loading.OnLoadingFinished += OnLoadingFinished;
    }
    private void OnDisable()
    {
        loading.OnLoadingProgress -= OnLoadingProgress;
        loading.OnLoadingFinished -= OnLoadingFinished;
    }

    private void OnLoadingProgress(float loaded)
    {
        Debug.Log(loaded);
    }
    private void OnLoadingFinished()
    {
        Debug.Log("OnLoadingFinished");
    }
}