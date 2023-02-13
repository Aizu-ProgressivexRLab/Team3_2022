using System.Threading;
using Cysharp.Threading.Tasks;
using ObjectPool;
using Rhythm;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace System
{
    public class Tutorial : MonoBehaviour
    {
        [SerializeField] private VFXObjectPoolProvider vfxProvider;

        [SerializeField] private VideoClip firstVideo;
        [SerializeField] private VideoClip normalVideo;
        [SerializeField] private VideoClip mashVideo;
        [SerializeField] private VideoClip criticalVideo;

        [SerializeField] private AudioClip bgm;
        [SerializeField] private AudioClip metronome;

        [SerializeField] private VideoPlayer videoPlayer;

        [SerializeField] private Transform eye;
        private float _baseHeight = 1.525f;


        private CancellationToken _ctsOnDestroy;
        private VFXBase _hitFX;

        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = bgm;

            var mouseDownStream = this.UpdateAsObservable().Where(_ => OVRInput.GetDown(OVRInput.Button.Start));
            var mouseUpStream = this.UpdateAsObservable().Where(_ => OVRInput.GetUp(OVRInput.Button.Start));

            mouseDownStream.Subscribe(_ => this.GetComponent<Collider>().enabled = true).AddTo(this);

            //長押しの判定
            //マウスクリックされたら3秒後にOnNextを流す
            mouseDownStream
                .SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(3)))
                //途中でMouseUpされたらストリームをリセット
                .TakeUntil(mouseUpStream)
                .RepeatUntilDestroy(this.gameObject)
                .Subscribe(_ =>
                {
                    INote.NowNoteNum = 0;
                    SceneManager.LoadScene("Scenes/MainScene");
                }).AddTo(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hand"))
            {
                GameStart();
                GameManager.DeltaHeight = eye.position.y - _baseHeight;
                this.transform.position += Vector3.up * GameManager.DeltaHeight;
            }
        }

        private async void GameStart()
        {
            _hitFX = vfxProvider.Get(1).Rent();
            _hitFX.transform.position = transform.position;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<Collider>().enabled = false;

            videoPlayer.gameObject.SetActive(true);
            videoPlayer.clip = firstVideo;
            videoPlayer.isLooping = false;
            videoPlayer.Play();

            await UniTask.Delay(TimeSpan.FromSeconds(firstVideo.length), cancellationToken: _ctsOnDestroy);

            _ctsOnDestroy = this.GetCancellationTokenOnDestroy();
            TutorialFlow();
        }

        private async void TutorialFlow()
        {
            int bc = 1;

            INote.NowNoteNum = 1;

            await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: _ctsOnDestroy);

            videoPlayer.clip = normalVideo;
            videoPlayer.Play();

            await UniTask.Delay(TimeSpan.FromSeconds(normalVideo.length), cancellationToken: _ctsOnDestroy);

            _audioSource.clip = metronome;
            _audioSource.Play();

            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: _ctsOnDestroy);

            CancellationTokenSource cts = new CancellationTokenSource();
            NoteLoop(cts.Token);
            await UniTask.WhenAll(
                UniTask.Delay(TimeSpan.FromSeconds(5f), cancellationToken: _ctsOnDestroy),
                UniTask.WaitUntil(() => ScoreManager.Instance.Score >= 4, cancellationToken: _ctsOnDestroy));
            cts.Cancel();
            _audioSource.Stop();

            // なんとなく間隔開けた
            await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: _ctsOnDestroy);

            ScoreManager.Instance.Score = 0;
            videoPlayer.clip = mashVideo;
            videoPlayer.Play();

            await UniTask.Delay(TimeSpan.FromSeconds(mashVideo.length), cancellationToken: _ctsOnDestroy);

            _audioSource.clip = metronome;
            _audioSource.Play();

            await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: _ctsOnDestroy);

            cts = new CancellationTokenSource();
            MashNoteLoop(cts.Token);
            await UniTask.Delay(TimeSpan.FromSeconds(8f), cancellationToken: _ctsOnDestroy);
            
            cts.Cancel();
            _audioSource.Stop();

            // なんとなく間隔開けた
            await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: _ctsOnDestroy);

            ScoreManager.Instance.Score = 5;
            videoPlayer.clip = criticalVideo;
            videoPlayer.Play();

            await UniTask.Delay(TimeSpan.FromSeconds(criticalVideo.length), cancellationToken: _ctsOnDestroy);

            _audioSource.clip = metronome;
            _audioSource.Play();

            await UniTask.Delay(TimeSpan.FromSeconds(3.3333f), cancellationToken: _ctsOnDestroy);

            cts = new CancellationTokenSource();
            CriticalNoteLoop(cts.Token);
            await UniTask.WhenAll(
                UniTask.Delay(TimeSpan.FromSeconds(criticalVideo.length), cancellationToken: _ctsOnDestroy),
                UniTask.WaitUntil(() => ScoreManager.Instance.Score >= 6, cancellationToken: _ctsOnDestroy));
            cts.Cancel();
            _audioSource.Stop();

            // なんとなく間隔開けた
            await UniTask.Delay(TimeSpan.FromSeconds(5f), cancellationToken: _ctsOnDestroy);

            INote.NowNoteNum = 0;
            SceneManager.LoadScene("Scenes/MainScene");
        }

        private async void NoteLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var n = (NotesController)vfxProvider.Get(0).Rent();
                n.transform.position = transform.position;
                n.Initialize(vfxProvider, INote.NowNoteNum).Forget();

                await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: _ctsOnDestroy);
            }
        }

        private async void MashNoteLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var n = (MashNoteController)vfxProvider.Get(2).Rent();
                n.transform.position = transform.position;
                n.Initialize(vfxProvider, INote.NowNoteNum, 2).Forget();

                await UniTask.Delay(TimeSpan.FromSeconds(4f), cancellationToken: _ctsOnDestroy);
            }
        }

        private async void CriticalNoteLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var n = (CriticalNotesController)vfxProvider.Get(3).Rent();
                n.transform.position = transform.position;
                n.Initialize(vfxProvider, INote.NowNoteNum).Forget();

                await UniTask.Delay(TimeSpan.FromSeconds(4f), cancellationToken: _ctsOnDestroy);
            }
        }
    }
}