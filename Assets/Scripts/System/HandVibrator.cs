using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace System
{
    public static class HandVibrator
    {
        /// <summary>
        /// コントローラーを振動させる
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="time">継続時間</param>
        /// <param name="cts"></param>
        public static async void Vibrate(OVRInput.Controller controller, float time, CancellationToken cts, float frequency = 0.5f, float amplitude = 0.5f)
        {
            OVRInput.SetControllerVibration(frequency, amplitude, controller);

            await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: cts);
            
            OVRInput.SetControllerVibration(0, 0, controller);
        }
    }
}
