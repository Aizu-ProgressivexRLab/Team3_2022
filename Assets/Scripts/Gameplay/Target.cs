using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

public class Target : MonoBehaviour
{
   [SerializeField] private VisualEffect smoke;
   [SerializeField] private VisualEffect trail;

   private Rigidbody _rb;
   private Vector3 _initialPos;

   private async void Start()
   {
      _rb = GetComponent<Rigidbody>();
      _initialPos = transform.position;
      
      await GameManager.Instance.OnFinish.ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
      
      smoke.SendEvent("OnPlay");
      trail.SendEvent("OnPlay");

      await UniTask.Delay(TimeSpan.FromSeconds(3f), cancellationToken: this.GetCancellationTokenOnDestroy());
      
      smoke.SendEvent("OnStop");
      trail.gameObject.SetActive(false);
      
      GameManager.Instance.Distance = (int)((transform.position - _initialPos).magnitude);
   }
}
