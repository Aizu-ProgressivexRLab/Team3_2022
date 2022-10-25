using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

public class Target : MonoBehaviour
{
   [SerializeField] private GameObject smoke;
   [SerializeField] private GameObject trail;

   private Rigidbody _rb;

   private async void Start()
   {
      _rb = GetComponent<Rigidbody>();
      
      await GameManager.Instance.OnFinish.ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
      
      smoke.SetActive(true);
      trail.SetActive(true);

      await UniTask.Delay(TimeSpan.FromSeconds(3f), cancellationToken: this.GetCancellationTokenOnDestroy());
      
      smoke.SetActive(false);
      trail.SetActive(false);
   }
}
