using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Overlay element that renders the combo box dropdown above all page content.
/// </summary>
internal class ComboBoxDropdownOverlay<T>(ComboBox<T> comboBox) : UiElement, IInputControl, IDismissableOverlay
{
    /// <inheritdoc />
    protected internal override bool IsFocusable => false;

    /// <inheritdoc />
    public override AccessibilityRole AccessibilityRole => AccessibilityRole.List;

    private int _hitItemIndex = -1;
    private bool _hitOnComboBox;

    public override void Render(SKCanvas canvas)
    {
        if (!comboBox.IsOpen)
            return;

        comboBox.RenderDropdown(canvas);
    }

    public override UiElement? HitTest(Point point)
    {
        if (!comboBox.IsOpen || comboBox._cachedItems.Count == 0)
            return null;

        var dropdownRect = comboBox.GetDropdownRect();

        // Check if hit is in dropdown area
        if (point.X >= dropdownRect.Left && point.X <= dropdownRect.Right &&
            point.Y >= dropdownRect.Top && point.Y <= dropdownRect.Bottom)
        {
            // Store which item was hit for later use in InvokeCommand
            var relativeY = point.Y - dropdownRect.Top;
            var displayIndex = (int)(relativeY / ComboBox<T>.ItemHeight);
            _hitItemIndex = comboBox._scrollStartIndex + displayIndex;

            // Update hover index for visual feedback
            comboBox._hoveredIndex = _hitItemIndex;

            _hitOnComboBox = false;
            return this;
        }

        // Not in dropdown - clear hover
        comboBox._hoveredIndex = -1;

        // Check if hit is on the combo box itself (so we don't dismiss when clicking the toggle button)
        if (point.X >= comboBox.Position.X && point.X <= comboBox.Position.X + comboBox.ElementSize.Width &&
            point.Y >= comboBox.Position.Y && point.Y <= comboBox.Position.Y + comboBox.ElementSize.Height)
        {
            _hitItemIndex = -1;
            _hitOnComboBox = true;
            return this;
        }

        _hitItemIndex = -1;
        _hitOnComboBox = false;
        return null;
    }

    public void InvokeCommand()
    {
        if (_hitOnComboBox)
        {
            // Clicked on combo box button - close dropdown
            Dismiss();
            return;
        }

        if (_hitItemIndex >= 0 && _hitItemIndex < comboBox._cachedItems.Count)
        {
            comboBox.SetSelectedIndex(_hitItemIndex);
            comboBox.InvokeSetters();
        }

        Dismiss();
    }

    public void Dismiss()
    {
        comboBox.SetIsOpen(false);
    }
}
