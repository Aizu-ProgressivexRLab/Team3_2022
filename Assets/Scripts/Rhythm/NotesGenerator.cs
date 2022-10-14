using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using ObjectPool;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rhythm
{
    public class NotesGenerator : MonoBehaviour
    {
        [Serializable]
        public class InputJson
        {
            public Notes[] notes;
            public int BPM;
            public int offset;
        }

        [Serializable]
        public class Notes
        {
            public int num;
            public int block;
            public int LPB;
        }
        
        [SerializeField, Tooltip("子オブジェクトがスポナー")] private GameObject spawnPosParent;
        private List<Transform> _spawnPos;
        private Transform _prePos; // 重複防止用

        /// <summary>
        /// (ノーツがある)譜面の位置番号
        /// </summary>
        private int[] _scoreNum;   
        
        /// <summary>
        /// (ノーツがある)譜面のレーン番号
        /// </summary>
        private int[] _scoreBlock;
        
        private int _BPM;
        
        /// <summary>
        /// 1拍当たりの分割数
        /// </summary>
        private int _LPB;
        
        private float _offset;

        private float _nowTime;
        private float _moveSpan = 0.02f;    // FixedUpdateの間隔
        
        /// <summary>
        /// 生成したノーツ数
        /// </summary>
        private int _beatCount;
        
        /// <summary>
        /// 現在の再生位置
        /// </summary>
        private int _beatNum;
        
        private bool _isBeat;   // かぶり防止用

        private AudioSource _audioSource;
        private VFXObjectPoolProvider _vfxProvider;
        private VFXObjectPool _vfxPool;

        public static bool IsAudioPlay = false;

        private async void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _vfxProvider = GetComponent<VFXObjectPoolProvider>();

            _spawnPos = spawnPosParent.GetComponentsInChildren<Transform>().ToList();

            // オブジェクトプールを生成
            _vfxPool = _vfxProvider.Get();
            
            ReadMusic();

            // スタート条件
            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space),
                cancellationToken: this.GetCancellationTokenOnDestroy());
            
            this.FixedUpdateAsObservable()
                .Subscribe(_ => GenerateNotes())
                .AddTo(this);

            // 最初の空ノーツが判定ラインに来たら音楽を鳴らす
            await UniTask.WaitUntil(() => IsAudioPlay, cancellationToken: this.GetCancellationTokenOnDestroy());
            _audioSource.Play();
        }

        /// <summary>
        /// Jsonファイルを読み込む
        /// </summary>
        private void ReadMusic()
        {
            string inputString = Resources.Load<TextAsset>("Noesis").ToString();
            InputJson inputJson = JsonUtility.FromJson<InputJson>(inputString);

            // 値を各変数に代入
            _scoreNum = new int[inputJson.notes.Length];
            _scoreBlock = new int[inputJson.notes.Length];
            _BPM = inputJson.BPM;
            _LPB = inputJson.notes[0].LPB;
            _offset = inputJson.offset * 0.0001f;   // Offsetの単位がわからないから大体の値

            for (int i = 0; i < inputJson.notes.Length; i++)
            {
                _scoreNum[i] = inputJson.notes[i].num;
                _scoreBlock[i] = inputJson.notes[i].block;
            }
        }

        private void GetScoreTime()
        {
            _nowTime += _moveSpan;
            
            if (_beatCount > _scoreNum.Length) return;

            _beatNum = (int) (_nowTime * _BPM / 60 * _LPB - _offset);
        }

        private void GenerateNotes()
        {
            GetScoreTime();

            if (_beatCount < _scoreNum.Length)
            {
                _isBeat = (_scoreNum[_beatCount] == _beatNum);
            }

            if (_isBeat)
            {
                if (_scoreBlock[_beatCount] == 0)
                {
                    var note = _vfxPool.Rent();
                    note.transform.position = GetRandomPosition();
                    note.Initialize(_vfxPool, _beatCount).Forget();
                }
                else
                {
                    NotesController.NowNoteNum++;
                }

                _beatCount++;
                _isBeat = false;
            }
        }

        private Vector3 GetRandomPosition()
        {
            if (_prePos != null)
            {
                _spawnPos.Add(_prePos);
            }
            var rand = Random.Range(0, _spawnPos.Count);
            _prePos = _spawnPos[rand];
            _spawnPos.RemoveAt(rand);
            
            return _prePos.position;
        }
    }
}