namespace PlusUi.core.Controls.GridHelper;

/// <summary>
/// Internal helper class representing a child element's position in a Grid layout.
/// </summary>
internal class GridItem(UiElement child, int row, int column, int rowSpan, int columnSpan)
{
    public UiElement Element { get; } = child;
    public int Row { get; } = row;
    public int Column { get; } = column;
    public int RowSpan { get; } = rowSpan;
    public int ColumnSpan { get; } = columnSpan;
}
