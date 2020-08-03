using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Terminal.Gui;

namespace TcpMonitor.Views.Framework
{
    public sealed class GridViewColumn : View
    {
        private const char DividerChar = '-';

        private readonly Label _nameLabel;
        private readonly Label _dividerLabel;
        private readonly GridView _parent;
        private readonly OrderedDictionary _dataSource;
        private readonly OrderedDictionaryListWrapper _dataSourceList;
        private readonly SemaphoreSlim _lock;

        private int _height;
        private int _width;

        public Size Size => new Size(_width, _dataSource.Count);

        public string Name => _nameLabel.Text.ToString();

        public GridViewColumn(string name, GridView GridView)
        {
            _dataSource = new OrderedDictionary();
            _dataSourceList = new OrderedDictionaryListWrapper(_dataSource);
            _lock = new SemaphoreSlim(1);

            _parent = GridView;
            _height = 3;
            _width = name.Length;
            Width = _width;
            Height = Dim.Fill();

            _nameLabel = new Label(name) { Y = Pos.Top(this) };
            _dividerLabel = new Label(string.Empty.PadLeft(name.Length, DividerChar)) { Y = Pos.Bottom(_nameLabel) };
            Add(_nameLabel);
            Add(_dividerLabel);

            var columnListView = new ListView(_dataSourceList)
            {
                Y = Pos.Bottom(_dividerLabel),
                Height = Dim.Fill(),
                Width = Dim.Fill(),
                AllowsMarking = false,
                AllowsMultipleSelection = false
            };
            Add(columnListView);
        }

        public async Task AddRow(string value, int rowIndex)
        {
            value = !string.IsNullOrEmpty(value) ? value : DividerChar.ToString();
            await _lock.WaitAsync();
            _dataSource.Add(rowIndex, value);
            _lock.Release();
            RecalculateSize();
        }
        
        public async Task RemoveRow(int index)
        {
            await _lock.WaitAsync();
            _dataSource.Remove(index);
            _lock.Release();
            RecalculateSize();
        }
        
        public async Task ClearAll()
        {
            await _lock.WaitAsync();
            _dataSource.Clear();
            _lock.Release();
        }

        private void RecalculateSize()
        {
            var values = new List<string>(_dataSourceList.Cast<string>())
            {
                _dividerLabel.Text.ToString(),
                _nameLabel.Text.ToString()
            };

            var longestValue = values.Max();
            SetSize(longestValue.Length, _dataSource.Count);
        }

        private void SetSize(int width, int height)
        {
            if (width != _width)
            {
                Width = width;
                _width = width;
                _dividerLabel.Text = string.Empty.PadLeft(width, DividerChar);
            }

            if (Math.Max(3, height) == _height) return;

            Height = height;
            _height = height;
        }
    }
}
