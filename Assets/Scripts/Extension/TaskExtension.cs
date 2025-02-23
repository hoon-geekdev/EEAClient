using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace EEA.Extension
{
    public static class TaskExtensions
    {
        // Task를 IEnumerator로 변환
        public static IEnumerator AsCoroutine(this Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null; // Task가 완료될 때까지 대기
            }

            if (task.IsFaulted)
            {
                throw task.Exception ?? new System.Exception("Task failed with an unknown error.");
            }
        }

        // Task<T>를 IEnumerator로 변환 (결과 반환)
        public static IEnumerator AsCoroutine<T>(this Task<T> task, System.Action<T> callback)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsFaulted)
            {
                throw task.Exception ?? new System.Exception("Task failed with an unknown error.");
            }

            callback?.Invoke(task.Result);
        }

        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken token)
        {
            // 취소 토큰이 완료되거나 작업이 완료될 때까지 대기
            Task completedTask = await Task.WhenAny(task, Task.Delay(Timeout.Infinite, token));

            if (completedTask == task)
            {
                return await task; // 작업 완료 시 결과 반환
            }
            else
            {
                throw new OperationCanceledException(token); // 취소되면 예외 발생
            }
        }
    }
}
