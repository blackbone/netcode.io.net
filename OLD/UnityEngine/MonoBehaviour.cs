using System.Collections;

namespace Netcode.io.OLD.UnityEngine
{
    public class MonoBehaviour : Component
    {
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            if (routine == null)
                throw new NullReferenceException("routine is null");
            if (this is null)
                throw new ArgumentException("Coroutines can only be started on a MonoBehaviour");
            return this.StartCoroutineManaged2(routine);
        }

        public void StopCoroutine(Coroutine routine)
        {
            if (routine == null)
                throw new NullReferenceException("routine is null");
            if (this is null)
                throw new ArgumentException("Coroutines can only be stopped on a MonoBehaviour");
            this.StopCoroutineManaged(routine);
        }

        private Coroutine StartCoroutineManaged2(IEnumerator enumerator) => throw new NotImplementedException();
        private Coroutine StopCoroutineManaged(Coroutine enumerator) => throw new NotImplementedException();
    }
    
    public class YieldInstruction : IEnumerable
    {
        public IEnumerator GetEnumerator() => Enumerable.Empty<object>().GetEnumerator();
    }
    
    public sealed class Coroutine : YieldInstruction
    {
        private Coroutine()
        {
        }
    }
}