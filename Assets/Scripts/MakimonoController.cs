using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Rhythm;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MakimonoController : MonoBehaviour
{
    private Reach_Text _reachText;
    private Score_Text _scoreText;

    private AudioSource _audioSource;
    private Animator _animator;

    [SerializeField] private AudioClip displaySound;
    [SerializeField] private AudioClip handClapSound;

    private async void Start()
    {
        _reachText = GetComponentInChildren<Reach_Text>();
        _scoreText = GetComponentInChildren<Score_Text>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();

        await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: this.GetCancellationTokenOnDestroy());

        _scoreText.score = GameManager.Instance.Distance;
        _reachText.reach = GameManager.Instance.ArriveRoomNum;
        _scoreText.WriteScore();
        _reachText.WriteReach();
        _audioSource.PlayOneShot(displaySound);

        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: this.GetCancellationTokenOnDestroy());

        _audioSource.PlayOneShot(handClapSound);
        _animator.SetBool("IsShrink", true);

        await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: this.GetCancellationTokenOnDestroy());

        await UniTask.WaitUntil(() => OVRInput.GetDown(OVRInput.Button.One),
            cancellationToken: this.GetCancellationTokenOnDestroy());

        INote.NowNoteNum = 0;
        SceneManager.LoadScene("Scenes/opening");

        GameManager.DeltaHeight = 0;
    }
}