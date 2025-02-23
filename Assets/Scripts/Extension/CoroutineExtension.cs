using System.Collections;

namespace EEA.Extension
{
    public static class CoroutineExtensions
    {
        public static IEnumerator WhenAll(params IEnumerator[] coroutines)
        {
            foreach (var coroutine in coroutines)
            {
                yield return coroutine;
            }
        }
    }
}
