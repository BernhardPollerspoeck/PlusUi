using SkiaSharp;

namespace PlusUi.core;

/// <summary>
/// Overlay element that renders the combo box dropdown above all page content.
/// </summary>
internal class ComboBoxDropdownOverlay<T> : UiElement, IInputControl, IDismissableOverlay
{
    private readonly ComboBox<T> _comboBox;
    private int _hitItemIndex = -1;
    private bool _hitOnComboBox;

    public ComboBoxDropdownOverlay(ComboBox<T> comboBox)
    {
        _comboBox = comboBox;
    }

    public override void Render(SKCanvas canvas)
    {
        if (!_comboBox.IsOpen)
            return;

        _comboBox.RenderDropdown(canvas);
    }

    public override UiElement? HitTest(Point point)
    {
        if (!_comboBox.IsOpen || _comboBox._cachedItems.Count == 0)
            return null;

        var dropdownRect = _comboBox.GetDropdownRect();

        // Check if hit is in dropdown area
        if (point.X >= dropdownRect.Left && point.X <= dropdownRect.Right &&
            point.Y >= dropdownRect.Top && point.Y <= dropdownRect.Bottom)
        {
            // Store which item was hit for later use in InvokeCommand
            var relativeY = point.Y - dropdownRect.Top;
            _hitItemIndex = (int)(relativeY / ComboBox<T>.ItemHeight);

            // Update hover index for visual feedback
            _comboBox._hoveredIndex = _hitItemIndex;

            _hitOnComboBox = false;
            return this;
        }

        // Not in dropdown - clear hover
        _comboBox._hoveredIndex = -1;

        // Check if hit is on the combo box itself (so we don't dismiss when clicking the toggle button)
        if (point.X >= _comboBox.Position.X && point.X <= _comboBox.Position.X + _comboBox.ElementSize.Width &&
            point.Y >= _comboBox.Position.Y && point.Y <= _comboBox.Position.Y + _comboBox.ElementSize.Height)
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

        if (_hitItemIndex >= 0 && _hitItemIndex < _comboBox._cachedItems.Count)
        {
            _comboBox.SetSelectedIndex(_hitItemIndex);
            _comboBox.InvokeSetters();
        }

        Dismiss();
    }

    public void Dismiss()
    {
        _comboBox.SetIsOpen(false);
    }
}
