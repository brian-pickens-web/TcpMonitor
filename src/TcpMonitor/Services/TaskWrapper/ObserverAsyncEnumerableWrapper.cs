using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using PooledAwait;

namespace TcpMonitor.Services.TaskWrapper
{
    public sealed class ObserverAsyncEnumerableWrapper<T> : IAsyncEnumerable<T>, IAsyncEnumerator<T>, IObserver<T>, IDisposable
    {
        private readonly IDisposable _unsubscriber;
        private readonly SemaphoreSlim _enumerationSemaphore = new SemaphoreSlim(1);
        private readonly Channel<T> _channel = Channel.CreateUnbounded<T>();

        private CancellationToken _token = default;
        private bool _providerComplete = default;

        public ObserverAsyncEnumerableWrapper(IObservable<T> provider)
        {
            _unsubscriber = provider.Subscribe(this);
        }

        public async void OnNext(T value)
        {
            await _channel.Writer.WriteAsync(value);
        }

        public void OnError(Exception error)
        {
            _channel.Writer.Complete(error);
        }

        public void OnCompleted()
        {
            _providerComplete = true;
        }

        public async IAsyncEnumerator<T> GetAsyncEnumerator([EnumeratorCancellation] CancellationToken token = new CancellationToken())
        {
            // Set the cancellation token
            _token = token;

            // We lock this so we only ever enumerate once at a time.
            // That way we ensure all items are returned in a continuous
            // fashion with no 'holes' in the data when two foreach compete.await Task.Yield();
            await _enumerationSemaphore.WaitAsync(token);
            try
            {
                while (await MoveNextAsync())
                {
                    // Make sure to throw on cancellation so the Task will transfer into a canceled state
                    _token.ThrowIfCancellationRequested();

                    yield return Current;
                }
            }
            finally
            {
                _channel.Writer.TryComplete();
                _enumerationSemaphore.Release();
            }
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return Impl();
            async PooledValueTask<bool> Impl()
            {
                Current = await _channel.Reader.ReadAsync(_token);
                return await _channel.Reader.WaitToReadAsync(_token);
            }
        }

        public T Current { get; private set; }

        public async ValueTask DisposeAsync()
        {
            await Task.Yield();
            Dispose();
        }

        public void Dispose()
        {
            _unsubscriber?.Dispose();
            _enumerationSemaphore?.Dispose();
            _channel.Writer.TryComplete();
        }
    }
}
