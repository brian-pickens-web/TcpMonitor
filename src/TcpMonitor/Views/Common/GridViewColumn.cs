using Terminal.Gui;

namespace TcpMonitor.Views.Common
{
    public sealed class GridViewColumn : View
    {
        private const char DividerChar = '-';
        private int _width;
        private readonly Label _divider;

        public GridViewColumn(string name)
        {
            _width = name.Length;
            Height = 10;
            Width = _width;
            _divider = new Label(string.Empty.PadLeft(name.Length, DividerChar))
            {
                Width = Dim.Fill(),
                Y = Pos.Y(this) + 1
            };
            Add(new Label(name) { Y = Pos.Y(this) });
            Add(_divider);
        }

        public void AddValue(string value)
        {
            value = !string.IsNullOrEmpty(value) ? value : DividerChar.ToString();
            if (value.Length > _width)
            {
                Width = value.Length;
                _width = value.Length;
                _divider.Text = string.Empty.PadLeft(value.Length, DividerChar);
            }

            var yPos = Pos.Y(this) + this.Subviews.Count;
            var xPos = value == DividerChar.ToString() ? Pos.Center() : 0;
            Add(new Label(value) { Y = yPos, X = xPos });
        }
    }
}
