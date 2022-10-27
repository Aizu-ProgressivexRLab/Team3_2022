using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger)>0.4||OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger)>0.4)
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
