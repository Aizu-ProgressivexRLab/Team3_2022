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
    public class NotesController : MonoBehaviour
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
        private VFXObjectPool _pool;
        private Collider _collider;
        private CancellationTokenSource _cts;

        /// <summary>
        /// 現在判定中のノーツ
        /// </summary>
        public static int NowNoteNum = 0;

        // 出現
        public async UniTaskVoid Initialize(VFXObjectPool pool, int beatCount)
        {
            _cts = new CancellationTokenSource();
            _pool = pool;
            _vfx = GetComponent<VisualEffect>();
            _vfx.SetFloat("CloseTime", closeTime);
            _collider = GetComponent<Collider>();
            _lifeTime = 0;

            // 前のノーツが消えるまで待つ
            await UniTask.WaitUntil(() => NowNoteNum == beatCount, cancellationToken: _cts.Token);

            _collider.enabled = true;
            this.OnTriggerEnterAsObservable()
                .Where(x => x.CompareTag("Hand"))
                .Subscribe(_ => Hit()).AddTo(_cts.Token);

            if (beatCount == 0)
            {
                await UniTask.WaitUntil(() => _lifeTime >= closeTime, cancellationToken: _cts.Token);

                NotesGenerator.IsAudioPlay = true;
                Finish();
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
                Debug.Log("Perfect" + NowNoteNum);
            }
            else if (diff <= greatRange)
            {
                Debug.Log("Great" + NowNoteNum);
            }
            else if (diff <= goodRange)
            {
                Debug.Log("Good" + NowNoteNum);
            }
            else
            {
                Debug.Log("Bad" + NowNoteNum);
            }

            Finish();
        }

        /// <summary>
        /// ノーツの消滅時の処理
        /// </summary>
        private void Finish()
        {
            _pool.Return(this);
            NowNoteNum++;
            _collider.enabled = false;
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }

            _cts.Dispose();
        }
    }
}