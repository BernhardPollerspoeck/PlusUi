using PlusUi.core;

namespace PlusUi.core.Tests;

[TestClass]
public class UniformGridTests
{
    [TestMethod]
    public void UniformGrid_3x3_CellsShouldHaveEqualSize()
    {
        // Arrange
        var grid = new UniformGrid()
            .SetRows(3)
            .SetColumns(3)
            .SetDesiredSize(new Size(300, 300));

        // Add 9 simple borders as children
        for (int i = 0; i < 9; i++)
        {
            var cell = new Border()
                .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                .SetVerticalAlignment(VerticalAlignment.Stretch);
            cell.Parent = grid;
            grid.Children.Add(cell);
        }

        // Act
        grid.Measure(new Size(300, 300));
        grid.Arrange(new Rect(0, 0, 300, 300));

        // Assert - all cells should be 100x100
        var expectedCellSize = 100f;
        foreach (var child in grid.Children)
        {
            Assert.AreEqual(expectedCellSize, child.ElementSize.Width, 0.1f,
                $"Cell width should be {expectedCellSize}");
            Assert.AreEqual(expectedCellSize, child.ElementSize.Height, 0.1f,
                $"Cell height should be {expectedCellSize}");
        }
    }

    [TestMethod]
    public void UniformGrid_3x3_CellsShouldBePositionedCorrectly()
    {
        // Arrange
        var grid = new UniformGrid()
            .SetRows(3)
            .SetColumns(3)
            .SetDesiredSize(new Size(300, 300));

        var cells = new List<Border>();
        for (int i = 0; i < 9; i++)
        {
            var cell = new Border()
                .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                .SetVerticalAlignment(VerticalAlignment.Stretch);
            cell.Parent = grid;
            grid.Children.Add(cell);
            cells.Add(cell);
        }

        // Act
        grid.Measure(new Size(300, 300));
        grid.Arrange(new Rect(0, 0, 300, 300));

        // Assert - cells should be at correct positions
        // Row 0: (0,0), (100,0), (200,0)
        // Row 1: (0,100), (100,100), (200,100)
        // Row 2: (0,200), (100,200), (200,200)
        var expectedPositions = new Point[]
        {
            new(0, 0), new(100, 0), new(200, 0),
            new(0, 100), new(100, 100), new(200, 100),
            new(0, 200), new(100, 200), new(200, 200)
        };

        for (int i = 0; i < 9; i++)
        {
            Assert.AreEqual(expectedPositions[i].X, cells[i].Position.X, 0.1f,
                $"Cell {i} X position should be {expectedPositions[i].X}, was {cells[i].Position.X}");
            Assert.AreEqual(expectedPositions[i].Y, cells[i].Position.Y, 0.1f,
                $"Cell {i} Y position should be {expectedPositions[i].Y}, was {cells[i].Position.Y}");
        }
    }

