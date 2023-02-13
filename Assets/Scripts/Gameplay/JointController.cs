using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class JointController : MonoBehaviour
{
    private HingeJoint _joint;
    
    private async void Start()
    {
        _joint = GetComponent<HingeJoint>();
    }

    public void Remove()
    {
        _joint.breakForce = 10;
    }
}
