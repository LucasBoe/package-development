using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineUtil
{
    public static void ExecuteFloatRoutine(float from, float to, Action<float> function, MonoBehaviour behaviour, float duration = 1f)
    {
        behaviour.StartCoroutine(FloatRoutine(from, to, duration, function));
    }

    private static IEnumerator FloatRoutine(float min, float max, float duration, System.Action<float> function)
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            function?.Invoke(Mathf.Lerp(min, max, t));
            yield return null;
        }

        function?.Invoke(max);
    }

    public static void Delay(System.Action function, MonoBehaviour behaviour, float delay = 1f)
    {
        behaviour.StartCoroutine(DelayRoutine(function, delay));
    }

    private static IEnumerator DelayRoutine(System.Action function, float delay)
    {
        yield return new WaitForSeconds(delay);
        function?.Invoke();
    }
}
