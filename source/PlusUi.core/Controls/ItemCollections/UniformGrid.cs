using PlusUi.core.Attributes;
using System.Linq.Expressions;

namespace PlusUi.core;

/// <summary>
/// A grid layout where all cells have equal size. Children are automatically
/// placed in cells from left to right, top to bottom.
/// </summary>
/// <remarks>
/// UniformGrid simplifies grid layouts when all cells should be the same size.
/// You can specify Rows, Columns, or both. If only one is specified, the other
/// is calculated automatically based on the number of children.
/// </remarks>
/// <example>
/// <code>
/// // Calculator-style layout with 4 columns
/// new UniformGrid()
///     .SetColumns(4)
///     .AddChildren(
///         new Button().SetText("7"), new Button().SetText("8"), new Button().SetText("9"), new Button().SetText("/"),
///         new Button().SetText("4"), new Button().SetText("5"), new Button().SetText("6"), new Button().SetText("*"),
///         new Button().SetText("1"), new Button().SetText("2"), new Button().SetText("3"), new Button().SetText("-"),
///         new Button().SetText("0"), new Button().SetText("."), new Button().SetText("="), new Button().SetText("+")
///     );
///
/// // Fixed 3x3 grid
/// new UniformGrid()
///     .SetRows(3)
///     .SetColumns(3)
///     .AddChildren(children);
/// </code>
/// </example>
[GenerateShadowMethods]
public partial class UniformGrid : UiLayoutElement
{
    /// <inheritdoc />
    protected internal override bool IsFocusable => false;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.Grid;

    public UniformGrid()
    {
    }

    public UniformGrid(params UiElement[] elements)
    {
        foreach (var element in elements)
        {
            element.Parent = this;
        }
        Children.AddRange(elements);
    }

    #region Rows
    internal int? Rows
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            InvalidateMeasure();
        }
    }

    public UniformGrid SetRows(int rows)
    {
        Rows = rows;
        return this;
    }

    public UniformGrid BindRows(Expression<Func<int?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Rows = getter());
        return this;
    }
    #endregion

    #region Columns
    internal int? Columns
    {
        get => field;
        set
        {
            if (field == value) return;
            field = value;
            InvalidateMeasure();
        }
    }

    public UniformGrid SetColumns(int columns)
    {
        Columns = columns;
        return this;
    }

    public UniformGrid BindColumns(Expression<Func<int?>> propertyExpression)
    {
        var path = ExpressionPathService.GetPropertyPath(propertyExpression);
        var getter = propertyExpression.Compile();
        RegisterPathBinding(path, () => Columns = getter());
        return this;
    }
    #endregion

    #region AddChildren helper
    public UniformGrid AddChildren(params UiElement[] elements)
    {
        foreach (var element in elements)
        {
            element.Parent = this;
            Children.Add(element);
        }
        InvalidateMeasure();
        return this;
    }
    #endregion

    #region Calculated grid dimensions
    private int _calculatedRows;
    private int _calculatedColumns;
    private float _cellWidth;
    private float _cellHeight;

    private void CalculateGridDimensions(int childCount)
    {
        if (childCount == 0)
        {
            _calculatedRows = 0;
            _calculatedColumns = 0;
            return;
        }

        if (Rows.HasValue && Columns.HasValue)
        {
            // Both specified - use as-is
            _calculatedRows = Rows.Value;
            _calculatedColumns = Columns.Value;
        }
        else if (Rows.HasValue)
        {
            // Only rows specified - calculate columns
            _calculatedRows = Rows.Value;
            _calculatedColumns = (int)Math.Ceiling((double)childCount / _calculatedRows);
        }
        else if (Columns.HasValue)
        {
            // Only columns specified - calculate rows
            _calculatedColumns = Columns.Value;
            _calculatedRows = (int)Math.Ceiling((double)childCount / _calculatedColumns);
        }
        else
        {
            // Neither specified - create a square-ish grid
            _calculatedColumns = (int)Math.Ceiling(Math.Sqrt(childCount));
            _calculatedRows = (int)Math.Ceiling((double)childCount / _calculatedColumns);
        }
    }
    #endregion

    #region Measure/Arrange
    public override Size MeasureInternal(Size availableSize, bool dontStretch = false)
    {
        var visibleChildren = Children.Where(c => c.IsVisible).ToList();
        CalculateGridDimensions(visibleChildren.Count);

        if (_calculatedRows == 0 || _calculatedColumns == 0)
        {
            return new Size(0, 0);
        }

        // Use DesiredSize if set, otherwise use availableSize
        var gridWidth = DesiredSize?.Width > 0 ? DesiredSize.Value.Width : availableSize.Width;
        var gridHeight = DesiredSize?.Height > 0 ? DesiredSize.Value.Height : availableSize.Height;

        // Calculate cell size based on grid size
        _cellWidth = gridWidth / _calculatedColumns;
        _cellHeight = gridHeight / _calculatedRows;

        // Measure all children with the uniform cell size
        // Don't subtract margin here - UiElement.Measure handles margin for Stretch alignment
        var cellSize = new Size(_cellWidth, _cellHeight);
        foreach (var child in visibleChildren)
        {
            child.Measure(cellSize, dontStretch);
        }

        // Return the total grid size
        return new Size(gridWidth, gridHeight);
    }

    protected override Point ArrangeInternal(Rect bounds)
    {
        var positionX = HorizontalAlignment switch
        {
            HorizontalAlignment.Center => bounds.Left + ((bounds.Width - ElementSize.Width) / 2),
            HorizontalAlignment.Right => bounds.Right - ElementSize.Width - Margin.Right,
            _ => bounds.Left + Margin.Left,
        };
        var positionY = VerticalAlignment switch
        {
            VerticalAlignment.Center => bounds.Top + ((bounds.Height - ElementSize.Height) / 2),
            VerticalAlignment.Bottom => bounds.Bottom - ElementSize.Height - Margin.Bottom,
            _ => bounds.Top + Margin.Top,
        };

        // Recalculate cell size based on actual ElementSize (may differ from Measure if constrained)
        var cellWidth = _calculatedColumns > 0 ? ElementSize.Width / _calculatedColumns : 0;
        var cellHeight = _calculatedRows > 0 ? ElementSize.Height / _calculatedRows : 0;

        var visibleChildren = Children.Where(c => c.IsVisible).ToList();
        var childIndex = 0;

        for (var row = 0; row < _calculatedRows && childIndex < visibleChildren.Count; row++)
        {
            for (var col = 0; col < _calculatedColumns && childIndex < visibleChildren.Count; col++)
            {
                var child = visibleChildren[childIndex];
                var cellX = positionX + (col * cellWidth);
                var cellY = positionY + (row * cellHeight);

                // Give child the full cell bounds - child handles its own alignment
                child.Arrange(new Rect(cellX, cellY, cellWidth, cellHeight));
                childIndex++;
            }
        }

        return new Point(positionX, positionY);
    }
    #endregion

    public override UiElement? HitTest(Point point)
    {
        foreach (var child in Children.ToList())
        {
            var result = child.HitTest(point);
            if (result is not null)
            {
                return result;
            }
        }
        return base.HitTest(point);
    }
}
