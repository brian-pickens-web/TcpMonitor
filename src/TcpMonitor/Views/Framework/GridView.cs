using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terminal.Gui;

namespace TcpMonitor.Views.Framework
{
    public class GridView : View, IGridViewAsync, IGridView
    {
        private readonly List<GridViewColumn> _columns;
        private readonly ConcurrentDictionary<int, string[]> _rowIndex;
        private Func<MainLoop, bool> _refreshAction;
        
        public IProgress<object> RefreshItem { private get; set; }
        public IProgress<Size> RefreshSize { private get; set; }

        public GridView()
        {
            Height = Dim.Fill();
            Width = Dim.Fill();
            _columns = new List<GridViewColumn>();
            _rowIndex = new ConcurrentDictionary<int, string[]>();
        }

        public void SetSourceAsync<T>(Func<IAsyncEnumerable<T>> getStream)
        {
            TrySetColumnsFromType(typeof(T));
            _refreshAction = loop =>
            {
                loop.Invoke(async () => await RefreshAsync(getStream));
                return true;
            };
        }

        public void SetSourceAsync<T>(Func<Task<T>> getItemAsync)
        {
            TrySetColumnsFromType(typeof(T));
            _refreshAction = loop =>
            {
                loop.Invoke(async () => await RefreshAsync(getItemAsync));
                return true;
            };
        }

        public void SetSource<T>(T item)
        {
            SetSource(new[] { item }.AsEnumerable());
        }

        public void SetSource<T>(IEnumerable<T> enumerable)
        {
            TrySetColumnsFromType(typeof(T));
            var newRowList = new List<string[]>();
            foreach (var item in enumerable)
            {
                var row = InsertItem(item).GetAwaiter().GetResult();
                newRowList.Add(row);
            }
            RemoveExpiredRows(newRowList).GetAwaiter().GetResult();
        }

        public void StopRefresh()
        {
            Application.MainLoop.RemoveTimeout(_refreshAction);
        }

        public void StartRefresh(int refreshMilliseconds = 3000)
        {
            Application.MainLoop.Invoke(() => _refreshAction.Invoke(Application.MainLoop));
            Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(refreshMilliseconds), _refreshAction);
        }

        public async Task UpdateAsync()
        {
            await Task.Run(() => _refreshAction?.Invoke(Application.MainLoop));
        }

        private async Task RefreshAsync<T>(Func<IAsyncEnumerable<T>> getStream)
        {
            var newRowList = new List<string[]>();
            await foreach (var item in getStream())
            {
                var row = await InsertItem(item);
                newRowList.Add(row);
                RefreshItem?.Report(item);
                RefreshSize?.Report(GetGridSize());
            }
            await RemoveExpiredRows(newRowList);
        }

        private async Task RefreshAsync<T>(Func<Task<T>> getItemAsync)
        {
            var newRowList = new List<string[]>();
            var item = await getItemAsync();
            var row = await InsertItem(item);
            newRowList.Add(row);
            await RemoveExpiredRows(newRowList);
            RefreshItem?.Report(item);
            RefreshSize?.Report(GetGridSize());
        }

        private async Task RemoveExpiredRows(IEnumerable<string[]> newRowList)
        {
            var expiredRows = _rowIndex.Values.Except(newRowList);
            foreach (var expiredRow in expiredRows.ToArray())
                await RemoveRow(expiredRow);
        }

        private async Task<string[]> InsertItem<T>(T item)
        {
            var row = CreateRow(item);
            await AddRow(row);
            return row;
        }

        private async Task AddRow(params string[] row)
        {
            // Hash the row as the identifier
            var rowIndex = GetRowIndex(row);
            if (_rowIndex.ContainsKey(rowIndex))
                return;

            for (int i = 0; i < row.Length; i++)
            {
                var value = row[i];
                var column = _columns.ElementAtOrDefault(i);
                if (column == null)
                {
                    column = new GridViewColumn(string.Empty, this);
                    AddColumn(column);
                }
                await column.AddRow(value, rowIndex);
                column.SetNeedsDisplay();
            }

            _rowIndex.TryAdd(rowIndex, row);
        }

        private async Task RemoveRow(params string[] values)
        {
            var index = GetRowIndex(values);
            _rowIndex.TryRemove(index, out _);
            foreach (var column in _columns)
            {
                await column.RemoveRow(index);
            }
        }

        private void TrySetColumnsFromType(Type type)
        {
            if (_columns.Any()) return;
            var columnNames = type.GetProperties().Select(info => info.Name).ToArray();
            SetColumns(columnNames);
        }

        private void SetColumns(IEnumerable<string> columnNames)
        {
            foreach (var columnName in columnNames)
            {
                AddColumn(new GridViewColumn(columnName, this));
            }
        }

        private void AddColumn(GridViewColumn column)
        {
            column.X = _columns.Any() ? Pos.Right(_columns.Last()) + 1 : 0;
            _columns.Add(column);
            Add(column);
            column.SetNeedsDisplay();
        }

        private Size GetGridSize()
        {
            var width = _columns.Sum(column => column.Size.Width);
            return new Size(width, _rowIndex.Count);
        }

        private static int GetRowIndex(params string[] values)
        {
            return string.Join("-", values).GetHashCode();
        }

        private static string[] CreateRow<T>(T value)
        {
            var type = typeof(T);
            var columnNames = type.GetProperties().Select(info => info.Name).ToArray();
            var row = new string[columnNames.Length];
            for (int i = 0; i < columnNames.Length; i++)
            {
                row[i] = type.GetProperty(columnNames[i])?.GetValue(value).ToString();
            }
            return row;
        }
    }
}