    [TestMethod]
    public void UniformGrid_WithTapGestureDetector_CellsShouldHaveCorrectSizeAndPosition()
    {
        // Arrange - This mimics the Tic-Tac-Toe demo structure
        var grid = new UniformGrid()
            .SetRows(3)
            .SetColumns(3)
            .SetDesiredSize(new Size(300, 300));

        var tapDetectors = new List<TapGestureDetector>();
        var borders = new List<Border>();

        for (int i = 0; i < 9; i++)
        {
            var border = new Border()
                .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                .SetVerticalAlignment(VerticalAlignment.Stretch);

            var tap = new TapGestureDetector(border)
                .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                .SetVerticalAlignment(VerticalAlignment.Stretch)
                .SetMargin(new Margin(3));

            tap.Parent = grid;
            grid.Children.Add(tap);
            tapDetectors.Add(tap);
            borders.Add(border);
        }

        // Act
        grid.Measure(new Size(300, 300));
        grid.Arrange(new Rect(0, 0, 300, 300));

        // Assert
        var cellSize = 100f;
        var margin = 3f;
        var expectedTapSize = cellSize - (margin * 2); // 94

        // Check TapGestureDetector sizes - should all be equal
        for (int i = 0; i < 9; i++)
        {
            Assert.AreEqual(expectedTapSize, tapDetectors[i].ElementSize.Width, 0.1f,
                $"TapGestureDetector {i} width should be {expectedTapSize}, was {tapDetectors[i].ElementSize.Width}");
            Assert.AreEqual(expectedTapSize, tapDetectors[i].ElementSize.Height, 0.1f,
                $"TapGestureDetector {i} height should be {expectedTapSize}, was {tapDetectors[i].ElementSize.Height}");
        }

        // Check Border sizes - should all be equal to TapGestureDetector (no additional margin)
        for (int i = 0; i < 9; i++)
        {
            Assert.AreEqual(expectedTapSize, borders[i].ElementSize.Width, 0.1f,
                $"Border {i} width should be {expectedTapSize}, was {borders[i].ElementSize.Width}");
            Assert.AreEqual(expectedTapSize, borders[i].ElementSize.Height, 0.1f,
                $"Border {i} height should be {expectedTapSize}, was {borders[i].ElementSize.Height}");
        }

        // Check positions - TapGestureDetectors should be offset by margin within each cell
        var expectedTapPositions = new Point[]
        {
            new(3, 3), new(103, 3), new(203, 3),
            new(3, 103), new(103, 103), new(203, 103),
            new(3, 203), new(103, 203), new(203, 203)
        };

        for (int i = 0; i < 9; i++)
        {
            Assert.AreEqual(expectedTapPositions[i].X, tapDetectors[i].Position.X, 0.1f,
                $"TapGestureDetector {i} X should be {expectedTapPositions[i].X}, was {tapDetectors[i].Position.X}");
            Assert.AreEqual(expectedTapPositions[i].Y, tapDetectors[i].Position.Y, 0.1f,
                $"TapGestureDetector {i} Y should be {expectedTapPositions[i].Y}, was {tapDetectors[i].Position.Y}");
        }

        // Borders should be at same position as their TapGestureDetector parent
        for (int i = 0; i < 9; i++)
        {
            Assert.AreEqual(tapDetectors[i].Position.X, borders[i].Position.X, 0.1f,
                $"Border {i} X should match TapGestureDetector");
            Assert.AreEqual(tapDetectors[i].Position.Y, borders[i].Position.Y, 0.1f,
                $"Border {i} Y should match TapGestureDetector");
        }
    }

    [TestMethod]
    public void UniformGrid_InsideBorder_CellsShouldHaveCorrectSize()
    {
        // Arrange - This mimics the exact demo structure
        var outerBorder = new Border()
            .SetStrokeThickness(0)
            .SetHorizontalAlignment(HorizontalAlignment.Center);

        var grid = new UniformGrid()
            .SetRows(3)
            .SetColumns(3)
            .SetDesiredSize(new Size(300, 300));

        grid.Parent = outerBorder;
        outerBorder.Children.Add(grid);

        var tapDetectors = new List<TapGestureDetector>();
        for (int i = 0; i < 9; i++)
        {
            var border = new Border()
                .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                .SetVerticalAlignment(VerticalAlignment.Stretch);

            var tap = new TapGestureDetector(border)
                .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                .SetVerticalAlignment(VerticalAlignment.Stretch)
                .SetMargin(new Margin(3));

            tap.Parent = grid;
            grid.Children.Add(tap);
            tapDetectors.Add(tap);
        }

        // Act
        outerBorder.Measure(new Size(400, 400));
        outerBorder.Arrange(new Rect(0, 0, 400, 400));

        // Assert - UniformGrid should be 300x300
        Assert.AreEqual(300f, grid.ElementSize.Width, 0.1f, "Grid width");
        Assert.AreEqual(300f, grid.ElementSize.Height, 0.1f, "Grid height");

        // All TapGestureDetectors should be 94x94 (100 - 6 margin)
        var expectedSize = 94f;
        for (int i = 0; i < 9; i++)
        {
            Assert.AreEqual(expectedSize, tapDetectors[i].ElementSize.Width, 0.1f,
                $"TapGestureDetector {i} width should be {expectedSize}, was {tapDetectors[i].ElementSize.Width}");
            Assert.AreEqual(expectedSize, tapDetectors[i].ElementSize.Height, 0.1f,
                $"TapGestureDetector {i} height should be {expectedSize}, was {tapDetectors[i].ElementSize.Height}");
        }
    }

