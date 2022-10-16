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
        private float closeTime;

        [SerializeField, Tooltip("Perfectが出る範囲")]
        private float perfectRange;

        [SerializeField, Tooltip("Greatが出る範囲")]
        private float greatRange;

        [SerializeField, Tooltip("Goodが出る範囲")] private float goodRange;
        [SerializeField, Tooltip("判定が出る範囲")] private float badRange;

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
            if (_hitVFX != null)
            {
                _poolProvider.Get(1).Return(_hitVFX);   
            }

            // 前のノーツが消えるまで待つ
            await UniTask.WaitUntil(() => INote.NowNoteNum == beatCount, cancellationToken: _cts.Token);

            _collider.enabled = true;
            this.OnTriggerEnterAsObservable()
                .Where(x => x.CompareTag("Hand"))
                .Subscribe(_ => Hit()).AddTo(_cts.Token);

            if (beatCount == 0)
            {
                await UniTask.WaitUntil(() => _lifeTime >= closeTime, cancellationToken: _cts.Token);

                NotesGenerator.IsAudioPlay = true;
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
        private void Hit()
        {
            var diff = Math.Abs(_lifeTime - closeTime);
            if (diff <= perfectRange)
            {
                Debug.Log("Perfect" + INote.NowNoteNum);
            }
            else if (diff <= greatRange)
            {
                Debug.Log("Great" + INote.NowNoteNum);
            }
            else if (diff <= goodRange)
            {
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
            _poolProvider.Get(0).Return(this);
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