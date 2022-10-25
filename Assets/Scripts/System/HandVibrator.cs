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
        public static async void Vibrate(OVRInput.Controller controller, float time, CancellationToken cts)
        {
            OVRInput.SetControllerVibration(0.1f, 0.1f, controller);

            await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: cts);
            
            OVRInput.SetControllerVibration(0, 0, controller);
        }
    }
}
