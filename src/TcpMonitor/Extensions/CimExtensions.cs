using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Management.Infrastructure.Generic;
using TcpMonitor.Services.TaskWrapper;

namespace TcpMonitor.Extensions
{
    public static class CimExtensions
    {
        public static Task<T> AsTask<T>(this CimAsyncResult<T> observable)
        {
            return new ObservableResultTaskWrapper<T>(observable).Task;
        }

        public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(this CimAsyncMultipleResults<T> observable, [EnumeratorCancellation] CancellationToken token = new CancellationToken())
        {
            var enumerable  = new ObservableAsyncEnumerableTaskWrapper<T>(observable).AsyncEnumerable;
            await using var e = enumerable.GetAsyncEnumerator(token);
            while (await e.MoveNextAsync())
            {
                yield return e.Current;
            }
        }
    }
}
