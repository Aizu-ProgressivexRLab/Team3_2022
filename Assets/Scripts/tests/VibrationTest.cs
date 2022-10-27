using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class VibrationTest : MonoBehaviour
{
    private async void Start()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(3));
        
        HandVibrator.Vibrate(OVRInput.Controller.RTouch, 2, this.GetCancellationTokenOnDestroy(), 1f, 1f);

        await UniTask.Delay(TimeSpan.FromSeconds(3));
        
        HandVibrator.Vibrate(OVRInput.Controller.RTouch, 2, this.GetCancellationTokenOnDestroy(), 2f, 2f);

        await UniTask.Delay(TimeSpan.FromSeconds(3));
        
        HandVibrator.Vibrate(OVRInput.Controller.RTouch, 2, this.GetCancellationTokenOnDestroy(), 0.1f, 1f);

        await UniTask.Delay(TimeSpan.FromSeconds(3));
        
        HandVibrator.Vibrate(OVRInput.Controller.RTouch, 2, this.GetCancellationTokenOnDestroy(), 1f, 1f);
    }
}
