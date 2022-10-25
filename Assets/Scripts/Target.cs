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

   private async void Start()
   {
      _rb = GetComponent<Rigidbody>();
      
      await GameManager.Instance.OnFinish.ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
      
      smoke.SendEvent("OnPlay");
      trail.SendEvent("OnPlay");
      Debug.Log("KOKO");

      await UniTask.Delay(TimeSpan.FromSeconds(3f), cancellationToken: this.GetCancellationTokenOnDestroy());
      
      smoke.SendEvent("OnStop");
      trail.gameObject.SetActive(false);
   }
}
