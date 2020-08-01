using System;
using System.Threading;
using System.Threading.Tasks;

namespace TcpMonitor.Services.TaskWrapper
{
    public sealed class ObserverResultTaskWrapper<T> : TaskCompletionSource<T>, IObserver<T>, IDisposable
    {
        private readonly IDisposable _unsubscriber;
        private readonly CancellationToken _token;
        private T _result;

        public ObserverResultTaskWrapper(IObservable<T> provider, CancellationToken token = default)
        {
            _unsubscriber = provider.Subscribe(this);
            _token = token;
        }

        public void OnNext(T value)
        {
            if (_token.IsCancellationRequested)
            {
                SetCanceled();
            }
            _result = value;
        }

        public void OnError(Exception error)
        {
            SetException(error);
        }

        public void OnCompleted()
        {
            SetResult(_result);
        }

        public void Dispose()
        {
            _unsubscriber?.Dispose();
            SetCanceled();
        }
    }
}
