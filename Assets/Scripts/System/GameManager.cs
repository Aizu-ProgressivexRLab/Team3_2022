using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject LeftHand => leftHand;
    public GameObject RightHand => rightHand;
    public Subject<Unit> OnFinish = new Subject<Unit>();

    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;

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

        OnFinish.AddTo(this);
    }

    private static GameManager _instance;
}
