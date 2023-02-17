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
    [SerializeField] private JointController _jointController;
    [SerializeField] private Rigidbody _targetRb;

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
        if (collider.CompareTag("last"))
        {
            var multiply = Mathf.Clamp(Kasokudo * 1000 + ScoreManager.Instance.Score * 10, 400, 2250f);
            Debug.Log($"A = {Kasokudo}, S = {ScoreManager.Instance.Score}, Multiply = {multiply}");
            
            _jointController.Remove();

            _targetRb.AddForce(Vector3.forward * multiply + Vector3.up * Mathf.Sqrt(multiply));
        }
    } 
    
}
