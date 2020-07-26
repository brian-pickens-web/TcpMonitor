using System;
using System.Collections.Generic;
using System.Linq;
using Terminal.Gui;

namespace TcpMonitor.Views.Common
{
    public class GridView : View
    {
        private readonly List<GridViewColumn> _columns;

        public GridView()
        {
            Width = Dim.Fill();
            Height = Dim.Fill();
            _columns = new List<GridViewColumn>();
        }

        public void SetRefreshableDataSource<T>(Func<T> getData, int refreshMilliseconds = 3000)
        {
            var wrapper = new Func<IEnumerable<T>>(() => new [] { getData() }.AsEnumerable());
            SetRefreshableDataSource(wrapper, refreshMilliseconds);
        }

        public void SetRefreshableDataSource<T>(Func<IEnumerable<T>> getData, int refreshMilliseconds = 3000)
        {
            SetDataSource(getData());
            Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(refreshMilliseconds), loop =>
            {
                ClearRows();
                SetRowsFromDatasource(getData());
                return true;
            });
        }

        public void SetDataSource<T>(T datasource)
        {
            SetDataSource(new [] { datasource }.AsEnumerable());
        }

        public void SetDataSource<T>(IEnumerable<T> datasource)
        {
            var type = typeof(T);
            var columnNames = type.GetProperties().Select(info => info.Name).ToArray();
            SetColumns(columnNames);
            SetRowsFromDatasource(datasource);
        }

        public void SetColumns(params string[] columnNames)
        {
            SetColumns(columnNames.AsEnumerable());
        }

        public void SetColumns(IEnumerable<string> columnNames)
        {
            this.RemoveAll();
            foreach (var columnName in columnNames)
            {
                AddColumn(new GridViewColumn(columnName));
            }
        }

        public void ClearRows()
        {
            foreach (var column in _columns)
            {
                column.ClearValues();
            }
        }

        private void SetRowsFromDatasource<T>(IEnumerable<T> datasource)
        {
            var type = typeof(T);
            var columnNames = type.GetProperties().Select(info => info.Name).ToArray();
            foreach (var item in datasource)
            {
                var row = new string[columnNames.Length];
                for (int i = 0; i < columnNames.Length; i++)
                {
                    row[i] = type.GetProperty(columnNames[i])?.GetValue(item).ToString();
                }
                AddRow(row);
            }
        }

        private void AddRow(params string[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                var value = values[i];
                var column = _columns.ElementAtOrDefault(i);
                if (column == null)
                {
                    column = new GridViewColumn(string.Empty);
                    AddColumn(column);
                }
                column.AddValue(value);
            }
        }

        private void AddColumn(GridViewColumn column)
        {
            var xPos = _columns.Any()
                ? Pos.Right(_columns.Last()) + 1
                : (this.SuperView != null) ? Pos.Right(this.SuperView) : 0;
            column.X = xPos;
            _columns.Add(column);
            Add(column);
        }
    }
}
