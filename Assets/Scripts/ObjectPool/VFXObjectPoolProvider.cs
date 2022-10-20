using System;
using System.Collections.Generic;
using Rhythm;
using UnityEngine;

namespace ObjectPool
{
    public class VFXObjectPoolProvider : MonoBehaviour
    {
        public List<VFXBase> Prefab => prefabs;

        [SerializeField] private List<VFXBase> prefabs;

        private VFXObjectPool[] _objectPools = new VFXObjectPool[100];

        public VFXObjectPool Get(int i)
        {
            //すでに準備済みならそちらを返す
            if (i < _objectPools.Length && _objectPools[i] != null) return _objectPools[i];

            //ObjectPoolを作成
            _objectPools[i] = new VFXObjectPool(prefabs[i]);

            return _objectPools[i];
        }
        
        private void OnDestroy()
        {
            //すべて破棄
            foreach (var op in _objectPools)
            {
                op?.Dispose();
            }
        }
    }
}