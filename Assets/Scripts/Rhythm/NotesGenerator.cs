using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

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

        private int[] _scoreNum;
        private int[] _scoreBlock;
        private int _BPM;
        private int _LPB;
        private float _offset;

        private float _nowTime;
        private float _moveSpan = 0.02f;
        private int _beatCount;
        private int _beatNum;
        private bool _isBeat;

        private AudioSource _audioSource;

        [SerializeField] private GameObject notePrefab;

        public static bool IsAudioPlay = false;

        private async void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            
            ReadMusic();

            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space),
                cancellationToken: this.GetCancellationTokenOnDestroy());
            
            this.FixedUpdateAsObservable()
                .Subscribe(_ => GenerateNotes())
                .AddTo(this);

            await UniTask.WaitUntil(() => IsAudioPlay, cancellationToken: this.GetCancellationTokenOnDestroy());
            _audioSource.Play();
        }

        private void ReadMusic()
        {
            string inputString = Resources.Load<TextAsset>("Noesis").ToString();
            InputJson inputJson = JsonUtility.FromJson<InputJson>(inputString);

            _scoreNum = new int[inputJson.notes.Length];
            _scoreBlock = new int[inputJson.notes.Length];
            _BPM = inputJson.BPM;
            _LPB = inputJson.notes[0].LPB;
            _offset = inputJson.offset * 0.0001f;

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
                    Instantiate(notePrefab);
                }

                _beatCount++;
                _isBeat = false;
            }
        }
    }
}
