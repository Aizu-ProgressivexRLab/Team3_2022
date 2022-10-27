using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class ShockwaveController : MonoBehaviour
{
    [SerializeField] private float speed = 0.05f;
    [SerializeField] private Camera camera;
    [SerializeField] private Transform center;

    private MeshRenderer _meshRenderer;
    private float _nowProgress = 0f;
    
    private async void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        
        await GameManager.Instance.OnFinish.ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());

        this.FixedUpdateAsObservable()
            .Where(_ => _nowProgress <= 1)
            .Subscribe(_ =>
            {
                _meshRenderer.material.SetFloat("_Progress", _nowProgress);
                _nowProgress += speed;

                _meshRenderer.material.SetVector("_CenterPosition", camera.WorldToScreenPoint(center.position));
            }).AddTo(this);
    }
}
