using Rhythm;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace System
{
    public class GameManager : MonoBehaviour
    {
        public GameObject LeftHand => leftHand;
        public GameObject RightHand => rightHand;
        public Subject<Unit> OnFinish = new Subject<Unit>();

        [SerializeField] private GameObject leftHand;
        [SerializeField] private GameObject rightHand;
        
        public int Distance { get; set; }
        public int ArriveRoomNum { get; set; } = 1;

        public static float DeltaHeight = 0;

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("GameManager").AddComponent<GameManager>();
                }

                return _instance;
            }
        }

        private void Awake()
        {
            _instance = this;
            OVRManager.tiledMultiResLevel = OVRManager.TiledMultiResLevel.LMSHighTop;

            ScoreManager.Instance.Score = 0;

            OnFinish.AddTo(this);
        }

        private static GameManager _instance;
    }
}
