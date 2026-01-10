using System.ComponentModel;
using System.Linq.Expressions;

namespace PlusUi.core.Tests;

[TestClass]
public class CheckboxTests
{
    [TestMethod]
    public void Checkbox_Toggle_UpdatesIsChecked()
    {
        // Arrange
        var checkbox = new Checkbox().SetIsChecked(false);

        // Act
        checkbox.Toggle();

        // Assert
        Assert.IsTrue(checkbox.IsChecked);
    }

    [TestMethod]
    public void Checkbox_Toggle_InvokesPropertySetter()
    {
        // Arrange
        bool backingField = false;
        var checkbox = new Checkbox()
            .BindIsChecked((() => backingField), v => backingField = v);

        // Act
        checkbox.Toggle();

        // Assert
        Assert.IsTrue(backingField, "Setter should have been invoked when checkbox was toggled");
    }

    [TestMethod]
    public void Checkbox_BindIsChecked_Expression_SetsInitialValue()
    {
        // Arrange
        bool backingField = true;

        // Act
        var checkbox = new Checkbox()
            .BindIsChecked((() => backingField), v => backingField = v);

        // Assert
        Assert.IsTrue(checkbox.IsChecked, "Checkbox should reflect initial value from binding");
    }

    [TestMethod]
    public void Checkbox_BindIsChecked_Expression_UpdatesFromBinding()
    {
        // Arrange
        bool backingField = false;
        var checkbox = new Checkbox()
            .BindIsChecked((() => backingField), v => backingField = v);

        // Act
        backingField = true;
        checkbox.UpdateBindings("backingField");

        // Assert
        Assert.IsTrue(checkbox.IsChecked, "Checkbox should update when binding source changes");
    }

    [TestMethod]
    public void Checkbox_TwoCheckboxesBoundToSameProperty_ToggleOne_UpdatesBackingField()
    {
        // Arrange - Two checkboxes bound to the same backing field
        bool sharedValue = false;

        var checkbox1 = new Checkbox()
            .BindIsChecked((() => sharedValue), v => sharedValue = v);

        var checkbox2 = new Checkbox()
            .BindIsChecked((() => sharedValue), v => sharedValue = v);

        // Act - Toggle checkbox1
        checkbox1.Toggle();

        // Assert - sharedValue should be updated
        Assert.IsTrue(sharedValue, "Backing field should be updated when checkbox1 is toggled");
        Assert.IsTrue(checkbox1.IsChecked, "Checkbox1 should be checked after toggle");
    }

    [TestMethod]
    public void Checkbox_TwoCheckboxesBoundToSameProperty_ToggleOne_OtherUpdatesViaBinding()
    {
        // Arrange - Two checkboxes bound to the same backing field
        bool sharedValue = false;

        var checkbox1 = new Checkbox()
            .BindIsChecked((() => sharedValue), v => sharedValue = v);

        var checkbox2 = new Checkbox()
            .BindIsChecked((() => sharedValue), v => sharedValue = v);

        // Act - Toggle checkbox1, which updates sharedValue
        checkbox1.Toggle();

        // Simulate PropertyChanged notification reaching checkbox2
        checkbox2.UpdateBindings("sharedValue");

        // Assert - Both checkboxes should be synchronized
        Assert.IsTrue(checkbox1.IsChecked, "Checkbox1 should be checked");
        Assert.IsTrue(checkbox2.IsChecked, "Checkbox2 should be synchronized with checkbox1");
    }

    [TestMethod]
    public void Checkbox_TwoCheckboxesBoundToSameProperty_MultipleToggles_StaySynchronized()
    {
        // Arrange
        bool sharedValue = false;

        var checkbox1 = new Checkbox()
            .BindIsChecked((() => sharedValue), v => sharedValue = v);

        var checkbox2 = new Checkbox()
            .BindIsChecked((() => sharedValue), v => sharedValue = v);

        // Act & Assert - Toggle checkbox1 multiple times
        checkbox1.Toggle();
        checkbox2.UpdateBindings("sharedValue");
        Assert.IsTrue(checkbox1.IsChecked);
        Assert.IsTrue(checkbox2.IsChecked);

        checkbox1.Toggle();
        checkbox2.UpdateBindings("sharedValue");
        Assert.IsFalse(checkbox1.IsChecked);
        Assert.IsFalse(checkbox2.IsChecked);

        // Toggle checkbox2 this time
        checkbox2.Toggle();
        checkbox1.UpdateBindings("sharedValue");
        Assert.IsTrue(checkbox1.IsChecked);
        Assert.IsTrue(checkbox2.IsChecked);
    }

