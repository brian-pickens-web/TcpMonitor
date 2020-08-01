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
        private readonly ListView _columnListView;
        private readonly OrderedDictionary _orderedRowDictionary = new OrderedDictionary();
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);

        private int _height;
        private int _width;

        public Size Size => new Size(_width, _orderedRowDictionary.Count);

        public string Name => _nameLabel.Text.ToString();

        public GridViewColumn(string name, GridView GridView)
        {
            _parent = GridView;
            _height = 3;
            _width = name.Length;
            Width = _width;
            Height = Dim.Fill();

            _nameLabel = new Label(name) { Y = Pos.Top(this) };
            _dividerLabel = new Label(string.Empty.PadLeft(name.Length, DividerChar)) { Y = Pos.Bottom(_nameLabel) };
            Add(_nameLabel);
            Add(_dividerLabel);

            _columnListView = new ListView(new DelegateDatasource(GetListViewSource)) { Y = Pos.Bottom(_dividerLabel), Height = Dim.Fill(), Width = Dim.Fill() };
            _columnListView.AllowsMarking = false;
            _columnListView.AllowsMultipleSelection = false;
            Add(_columnListView);
        }

        public async Task AddRow(string value, int rowIndex)
        {
            value = !string.IsNullOrEmpty(value) ? value : DividerChar.ToString();
            await _lock.WaitAsync();
            _orderedRowDictionary.Add(rowIndex, value);
            _lock.Release();
            await RecalculateSize();
        }
        
        public async Task RemoveRow(int index)
        {
            await _lock.WaitAsync();
            _orderedRowDictionary.Remove(index);
            _lock.Release();
            await RecalculateSize();
        }
        
        public async Task ClearAll()
        {
            await _lock.WaitAsync();
            _orderedRowDictionary.Clear();
            _lock.Release();
        }

        private async Task RecalculateSize()
        {
            var values = new List<string>(await GetListViewSourceAsync())
            {
                _dividerLabel.Text.ToString(),
                _nameLabel.Text.ToString()
            };

            var longestValue = values.Max();
            SetSize(longestValue.Length, _orderedRowDictionary.Count);
        }

        private void SetSize(int width, int height)
        {
            if (width != _width)
            {
                Width = width;
                _width = width;
                _dividerLabel.Text = string.Empty.PadLeft(width, DividerChar);
            }

            if (Math.Max(3, height) != _height)
            {
                Height = height;
                _height = height;
            }
        }

        private async Task<string[]> GetListViewSourceAsync()
        {
            await _lock.WaitAsync();
            var values = new string[_orderedRowDictionary.Count];
            _orderedRowDictionary.Values.CopyTo(values, 0);
            _lock.Release();
            return values;
        }

        private IList GetListViewSource()
        {
            return GetListViewSourceAsync().GetAwaiter().GetResult();
        }
    }
}
