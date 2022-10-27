using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ObjectPool;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.VFX;

namespace Rhythm
{
    public class CriticalNotesController : VFXBase, INote
    {
        [SerializeField, Tooltip("リングが閉じるまでの時間")]
        private float closeTime = 0.5f;

        [SerializeField, Tooltip("Perfectが出る範囲")]
        private float perfectRange = 0.1f;

        [SerializeField, Tooltip("Greatが出る範囲")]
        private float greatRange = 0.2f;

        [SerializeField, Tooltip("Goodが出る範囲")] private float goodRange = 0.3f;
        [SerializeField, Tooltip("判定が出る範囲")] private float badRange = 0.5f;

        private float _lifeTime = 0f; // 生成されてからの時間
        private VisualEffect _vfx;
        private VFXObjectPoolProvider _poolProvider;
        private Collider _collider;
        private CancellationTokenSource _cts;
        private VFXBase _hitVFX;
        [SerializeField] private float perfectMultiply = 2.0f;
        [SerializeField] private float greatMultiply = 1.5f;
        [SerializeField] private float goodMultiply = 1.2f;
        [SerializeField] private float badMultiply = 1.0f;

        [SerializeField] private AudioClip hitSound;
        
        private GameObject _target;

        // 出現
        public async UniTaskVoid Initialize(VFXObjectPoolProvider pool, int beatCount, float length = 1f)
        {
            _cts = new CancellationTokenSource();
            _poolProvider = pool;
            _vfx = GetComponent<VisualEffect>();
            _vfx.SetFloat("CloseTime", closeTime);
            _collider = GetComponent<Collider>();
            _lifeTime = 0;
            if (_hitVFX != null)
            {
                _poolProvider.Get(1).Return(_hitVFX);   
            }
            _target = GameObject.FindWithTag("target");
            if (_target != null)
            {
                _target.GetComponent<Collider>().enabled = true;   
            }

            await UniTask.Delay(TimeSpan.FromSeconds(closeTime / 3),
                cancellationToken: this.GetCancellationTokenOnDestroy());
            
            _collider.enabled = true;
            
            this.OnTriggerEnterAsObservable()
                .Where(x => x.CompareTag("Hand"))
                .Subscribe(x =>
                {
                    if (x.gameObject == GameManager.Instance.LeftHand)
                    {
                        HandVibrator.Vibrate(OVRInput.Controller.LTouch, 0.1f, x.GetCancellationTokenOnDestroy(), 1f, 1f);
                    }
                    else if (x.gameObject == GameManager.Instance.RightHand)
                    {
                        HandVibrator.Vibrate(OVRInput.Controller.RTouch, 0.1f, x.GetCancellationTokenOnDestroy(), 1f, 1f);
                    }
                    Hit();
                }).AddTo(_cts.Token);

            await GameManager.Instance.OnFinish.ToUniTask(cancellationToken: _cts.Token);

            // 時間切れ 
            Finish();
        }

        private void FixedUpdate()
        {
            _lifeTime += Time.deltaTime;
        }

        /// <summary>
        /// パンチがノーツに当たった時の処理
        /// </summary>
        private void Hit()
        {
            var diff = Math.Abs(_lifeTime - closeTime);
            if (diff <= perfectRange)
            {
                ScoreManager.Instance.Score = (int)(ScoreManager.Instance.Score * perfectMultiply);
                Debug.Log("Perfect" + INote.NowNoteNum);
            }
            else if (diff <= greatRange)
            {
                ScoreManager.Instance.Score = (int)(ScoreManager.Instance.Score * greatMultiply);
                Debug.Log("Great" + INote.NowNoteNum);
            }
            else if (diff <= goodRange)
            {
                ScoreManager.Instance.Score = (int)(ScoreManager.Instance.Score * goodMultiply);
                Debug.Log("Good" + INote.NowNoteNum);
            }
            else
            {
                ScoreManager.Instance.Score = (int)(ScoreManager.Instance.Score * badMultiply);
                Debug.Log("Bad" + INote.NowNoteNum);
            }
            
            WaitHitFX(_hitVFX = _poolProvider.Get(1).Rent(), 1f).Forget();
            _hitVFX.transform.position = transform.position;
            DestroySoundManager.Play(hitSound);
            GameManager.Instance.OnFinish.OnNext(Unit.Default);
            GameManager.Instance.OnFinish.OnCompleted();

            Finish();
        }
        
        /// <summary>
        /// ノーツの消滅時の処理
        /// </summary>
        private void Finish()
        {
            INote.NowNoteNum++;
            _collider.enabled = false;
            _vfx.SendEvent("OnStop");
            _poolProvider.Get(3).Return(this);
            _cts.Cancel();
            _cts.Dispose();
        }
        
        private async UniTaskVoid WaitHitFX(VFXBase vfx, float waitTime)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: this.GetCancellationTokenOnDestroy());
            _poolProvider.Get(vfx.Id).Return(vfx);
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
