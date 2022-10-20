using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class hit2 : MonoBehaviour
{
    // Start is called before the first frame update

    private Vector3 preposition;
    private float Kasokudo;
    
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
            collider.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.forward*Kasokudo*10000 + Vector3.up * Kasokudo * 100);
        }
    } 
    
}
