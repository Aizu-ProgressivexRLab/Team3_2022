using System.Threading;
using Cysharp.Threading.Tasks;
using ObjectPool;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.VFX;

namespace Rhythm
{
    public class MashNoteController : VFXBase, INote
    {
        [SerializeField, Tooltip("リングが閉じるまでの時間")]
        private float closeTime;

        private float _lifeTime = 0f; // 生成されてからの時間
        private VisualEffect _vfx;
        private VFXObjectPoolProvider _poolProvider;
        private Collider _collider;
        private CancellationTokenSource _cts;
        private VFXBase _hitVFX;
        private float _totalScore;
        private OVRInput.Controller _leftHand;
        private OVRInput.Controller _rightHand;

        public async UniTaskVoid Initialize(VFXObjectPoolProvider pool, int beatCount, float length)
        {
            _leftHand = OVRInput.Controller.LTouch;
            _rightHand = OVRInput.Controller.RTouch;
            _cts = new CancellationTokenSource();
            _poolProvider = pool;
            _vfx = GetComponent<VisualEffect>();
            _vfx.SetFloat("CloseTime", closeTime);
            _vfx.SetFloat("MashTime", length);
            _collider = GetComponent<Collider>();
            _lifeTime = 0;

            // 前のノーツが消えるまで待つ
            await UniTask.WaitUntil(() => INote.NowNoteNum == beatCount, cancellationToken: _cts.Token);

            _collider.enabled = true;
            this.OnTriggerStayAsObservable()
                .Where(x => x.CompareTag("Hand"))
                .Subscribe(_ => Hit()).AddTo(_cts.Token);

            await UniTask.WaitUntil(() => _lifeTime >= closeTime + length, cancellationToken: _cts.Token);

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
            _totalScore = OVRInput.GetLocalControllerAcceleration(_leftHand).magnitude +
                          OVRInput.GetLocalControllerAcceleration(_rightHand).magnitude;
        }
        
        /// <summary>
        /// ノーツの消滅時の処理
        /// </summary>
        private void Finish()
        {
            ScoreManager.Instance.Score += (int) _totalScore;
            _poolProvider.Get(2).Return(this);
            INote.NowNoteNum += 2;
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