    [TestMethod]
    public void Checkbox_BindIsChecked_WithViewModel_ToggleSynchronizesViaPropertyChanged()
    {
        // Arrange - Using a ViewModel with INotifyPropertyChanged
        var viewModel = new CheckboxTestViewModel();

        var checkbox1 = new Checkbox()
            .BindIsChecked((() => viewModel.IsChecked), v => viewModel.IsChecked = v);
        checkbox1.Context = viewModel;

        var checkbox2 = new Checkbox()
            .BindIsChecked((() => viewModel.IsChecked), v => viewModel.IsChecked = v);
        checkbox2.Context = viewModel;

        // Act - Toggle checkbox1
        checkbox1.Toggle();

        // Assert - ViewModel should be updated
        Assert.IsTrue(viewModel.IsChecked, "ViewModel should be updated");

        // The PropertyChanged event should have triggered checkbox2 to update
        // (via PathBindingTracker subscribing to ViewModel)
        Assert.IsTrue(checkbox2.IsChecked, "Checkbox2 should be synchronized via PropertyChanged");
    }

    [TestMethod]
    public void Checkbox_NestedBinding_1Level_ToggleSynchronizes()
    {
        // Arrange - ViewModel with 1-level nested property: vm.Level1.Checked
        var viewModel = new NestedViewModel();

        var checkbox1 = new Checkbox()
            .BindIsChecked(() => viewModel.Level1.Checked, v => viewModel.Level1.Checked = v);
        checkbox1.Context = viewModel;

        var checkbox2 = new Checkbox()
            .BindIsChecked(() => viewModel.Level1.Checked, v => viewModel.Level1.Checked = v);
        checkbox2.Context = viewModel;

        // Act - Toggle checkbox1
        checkbox1.Toggle();

        // Assert
        Assert.IsTrue(viewModel.Level1.Checked, "Nested property should be updated");
        Assert.IsTrue(checkbox1.IsChecked, "Checkbox1 should be checked");
        Assert.IsTrue(checkbox2.IsChecked, "Checkbox2 should be synchronized via PropertyChanged");
    }

    [TestMethod]
    public void Checkbox_NestedBinding_1Level_ContextSetAfterBinding_Works()
    {
        // This simulates what happens in a Page - binding is created before Context is set
        var viewModel = new NestedViewModel();

        // Create checkboxes with bindings BEFORE setting Context
        var checkbox1 = new Checkbox()
            .BindIsChecked(() => viewModel.Level1.Checked, v => viewModel.Level1.Checked = v);

        var checkbox2 = new Checkbox()
            .BindIsChecked(() => viewModel.Level1.Checked, v => viewModel.Level1.Checked = v);

        // Context is set AFTER binding (simulating Page behavior)
        checkbox1.Context = viewModel;
        checkbox2.Context = viewModel;

        // Act - Toggle checkbox1
        checkbox1.Toggle();

        // Assert
        Assert.IsTrue(viewModel.Level1.Checked, "Nested property should be updated");
        Assert.IsTrue(checkbox1.IsChecked, "Checkbox1 should be checked");
        Assert.IsTrue(checkbox2.IsChecked, "Checkbox2 should be synchronized via PropertyChanged");
    }

    [TestMethod]
    public void Checkbox_NestedBinding_1Level_InitialValue_ReflectedAfterContextSet()
    {
        // Test that setting initial value BEFORE Context still works
        var viewModel = new NestedViewModel();
        viewModel.Level1.Checked = true; // Set initial value

        var checkbox = new Checkbox()
            .BindIsChecked(() => viewModel.Level1.Checked, v => viewModel.Level1.Checked = v);

        // Checkbox should NOT reflect value yet (Context not set)
        // Actually, the getter is compiled and called immediately in BindIsChecked

        // Set Context after binding
        checkbox.Context = viewModel;

        // Assert - Checkbox should now reflect the initial value
        Assert.IsTrue(checkbox.IsChecked, "Checkbox should reflect initial value after Context is set");
    }

