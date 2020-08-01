using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TcpMonitor
{
    public static class TestBufferBlock
    {
        private static BufferBlock<string> _bufferBlock;
        private static string[] _values = new[] {"foo", "bar", "foobar", "barfoo", "brian", "pickens"};
        private static bool _complete = false;

        public static async Task Main()
        {
            _bufferBlock = new BufferBlock<string>();
            await Task.WhenAll(Consumer(), Producer());
        }

        public static async Task Producer()
        {
            var tasks = new List<Task>();
            foreach (var value in _values)
            {
                tasks.Add(Task.Run(() =>
                {
                    Thread.Sleep(10);
                    _bufferBlock.Post(value);
                }));
            }
            _complete = true;
            await Task.WhenAll(tasks);
        }

        public static async Task Consumer()
        {
            while (!_complete || _bufferBlock.Count > 0)
            {
                var value = await _bufferBlock.ReceiveAsync();
                Console.WriteLine(value);
            }
        }

    }
}
