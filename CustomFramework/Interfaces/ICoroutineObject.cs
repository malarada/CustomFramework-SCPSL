using MEC;
using System.Collections.Generic;

namespace CustomFramework.Interfaces
{
    public interface ICoroutineObject
    {
        CoroutineHandle coroutine { get; set; }
        IEnumerator<float> Coroutine();
    }
}
