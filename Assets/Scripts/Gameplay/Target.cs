using System;
using Cysharp.Threading.Tasks;
using Rhythm;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

namespace Gameplay
{
   public class Target : MonoBehaviour
   {
      [SerializeField] private VisualEffect smoke;
      [SerializeField] private VisualEffect trail;
      [SerializeField] private AudioSource bgmSource;
      [SerializeField] private GameObject makimono;

      private Rigidbody _rb;
      private Vector3 _initialPos;

      private async void Start()
      {
         _rb = GetComponent<Rigidbody>();
         _initialPos = transform.position;
         
         var mouseDownStream = this.UpdateAsObservable().Where(_ => OVRInput.GetDown(OVRInput.Button.Start));
         var mouseUpStream = this.UpdateAsObservable().Where(_ => OVRInput.GetUp(OVRInput.Button.Start));
            
         //長押しの判定
         //マウスクリックされたら3秒後にOnNextを流す
         mouseDownStream
            .SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(5)))
            //途中でMouseUpされたらストリームをリセット
            .TakeUntil(mouseUpStream)
            .RepeatUntilDestroy(this.gameObject)
            .Subscribe(_ =>
            {
               INote.NowNoteNum = 0;
               SceneManager.LoadScene("Scenes/opening");
            }).AddTo(this);
      
         await GameManager.Instance.OnFinish.ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
      
         smoke.SendEvent("OnPlay");
         trail.SendEvent("OnPlay");

         await UniTask.Delay(TimeSpan.FromSeconds(6f), cancellationToken: this.GetCancellationTokenOnDestroy());
      
         smoke.SendEvent("OnStop");
         trail.gameObject.SetActive(false);
      
         GameManager.Instance.Distance = (int)(transform.position.z - _initialPos.z);
         makimono.SetActive(true);
         
         bgmSource.Play();
      }
   }
}
