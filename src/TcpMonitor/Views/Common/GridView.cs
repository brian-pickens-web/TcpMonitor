﻿using System;
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

        public void SetRefreshableDataSource<T>(Func<IEnumerable<T>> datasource, int refreshMilliseconds = 3000)
        {
            SetDataSource(datasource());
            Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(refreshMilliseconds), loop =>
            {
                return true;
            });
        }

        public void SetDataSource<T>(IEnumerable<T> datasource)
        {
            var type = typeof(T);
            var columnNames = type.GetProperties().Select(info => info.Name).ToArray();
            SetColumns(columnNames);
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

        public void AddRow(params string[] values)
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

        public override void RemoveAll()
        {
            _columns.Clear();
            base.RemoveAll();
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