    [TestMethod]
    public void Checkbox_NestedBinding_ViaHelperMethod_Works()
    {
        // This simulates the BindingDemoPage's CreateSection pattern
        var viewModel = new NestedViewModel();

        // Create checkboxes via helper that takes Expression as parameter (like CreateSection)
        var (checkbox1, checkbox2) = CreateCheckboxPair(
            () => viewModel.Level1.Checked,
            v => viewModel.Level1.Checked = v);

        // Set context (simulating page context propagation)
        checkbox1.Context = viewModel;
        checkbox2.Context = viewModel;

        // Act - Toggle checkbox1
        checkbox1.Toggle();

        // Assert
        Assert.IsTrue(viewModel.Level1.Checked, "Nested property should be updated");
        Assert.IsTrue(checkbox1.IsChecked, "Checkbox1 should be checked");
        Assert.IsTrue(checkbox2.IsChecked, "Checkbox2 should be synchronized");
    }

    private static (Checkbox, Checkbox) CreateCheckboxPair(
        System.Linq.Expressions.Expression<Func<bool>> bindingExpression,
        Action<bool> setter)
    {
        var checkbox1 = new Checkbox().BindIsChecked(bindingExpression, setter);
        var checkbox2 = new Checkbox().BindIsChecked(bindingExpression, setter);
        return (checkbox1, checkbox2);
    }

    [TestMethod]
    public void Checkbox_NestedBinding_2Level_ToggleSynchronizes()
    {
        // Arrange - ViewModel with 2-level nested property: vm.Level1.Level2.DeepChecked
        var viewModel = new NestedViewModel();

        var checkbox1 = new Checkbox()
            .BindIsChecked(() => viewModel.Level1.Level2.DeepChecked, v => viewModel.Level1.Level2.DeepChecked = v);
        checkbox1.Context = viewModel;

        var checkbox2 = new Checkbox()
            .BindIsChecked(() => viewModel.Level1.Level2.DeepChecked, v => viewModel.Level1.Level2.DeepChecked = v);
        checkbox2.Context = viewModel;

        // Act - Toggle checkbox1
        checkbox1.Toggle();

        // Assert
        Assert.IsTrue(viewModel.Level1.Level2.DeepChecked, "Deep nested property should be updated");
        Assert.IsTrue(checkbox1.IsChecked, "Checkbox1 should be checked");
        Assert.IsTrue(checkbox2.IsChecked, "Checkbox2 should be synchronized via PropertyChanged");
    }

    [TestMethod]
    public void Checkbox_NestedBinding_SwapIntermediateObject_UpdatesBinding()
    {
        // Arrange - Test that when Level1 is replaced, bindings update
        var viewModel = new NestedViewModel();
        viewModel.Level1.Checked = false;

        var checkbox = new Checkbox()
            .BindIsChecked(() => viewModel.Level1.Checked, v => viewModel.Level1.Checked = v);
        checkbox.Context = viewModel;

        Assert.IsFalse(checkbox.IsChecked, "Initial value should be false");

        // Act - Replace Level1 with a new object that has Checked = true
        viewModel.Level1 = new Level1ViewModel { Checked = true };

        // Assert - Checkbox should update to reflect new Level1's value
        Assert.IsTrue(checkbox.IsChecked, "Checkbox should update when intermediate object is swapped");
    }

    private class CheckboxTestViewModel : INotifyPropertyChanged
    {
        private bool _isChecked;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    private class NestedViewModel : INotifyPropertyChanged
    {
        private Level1ViewModel _level1 = new();

        public Level1ViewModel Level1
        {
            get => _level1;
            set
            {
                if (_level1 != value)
                {
                    _level1 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Level1)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    private class Level1ViewModel : INotifyPropertyChanged
    {
        private bool _checked;
        private Level2ViewModel _level2 = new();

        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Checked)));
                }
            }
        }

        public Level2ViewModel Level2
        {
            get => _level2;
            set
            {
                if (_level2 != value)
                {
                    _level2 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Level2)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    private class Level2ViewModel : INotifyPropertyChanged
    {
        private bool _deepChecked;

        public bool DeepChecked
        {
            get => _deepChecked;
            set
            {
                if (_deepChecked != value)
                {
                    _deepChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeepChecked)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
