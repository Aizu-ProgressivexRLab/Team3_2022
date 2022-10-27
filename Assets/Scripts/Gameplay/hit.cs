using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hit : MonoBehaviour
{
    private OVRInput.Controller Lhand;
    private float Kasokudo;
    // Start is called before the first frame update
    void Start()
    {
        Lhand = OVRInput.Controller.LTouch;
    }

    // Update is called once per frame
    void Update()
    {
        Kasokudo = OVRInput.GetLocalControllerAcceleration(Lhand).magnitude;
        Debug.Log(Kasokudo);
    }
    
    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("target"))
        {
            collider.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.forward*Kasokudo*10000);
        }
    } 
}
