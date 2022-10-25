using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject LeftHand => leftHand;
    public GameObject RightHand => rightHand;
    public IObservable<Unit> OnFinish
    {
        get => _onFinishSubject;
        set
        {
            _onFinishSubject = (Subject<Unit>)value;
        }
    }

    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;

    private Subject<Unit> _onFinishSubject;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("GameManager").AddComponent<GameManager>();
                DontDestroyOnLoad(_instance);
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    private static GameManager _instance;
}
