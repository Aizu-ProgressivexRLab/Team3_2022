using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

namespace Rhythm
{
    public class NotesController : MonoBehaviour
    {
        [SerializeField] private float closeTime;
        [SerializeField] private float perfectRange;
        [SerializeField] private float greatRange;
        [SerializeField] private float goodRange;
        [SerializeField] private float badRange;

        private float _lifeTime;
        private VisualEffect _vfx;

        private async void Start()
        {
            _vfx = GetComponent<VisualEffect>();
            _vfx.SetFloat("CloseTime", closeTime);

            await UniTask.Delay(TimeSpan.FromSeconds(closeTime),
                cancellationToken: this.GetCancellationTokenOnDestroy());
            
            NotesGenerator.IsAudioPlay = true;
            GetComponent<AudioSource>().Play();
        }

        private void FixedUpdate()
        {
            _lifeTime += Time.deltaTime;
        }

        private void Hit()
        {
            var diff = Math.Abs(_lifeTime - closeTime);
            if (diff <= perfectRange)
            {
                Debug.Log("Perfect");
            }
            else if (diff <= greatRange)
            {
                Debug.Log("Great");
            }
            else if (diff <= goodRange)
            {
                Debug.Log("Good");
            }
            else if(diff <= badRange)
            {
                Debug.Log("Bad");
            }
        }
    }
}
