using System;
using System.Threading;
using System.Threading.Tasks;

namespace TcpMonitor.Services.TaskWrapper
{
    public sealed class ObservableResultTaskWrapper<T>
    {
        public Task<T> Task { get; }

        public ObservableResultTaskWrapper(IObservable<T> observable, CancellationToken token = new CancellationToken())
        {
            Task = new ObserverResultTaskWrapper<T>(observable, token).Task;
        }
    }
}
