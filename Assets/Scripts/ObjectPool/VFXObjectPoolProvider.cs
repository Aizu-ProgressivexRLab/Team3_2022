using Rhythm;
using UnityEngine;

namespace ObjectPool
{
    public class VFXObjectPoolProvider : MonoBehaviour
    {
        public NotesController Prefab => prefab;

        [SerializeField] private NotesController prefab;

        private VFXObjectPool _objectPool;


        public VFXObjectPool Get()
        {
            //すでに準備済みならそちらを返す
            if (_objectPool != null) return _objectPool;

            //ObjectPoolを作成
            _objectPool = new VFXObjectPool(prefab);

            return _objectPool;
        }
        
        private void OnDestroy()
        {
            //すべて破棄
            _objectPool.Dispose();
        }
    }
}