    [TestMethod]
    public void UniformGrid_BorderWithImage_BorderShouldStretchToFillCell()
    {
        // Arrange - This is the EXACT structure from UniformGridDemoPage.cs
        // The visual problem: Border is smaller than TapGestureDetector when Image has DesiredSize
        var grid = new UniformGrid()
            .SetRows(3)
            .SetColumns(3)
            .SetDesiredSize(new Size(300, 300));

        var tapDetectors = new List<TapGestureDetector>();
        var borders = new List<Border>();
        var images = new List<Image>();

        for (int i = 0; i < 9; i++)
        {
            // This is exactly how CreateCell works in the demo
            var image = new Image()
                .SetAspect(Aspect.AspectFit)
                .SetDesiredSize(new Size(60, 60))  // <-- This might cause Border to shrink!
                .SetHorizontalAlignment(HorizontalAlignment.Center)
                .SetVerticalAlignment(VerticalAlignment.Center)
                .SetMargin(new Margin(0));

            var border = new Border()
                .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                .SetVerticalAlignment(VerticalAlignment.Stretch)
                .SetStrokeThickness(0);

            border.AddChild(image);

            var tap = new TapGestureDetector(border)
                .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                .SetVerticalAlignment(VerticalAlignment.Stretch)
                .SetMargin(new Margin(3));

            tap.Parent = grid;
            grid.Children.Add(tap);
            tapDetectors.Add(tap);
            borders.Add(border);
            images.Add(image);
        }

        // Act
        grid.Measure(new Size(300, 300));
        grid.Arrange(new Rect(0, 0, 300, 300));

        // Assert
        var cellSize = 100f;
        var margin = 3f;
        var expectedTapSize = cellSize - (margin * 2); // 94

        // TapGestureDetectors should be 94x94
        for (int i = 0; i < 9; i++)
        {
            Assert.AreEqual(expectedTapSize, tapDetectors[i].ElementSize.Width, 0.1f,
                $"TapGestureDetector {i} width should be {expectedTapSize}, was {tapDetectors[i].ElementSize.Width}");
            Assert.AreEqual(expectedTapSize, tapDetectors[i].ElementSize.Height, 0.1f,
                $"TapGestureDetector {i} height should be {expectedTapSize}, was {tapDetectors[i].ElementSize.Height}");
        }

        // CRITICAL: Border should ALSO be 94x94 (same as TapGestureDetector)
        // This is the bug: Border might be 60x60 (Image size) instead of stretching!
        for (int i = 0; i < 9; i++)
        {
            Assert.AreEqual(expectedTapSize, borders[i].ElementSize.Width, 0.1f,
                $"Border {i} width should be {expectedTapSize} (stretched), was {borders[i].ElementSize.Width}");
            Assert.AreEqual(expectedTapSize, borders[i].ElementSize.Height, 0.1f,
                $"Border {i} height should be {expectedTapSize} (stretched), was {borders[i].ElementSize.Height}");
        }

        // Images should be 60x60 (their DesiredSize)
        for (int i = 0; i < 9; i++)
        {
            Assert.AreEqual(60f, images[i].ElementSize.Width, 0.1f,
                $"Image {i} width should be 60, was {images[i].ElementSize.Width}");
            Assert.AreEqual(60f, images[i].ElementSize.Height, 0.1f,
                $"Image {i} height should be 60, was {images[i].ElementSize.Height}");
        }

        // Border should be at same position as TapGestureDetector
        for (int i = 0; i < 9; i++)
        {
            Assert.AreEqual(tapDetectors[i].Position.X, borders[i].Position.X, 0.1f,
                $"Border {i} X should match TapGestureDetector position");
            Assert.AreEqual(tapDetectors[i].Position.Y, borders[i].Position.Y, 0.1f,
                $"Border {i} Y should match TapGestureDetector position");
        }
    }

