using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Terminal.Gui;

namespace TcpMonitor.Views.Framework
{
    public interface IGridViewAsync
    {
        IProgress<object> RefreshItem { set; }
        IProgress<Size> RefreshSize { set; }
        void SetSourceAsync<T>(Func<IAsyncEnumerable<T>> getStream);
        void SetSourceAsync<T>(Func<Task<T>> getItemAsync);
        Task UpdateAsync();
        void StopRefresh();
        void StartRefresh(int refreshMilliseconds = 3000);
    }
}
