using System.Collections.Generic;

namespace TcpMonitor.Views.Framework
{
    public interface IGridView
    {
        void SetSource<T>(T item);
        void SetSource<T>(IEnumerable<T> enumerable);
    }
}