    [TestMethod]
    public void UniformGrid_OuterBorderShouldMatchGridSize()
    {
        // Arrange - This tests the OUTER Border that wraps the UniformGrid
        // The visual issue: "der background ist nicht so groß wie der content des uniform grid"
        var outerBorder = new Border()
            .SetStrokeThickness(0)
            .SetHorizontalAlignment(HorizontalAlignment.Center);

        var grid = new UniformGrid()
            .SetRows(3)
            .SetColumns(3)
            .SetDesiredSize(new Size(300, 300));

        // Use AddChild method like in the demo
        outerBorder.AddChild(grid);

        // Add cells like in demo
        var borders = new List<Border>();
        for (int i = 0; i < 9; i++)
        {
            var image = new Image()
                .SetDesiredSize(new Size(60, 60))
                .SetHorizontalAlignment(HorizontalAlignment.Center)
                .SetVerticalAlignment(VerticalAlignment.Center)
                .SetMargin(new Margin(0));

            var cellBorder = new Border()
                .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                .SetVerticalAlignment(VerticalAlignment.Stretch)
                .SetStrokeThickness(0);
            cellBorder.AddChild(image);

            var tap = new TapGestureDetector(cellBorder)
                .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                .SetVerticalAlignment(VerticalAlignment.Stretch)
                .SetMargin(new Margin(3));

            tap.Parent = grid;
            grid.Children.Add(tap);
            borders.Add(cellBorder);
        }

        // Act - simulate layout from a parent view (like VStack)
        outerBorder.Measure(new Size(500, 600));
        outerBorder.Arrange(new Rect(0, 0, 500, 600));

        // Assert - OuterBorder should be EXACTLY 300x300 (same as grid)
        Assert.AreEqual(300f, outerBorder.ElementSize.Width, 0.1f,
            $"OuterBorder width should be 300 (match grid), was {outerBorder.ElementSize.Width}");
        Assert.AreEqual(300f, outerBorder.ElementSize.Height, 0.1f,
            $"OuterBorder height should be 300 (match grid), was {outerBorder.ElementSize.Height}");

        // Grid should be 300x300
        Assert.AreEqual(300f, grid.ElementSize.Width, 0.1f,
            $"Grid width should be 300, was {grid.ElementSize.Width}");
        Assert.AreEqual(300f, grid.ElementSize.Height, 0.1f,
            $"Grid height should be 300, was {grid.ElementSize.Height}");

        // Grid and OuterBorder should be at same position
        Assert.AreEqual(outerBorder.Position.X, grid.Position.X, 0.1f,
            $"Grid X should match OuterBorder X");
        Assert.AreEqual(outerBorder.Position.Y, grid.Position.Y, 0.1f,
            $"Grid Y should match OuterBorder Y");
    }

