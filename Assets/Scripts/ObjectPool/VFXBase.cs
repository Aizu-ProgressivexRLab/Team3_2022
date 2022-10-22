using Rhythm;
using UnityEngine;

namespace ObjectPool
{
    public class VFXBase : MonoBehaviour
    {
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private int id;
        public int Id => id;
    }
}
