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
    public class NotesController : VFXBase, INote
    {
        [SerializeField, Tooltip("リングが閉じるまでの時間")]
        private float closeTime = 0.5f;

        [SerializeField, Tooltip("Perfectが出る範囲")]
        private float perfectRange = 0.1f;

        [SerializeField, Tooltip("Greatが出る範囲")]
        private float greatRange = 0.2f;

        [SerializeField, Tooltip("Goodが出る範囲")] private float goodRange = 0.3f;
        [SerializeField, Tooltip("判定が出る範囲")] private float badRange = 0.5f;

        [SerializeField] private int perfectPoint = 5;
        [SerializeField] private int greatPoint = 4;
        [SerializeField] private int goodPoint = 3;

        private float _lifeTime = 0f; // 生成されてからの時間
        private VisualEffect _vfx;
        private VFXObjectPoolProvider _poolProvider;
        private Collider _collider;
        private CancellationTokenSource _cts;
        private VFXBase _hitVFX;

        // 出現
        public async UniTaskVoid Initialize(VFXObjectPoolProvider pool, int beatCount, float length = 1f)
        {
            _cts = new CancellationTokenSource();
            _poolProvider = pool;
            _vfx = GetComponent<VisualEffect>();
            _vfx.SetFloat("CloseTime", closeTime);
            _collider = GetComponent<Collider>();
            _lifeTime = 0;

            // 前のノーツが消えるまで待つ
            await UniTask.WaitUntil(() => INote.NowNoteNum == beatCount, cancellationToken: _cts.Token);

            _collider.enabled = true;
            this.OnTriggerEnterAsObservable()
                .Where(x => x.CompareTag("Hand"))
                .Subscribe(x =>
                {
                    Hit(x);
                }).AddTo(_cts.Token);
            
            if (beatCount == 0)
            {
                _vfx.enabled = false;
                await UniTask.WaitUntil(() => _lifeTime >= closeTime, cancellationToken: _cts.Token);

                NotesGenerator.PlayingAudioIndex++;
                Finish();
                return;
            }

            await UniTask.WaitUntil(() => _lifeTime >= closeTime + badRange, cancellationToken: _cts.Token);

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
        private void Hit(Collider col)
        {
            var diff = Math.Abs(_lifeTime - closeTime);
            int colorIndex = -1;
            if (diff <= perfectRange)
            {
                ScoreManager.Instance.Score += perfectPoint;
                colorIndex = 0;
                Debug.Log("Perfect" + INote.NowNoteNum);
            }
            else if (diff <= greatRange)
            {
                ScoreManager.Instance.Score += greatPoint;
                colorIndex = 1;
                Debug.Log("Great" + INote.NowNoteNum);
            }
            else if (diff <= goodRange)
            {
                ScoreManager.Instance.Score += goodPoint;
                colorIndex = 2;
                Debug.Log("Good" + INote.NowNoteNum);
            }
            else
            {
                Debug.Log("Bad" + INote.NowNoteNum);
                Finish();
                return;
            }

            WaitHitFX(_hitVFX = _poolProvider.Get(1).Rent(), 1f).Forget();
            _hitVFX.transform.position = transform.position;
            _hitVFX.gameObject.GetComponent<VisualEffect>().SetInt("Index", colorIndex);
            
            if (col.gameObject == GameManager.Instance.LeftHand)
            {
                HandVibrator.Vibrate(OVRInput.Controller.LTouch, 0.1f, col.GetCancellationTokenOnDestroy());
            }
            else if (col.gameObject == GameManager.Instance.RightHand)
            {
                HandVibrator.Vibrate(OVRInput.Controller.RTouch, 0.1f, col.GetCancellationTokenOnDestroy());
            }

            Finish();
        }
        
        /// <summary>
        /// ノーツの消滅時の処理
        /// </summary>
        private void Finish()
        {
            _poolProvider.Get(0).Return(this);
            INote.NowNoteNum++;
            _collider.enabled = false;
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

        private void OnDisable()
        {
            _vfx.enabled = true;
        }
    }
}