    [TestMethod]
    public void UniformGrid_CellSpacing_ShouldBeUniform()
    {
        // Arrange
        var grid = new UniformGrid()
            .SetRows(3)
            .SetColumns(3)
            .SetDesiredSize(new Size(300, 300));

        var borders = new List<Border>();
        for (int i = 0; i < 9; i++)
        {
            var tap = new TapGestureDetector(
                new Border()
                    .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                    .SetVerticalAlignment(VerticalAlignment.Stretch)
            )
            .SetHorizontalAlignment(HorizontalAlignment.Stretch)
            .SetVerticalAlignment(VerticalAlignment.Stretch)
            .SetMargin(new Margin(3));

            tap.Parent = grid;
            grid.Children.Add(tap);
        }

        // Act
        grid.Measure(new Size(300, 300));
        grid.Arrange(new Rect(0, 0, 300, 300));

        // Assert - Gap between cells should be uniform (6px = 3px margin on each side)
        // Cell 0 right edge + gap = Cell 1 left edge
        var cell0 = grid.Children[0];
        var cell1 = grid.Children[1];
        var cell3 = grid.Children[3];

        var gap01 = cell1.Position.X - (cell0.Position.X + cell0.ElementSize.Width);
        var gap03 = cell3.Position.Y - (cell0.Position.Y + cell0.ElementSize.Height);

        Assert.AreEqual(6f, gap01, 0.1f, $"Horizontal gap should be 6px (3+3 margin), was {gap01}");
        Assert.AreEqual(6f, gap03, 0.1f, $"Vertical gap should be 6px (3+3 margin), was {gap03}");

        // All horizontal gaps should be the same
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 2; col++)
            {
                var leftCell = grid.Children[row * 3 + col];
                var rightCell = grid.Children[row * 3 + col + 1];
                var gap = rightCell.Position.X - (leftCell.Position.X + leftCell.ElementSize.Width);
                Assert.AreEqual(6f, gap, 0.1f,
                    $"Gap between cell ({row},{col}) and ({row},{col+1}) should be 6px, was {gap}");
            }
        }
    }

    [TestMethod]
    public void UniformGrid_InVStack_BorderShouldMatchGridSize()
    {
        // Arrange - This simulates the EXACT demo page structure inside a VStack
        // Problem: "der background ist nicht so groß wie der content des uniform grid"
        var vstack = new VStack();

        // Header placeholder
        var header = new Label().SetText("Header");
        header.Parent = vstack;
        vstack.Children.Add(header);

        // The outer Border wrapping the UniformGrid - THIS IS THE "BACKGROUND"
        var outerBorder = new Border()
            .SetStrokeThickness(0)
            .SetCornerRadius(12)
            .SetHorizontalAlignment(HorizontalAlignment.Center);

        var grid = new UniformGrid()
            .SetRows(3)
            .SetColumns(3)
            .SetDesiredSize(new Size(300, 300));

        outerBorder.AddChild(grid);
        outerBorder.Parent = vstack;
        vstack.Children.Add(outerBorder);

        // Add cells with Image (exactly like demo)
        var cellBorders = new List<Border>();
        for (int i = 0; i < 9; i++)
        {
            var image = new Image()
                .SetDesiredSize(new Size(60, 60))
                .SetHorizontalAlignment(HorizontalAlignment.Center)
                .SetVerticalAlignment(VerticalAlignment.Center)
                .SetMargin(new Margin(0));

            var cellBorder = new Border()
                .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                .SetVerticalAlignment(VerticalAlignment.Stretch)
                .SetStrokeThickness(0)
                .SetCornerRadius(8);
            cellBorder.AddChild(image);

            var tap = new TapGestureDetector(cellBorder)
                .SetHorizontalAlignment(HorizontalAlignment.Stretch)
                .SetVerticalAlignment(VerticalAlignment.Stretch)
                .SetMargin(new Margin(3));

            tap.Parent = grid;
            grid.Children.Add(tap);
            cellBorders.Add(cellBorder);
        }

        // Act - Layout the VStack like a real page would
        vstack.Measure(new Size(500, 800));
        vstack.Arrange(new Rect(0, 0, 500, 800));

        // Assert
        // Grid should be 300x300
        Assert.AreEqual(300f, grid.ElementSize.Width, 0.1f,
            $"Grid width should be 300, was {grid.ElementSize.Width}");
        Assert.AreEqual(300f, grid.ElementSize.Height, 0.1f,
            $"Grid height should be 300, was {grid.ElementSize.Height}");

        // CRITICAL: OuterBorder (the "background") should be EXACTLY 300x300
        Assert.AreEqual(300f, outerBorder.ElementSize.Width, 0.1f,
            $"OuterBorder (background) width should be 300, was {outerBorder.ElementSize.Width}");
        Assert.AreEqual(300f, outerBorder.ElementSize.Height, 0.1f,
            $"OuterBorder (background) height should be 300, was {outerBorder.ElementSize.Height}");

        // All cell borders should be 94x94
        var expectedCellSize = 94f;
        for (int i = 0; i < 9; i++)
        {
            Assert.AreEqual(expectedCellSize, cellBorders[i].ElementSize.Width, 0.1f,
                $"CellBorder {i} width should be {expectedCellSize}, was {cellBorders[i].ElementSize.Width}");
            Assert.AreEqual(expectedCellSize, cellBorders[i].ElementSize.Height, 0.1f,
                $"CellBorder {i} height should be {expectedCellSize}, was {cellBorders[i].ElementSize.Height}");
        }
    }
}
