using UnityEngine;

namespace System
{
    public class WallBreaker : MonoBehaviour
    {
        [SerializeField] private GameObject smokePrefab;
        
        [SerializeField] private GameObject leftWB;
        [SerializeField] private GameObject rightWB;

        [SerializeField] private AudioClip explodeSE;

        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("target"))
            {
                Instantiate(smokePrefab, other.transform.position, Quaternion.identity);
                gameObject.SetActive(false);
                leftWB.SetActive(false);
                rightWB.SetActive(false);
                
                _audioSource.PlayOneShot(explodeSE);
            }
        }
    }
}
