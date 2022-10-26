using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Rhythm;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.VFX;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField] private string propertyName;
    [SerializeField] private float updateSpan = 0.5f;
    private int maxScore = 256;

    private VisualEffect _vfx;
    private MeshRenderer _mr;

    private void Start()
    {
        if (TryGetComponent(out _vfx))
        {
            this.FixedUpdateAsObservable()
                .ThrottleFirst(TimeSpan.FromSeconds(updateSpan))
                .Subscribe(_ =>
                {
                    _vfx.SetFloat(propertyName, ScoreManager.Instance.Score / (float)maxScore);
                }).AddTo(this);
        }
        else if (TryGetComponent(out _mr))
        {
            this.FixedUpdateAsObservable()
                .ThrottleFirst(TimeSpan.FromSeconds(updateSpan))
                .Subscribe(_ =>
                {
                    _mr.material.SetFloat(propertyName, ScoreManager.Instance.Score / (float)maxScore);
                }).AddTo(this);
        }
    }

    private void FixedUpdate()
    {
           
    }
}
