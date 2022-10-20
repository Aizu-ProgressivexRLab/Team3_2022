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
        [SerializeField] private int perfectPoint;
        [SerializeField] private int greatPoint;
        [SerializeField] private int goodPoint;

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

            _collider.enabled = true;
            this.OnTriggerEnterAsObservable()
                .Where(x => x.CompareTag("Hand"))
                .Subscribe(_ => Hit()).AddTo(_cts.Token);

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
        private void Hit()
        {
            var diff = Math.Abs(_lifeTime - closeTime);
            if (diff <= perfectRange)
            {
                ScoreManager.Instance.Score += perfectPoint;
                Debug.Log("Perfect" + INote.NowNoteNum);
            }
            else if (diff <= greatRange)
            {
                ScoreManager.Instance.Score += greatPoint;
                Debug.Log("Great" + INote.NowNoteNum);
            }
            else if (diff <= goodRange)
            {
                ScoreManager.Instance.Score += goodPoint;
                Debug.Log("Good" + INote.NowNoteNum);
            }
            else
            {
                Debug.Log("Bad" + INote.NowNoteNum);
            }

            _hitVFX = _poolProvider.Get(1).Rent();
            _hitVFX.transform.position = transform.position;

            Finish();
        }
        
        /// <summary>
        /// ノーツの消滅時の処理
        /// </summary>
        private void Finish()
        {
            INote.NowNoteNum++;
            _collider.enabled = false;
            _cts.Cancel();
            _cts.Dispose();
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
