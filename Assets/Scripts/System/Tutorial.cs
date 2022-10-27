using System.Threading;
using Cysharp.Threading.Tasks;
using ObjectPool;
using Rhythm;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace System
{
    public class Tutorial : MonoBehaviour
    {
        [SerializeField] private VFXObjectPoolProvider vfxProvider;

        private CancellationToken _ctsOnDestroy;
        private VFXBase _hitFX;

        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hand"))
            {
                GameStart();
                _audioSource.Stop();
            }
        }

        private void GameStart()
        {
            _hitFX = vfxProvider.Get(1).Rent();
            _hitFX.transform.position = transform.position;
            gameObject.SetActive(false);
            
            TutorialFlow();
            _ctsOnDestroy = this.GetCancellationTokenOnDestroy();
        }

        private async void TutorialFlow()
        {
            int bc = 1;
            
            INote.NowNoteNum = 1;
            
            await UniTask.Delay(TimeSpan.FromSeconds(5f), cancellationToken: _ctsOnDestroy);

            for (int i = 0; i < 3; i++)
            {
                var n = (NotesController)vfxProvider.Get(0).Rent();
                n.transform.position = transform.position;
                bc = INote.NowNoteNum;
                n.Initialize(vfxProvider, bc).Forget();

                await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: _ctsOnDestroy);   
            }

            // なんとなく間隔開けた
            await UniTask.Delay(TimeSpan.FromSeconds(5f), cancellationToken: _ctsOnDestroy);

            for (int i = 0; i < 2; i++)
            {
                var m = (MashNoteController)vfxProvider.Get(2).Rent();
                m.transform.position = transform.position;
                bc = INote.NowNoteNum;
                m.Initialize(vfxProvider, bc, 3.0f).Forget();

                await UniTask.Delay(TimeSpan.FromSeconds(5f), cancellationToken: _ctsOnDestroy);   
            }

            // なんとなく間隔開けた
            await UniTask.Delay(TimeSpan.FromSeconds(5f), cancellationToken: _ctsOnDestroy);
            
            var c = (CriticalNotesController)vfxProvider.Get(3).Rent();
            c.transform.position = transform.position;
            bc = INote.NowNoteNum;
            c.Initialize(vfxProvider, bc).Forget();

            // なんとなく間隔開けた
            await UniTask.Delay(TimeSpan.FromSeconds(10f), cancellationToken: _ctsOnDestroy);

            SceneManager.LoadScene("Scenes/MainScene");

            INote.NowNoteNum = 0;

        }
    }
}
