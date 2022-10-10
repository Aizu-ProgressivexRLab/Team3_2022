using Rhythm;
using UnityEngine;
using UnityEngine.VFX;
using UniRx.Toolkit;

namespace ObjectPool
{
    public class VFXObjectPool : ObjectPool<NotesController>
    {
        //ItemのPrefab
        private readonly NotesController _prefab;
    
        private readonly VFXObjectPoolProvider _provider;
    
        //ヒエラルキウィンドウ上で親となるTransform
        private readonly Transform _root;

        public VFXObjectPool(NotesController prefab)
        {
            _prefab = prefab;

            //親になるObject
            _root = new GameObject().transform;
            _root.name = $"{prefab.name}s";
            _root.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        protected override NotesController CreateInstance()
        {
            //インスタンスが新しく必要になったらInstantiate
            var newItem = GameObject.Instantiate(_prefab);

            //親となるTransformを変更する
            newItem.transform.SetParent(_root);

            return newItem;
        }
    }
}
