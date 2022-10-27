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

        await GameManager.Instance.OnFinish.ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());

        _joint.breakForce = 10;
    }
}
