using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;

namespace TcpMonitor.Services.TaskWrapper
{
public sealed class ObserverAsyncEnumerableWrapper<T> : IAsyncEnumerable<T>, IObserver<T>, IDisposable
{
    private readonly IDisposable _unsubscriber;
    private readonly Channel<T> _channel = Channel.CreateUnbounded<T>();

    private bool _producerComplete;

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
        _producerComplete = true;
    }

    public async IAsyncEnumerator<T> GetAsyncEnumerator([EnumeratorCancellation] CancellationToken token = new CancellationToken())
    {
        token.ThrowIfCancellationRequested();

        while (await _channel.Reader.WaitToReadAsync(token) || !_producerComplete)
        {
            token.ThrowIfCancellationRequested();
            while (_channel.Reader.TryRead(out var item))
            {
                token.ThrowIfCancellationRequested();
                yield return item;
            }
        }
        _channel.Writer.Complete();
    }

    public void Dispose()
    {
        _channel.Writer.Complete();
        _unsubscriber?.Dispose();
    }
}
}
