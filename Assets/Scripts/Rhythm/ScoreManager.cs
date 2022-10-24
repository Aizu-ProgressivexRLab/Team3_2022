using System;
using UnityEngine;

namespace Rhythm
{
    public class ScoreManager : MonoBehaviour
    {
        public int Score { get; set; }
        private int _score;

        public static ScoreManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("SceneManager").AddComponent<ScoreManager>();
                    DontDestroyOnLoad(_instance);
                }

                return _instance;
            }
        }
        
        private void Awake()
        {
            _instance = this;
        }


        private static ScoreManager _instance;
    }
}