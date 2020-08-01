using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TcpMonitor.Services.TaskWrapper
{
    public sealed class ObserverAsyncEnumerableWrapper<T> : IAsyncEnumerable<T>, IObserver<T>, IDisposable
    {
        private readonly IDisposable _unsubscriber;
        private readonly SemaphoreSlim _enumerationSemaphore = new SemaphoreSlim(1);
        private readonly BufferBlock<T> _bufferBlock = new BufferBlock<T>();

        private bool _providerComplete = false;

        public Task Completion => _bufferBlock.Completion;

        public ObserverAsyncEnumerableWrapper(IObservable<T> provider)
        {
            _unsubscriber = provider.Subscribe(this);
        }

        public void OnNext(T value)
        {
            _bufferBlock.Post(value);
        }

        public void OnError(Exception error)
        {
            throw error;
        }

        public void OnCompleted()
        {
            _providerComplete = true;
        }

        public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken token = new CancellationToken())
        {
            // We lock this so we only ever enumerate once at a time.
            // That way we ensure all items are returned in a continuous
            // fashion with no 'holes' in the data when two foreach compete.
            await _enumerationSemaphore.WaitAsync();
            try
            {
                // Return new elements until cancellationToken is triggered.
                while (!_providerComplete || _bufferBlock.Count > 0)
                {
                    // Make sure to throw on cancellation so the Task will transfer into a canceled state
                    token.ThrowIfCancellationRequested();
                    // Log.Logger.Verbose("waiting on bufferBlock.Receive");
                    // var value = await _bufferBlock.ReceiveAsync();
                    // Log.Logger.Verbose($"yield return: {value}");
                    // yield return value;

                    _bufferBlock.TryReceive(null, out var item);
                    if (item == null) continue;
                    yield return item;
                }
            }
            finally
            {
                _bufferBlock.Complete();
                _enumerationSemaphore.Release();
            }
        }

        public void Dispose()
        {
            _bufferBlock.Complete();
            _unsubscriber?.Dispose();
            _enumerationSemaphore?.Dispose();
        }
    }
}
