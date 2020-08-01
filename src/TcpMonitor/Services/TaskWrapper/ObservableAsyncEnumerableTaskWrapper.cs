using System;
using System.Collections.Generic;

namespace TcpMonitor.Services.TaskWrapper
{
    public class ObservableAsyncEnumerableTaskWrapper<T>
    {
        public IAsyncEnumerable<T> AsyncEnumerable { get; }

        public ObservableAsyncEnumerableTaskWrapper(IObservable<T> observable)
        {
            AsyncEnumerable = new ObserverAsyncEnumerableWrapper<T>(observable);
        }
    }
}
