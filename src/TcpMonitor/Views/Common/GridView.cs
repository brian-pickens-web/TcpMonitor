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

        public void SetDataSource(Dictionary<string, IEnumerable<string>> datasource)
        {
            var pivotedDatasource = new Dictionary<int, IEnumerable<string>>();
            Enumerable.Range(0, datasource.Max(pair => pair.Value.Count()))
                .ToList()
                .ForEach(i => pivotedDatasource.Add(i, datasource.Values.Select(values => values.Count() > i ? values.ToArray()[i] : string.Empty)));

            SetColumns(datasource.Keys.ToArray());
            foreach (var row in pivotedDatasource)
            {
                AddRow(row.Value.ToArray());
            }
        }

        public void SetColumns(params string[] columnNames)
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
