using System.Collections.Generic;
using System.Linq;
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
        
        private hit2 _leftHand;
        private hit2 _rightHand;

        private float _lifeTime = 0f; // 生成されてからの時間
        private VisualEffect _vfx;
        private VFXObjectPoolProvider _poolProvider;
        private Collider _collider;
        private CancellationTokenSource _cts;
        private VFXBase _hitVFX;
        private float _totalScore;
        private MeshRenderer _crackShader;
        private float _length;
        private Queue<float> _recentAcc = new Queue<float>();

        public async UniTaskVoid Initialize(VFXObjectPoolProvider pool, int beatCount, float length)
        {
            _cts = new CancellationTokenSource();
            _poolProvider = pool;
            _vfx = GetComponent<VisualEffect>();
            _vfx.SetFloat("CloseTime", closeTime);
            _vfx.SetFloat("MashTime", length);
            _collider = GetComponent<Collider>();
            _lifeTime = 0;
            _crackShader = GetComponentInChildren<MeshRenderer>();
            _leftHand = GameManager.Instance.LeftHand.GetComponent<hit2>();
            _rightHand = GameManager.Instance.RightHand.GetComponent<hit2>();
            _length = length;

            // 前のノーツが消えるまで待つ
            await UniTask.WaitUntil(() => INote.NowNoteNum == beatCount, cancellationToken: _cts.Token);

            _collider.enabled = true;
            this.OnTriggerEnterAsObservable()
                .Where(x => x.CompareTag("Hand"))
                .Subscribe(_ => Hit()).AddTo(_cts.Token);

            this.FixedUpdateAsObservable()
                .ThrottleFirstFrame(10)
                .Subscribe(_ =>
                {
                    _recentAcc.Enqueue(0);
                    if (_recentAcc.Count % 5 == 0)
                    {
                        _recentAcc.Dequeue();
                    }
                }).AddTo(_cts.Token);

            await UniTask.WaitUntil(() => _lifeTime >= closeTime + length, cancellationToken: _cts.Token);

            // 時間切れ 
            Finish();
        }
        
        private void FixedUpdate()
        {
            _lifeTime += Time.deltaTime;
            
            if (_recentAcc.Count != 0)
            {
                _crackShader.material.SetFloat("_ColorExposure", _recentAcc.Average() * 20f);
                Debug.Log(_recentAcc.Average());   
            }
        }

        /// <summary>
        /// パンチがノーツに当たった時の処理
        /// </summary>
        private void Hit()
        {
            // _totalScore = OVRInput.GetLocalControllerAcceleration(_leftHand).magnitude +
            //               OVRInput.GetLocalControllerAcceleration(_rightHand).magnitude;

            var score = _leftHand.Acceleration + _rightHand.Acceleration;
            
            _totalScore += score;
            _recentAcc.Enqueue(score);
            if (_recentAcc.Count % 5 == 0)
            {
                _recentAcc.Dequeue();
            }
            _crackShader.material.SetFloat("_Exposure", _totalScore * 1 / (0.78f * _length));
        }
        
        /// <summary>
        /// ノーツの消滅時の処理
        /// </summary>
        private void Finish()
        {
            Debug.Log(_totalScore);
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