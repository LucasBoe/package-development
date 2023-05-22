using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Loading
{
    public class LoadingHandler : MonoBehaviour
    {
        public System.Action<float> OnLoadingProgress;
        public System.Action OnLoadingFinished;

        async void Start()
        {
            MonoBehaviour[] monoBehaviours = FindObjectsOfType<MonoBehaviour>();


            var waitables = monoBehaviours.OfType<IWaitForMeWhenLoading>();

            var finishedCounter = 0;
            var neededCounter = waitables.Count();

            while (finishedCounter != neededCounter)
            {
                var c = 0;
                foreach (IWaitForMeWhenLoading wait in waitables)
                {
                    if (wait.DontNeedToWaitAnymore) c++;
                }

                if (c > finishedCounter)
                {
                    finishedCounter = c;
                    OnLoadingProgress?.Invoke((float) neededCounter / finishedCounter);
                }

                await Task.Yield();
            }

            OnLoadingFinished?.Invoke();
        }
    }
}
