using Cysharp.Threading.Tasks;
using ObjectPool;
using UnityEngine;

namespace Rhythm
{
    public interface INote
    {
        public static int NowNoteNum = 0;
        
#pragma warning disable 1998
        UniTaskVoid Initialize(VFXObjectPoolProvider pool, int beatCount, float length = 1f);
#pragma warning restore 1998
    }
}
