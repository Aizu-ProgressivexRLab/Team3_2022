using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hit : MonoBehaviour
{
    private OVRInput.Controller Lhand;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Lhand = OVRInput.Controller.LTouch;
        var Lefta = OVRInput.GetLocalControllerAcceleration(Lhand);
        // if (Lefta != new Vector3(0, 0, 0))
        // {
            Debug.Log(Lefta);
        //}
    }
}
