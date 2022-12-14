using System.Collections;
using System.Collections.Generic;
using Rhythm;
using Unity.VisualScripting;
using UnityEngine;

public class hit2 : MonoBehaviour
{
    // Start is called before the first frame update
    
    public float Acceleration => Kasokudo;  // 公開用のプロパティ
    
    private Vector3 preposition;
    private float Kasokudo;
    [SerializeField] private float strength = 10;
    
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       Kasokudo =  (transform.position - preposition).magnitude;
       preposition = transform.position;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("target"))
        {
            var multiply = Mathf.Clamp(Kasokudo * ScoreManager.Instance.Score * strength, 100f, 2000f);
            
            collider.transform.parent.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.forward * multiply + Vector3.up * Mathf.Sqrt(multiply));
        }
    } 
    
}
