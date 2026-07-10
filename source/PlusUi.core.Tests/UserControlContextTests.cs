using PlusUi.core;
using PlusUi.core.Services.DebugBridge;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace PlusUi.core.Tests;

[TestClass]
public class UserControlContextTests
{
    private class TestViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private ChildViewModel _child = new();
        public ChildViewModel Child
        {
            get => _child;
            set
            {
                _child = value;
                OnPropertyChanged();
            }
        }

        private string _title = "initial";
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private class ChildViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _name = "child-initial";
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
    }

    private class TestCard(Expression<Func<string?>> titleBinding) : UserControl
    {
        public Label TitleLabel { get; } = new Label();

        protected override UiElement Build() =>
            new VStack(
                TitleLabel.BindText(titleBinding));
    }

    private class OwnVmCard : UserControl
    {
        public Label TitleLabel { get; } = new Label();
        private readonly ChildViewModel _viewModel;

        public OwnVmCard(ChildViewModel viewModel) : base(viewModel)
        {
            _viewModel = viewModel;
        }

        protected override UiElement Build() =>
            new VStack(
                TitleLabel.BindText(() => _viewModel.Name));
    }

    private static UiElement GetContent(UserControl control)
        => ((IDebugInspectable)control).GetDebugChildren().Single();

    [TestMethod]
    public void UserControl_ContextAssigned_PropagatesToContent()
    {
        // Arrange
        var viewModel = new TestViewModel();
        var card = new TestCard(() => viewModel.Title);
        card.BuildContent();

        // Act - simulates page context propagation reaching the UserControl
        card.Context = viewModel;

        // Assert - the composed content tree must receive the same context
        var content = GetContent(card);
        Assert.AreSame(viewModel, content.Context,
            "UserControl must forward its Context to the content built by Build()");
    }

    [TestMethod]
    public void UserControl_NestedPathBinding_OnContent_UpdatesWhenNestedPropertyChanges()
    {
        // Arrange - label inside the UserControl binds to a nested path (vm.Child.Name)
        var viewModel = new TestViewModel();
        var card = new TestCard(() => viewModel.Child.Name);
        card.BuildContent();
        card.Context = viewModel;

        // Act - nested object raises PropertyChanged (page VM itself raises nothing)
        viewModel.Child.Name = "updated";

        // Assert - same scenario works for a Checkbox bound directly (see CheckboxTests),
        // so it must also work when the bound element lives inside a UserControl
        Assert.AreEqual("updated", card.TitleLabel.Text,
            "Nested path binding inside a UserControl must update when the nested property changes");
    }

    [TestMethod]
    public void UserControl_NestedPathBinding_OnContent_UpdatesWhenIntermediateObjectIsSwapped()
    {
        // Arrange - same scenario as Checkbox_NestedBinding_SwapIntermediateObject_UpdatesBinding,
        // but the bound element lives inside a UserControl
        var viewModel = new TestViewModel();
        var card = new TestCard(() => viewModel.Child.Name);
        card.BuildContent();
        card.Context = viewModel;

        // Act - replace the intermediate object; afterwards the new child raises changes
        viewModel.Child = new ChildViewModel { Name = "swapped" };
        viewModel.Child.Name = "swapped-then-changed";

        // Assert
        Assert.AreEqual("swapped-then-changed", card.TitleLabel.Text,
            "Path binding must re-subscribe to the swapped intermediate object");
    }

    [TestMethod]
    public void UserControl_WithOwnViewModel_ContentContextIsOwnViewModel()
    {
        // Arrange
        var ownViewModel = new ChildViewModel();
        var pageViewModel = new TestViewModel();
        var card = new OwnVmCard(ownViewModel);
        card.BuildContent();

        // Act - page context propagation must not clobber the own view model
        card.Context = pageViewModel;

        // Assert
        Assert.AreSame(ownViewModel, GetContent(card).Context,
            "A UserControl constructed with its own view model must keep it as content context");
    }

    [TestMethod]
    public void UserControl_WithOwnViewModel_UpdatesBindings_WithoutPageForwarding()
    {
        // Arrange
        var ownViewModel = new ChildViewModel();
        var card = new OwnVmCard(ownViewModel);
        card.BuildContent();
        card.Context = new TestViewModel(); // page context arrives, own VM must win

        // Act - only the own view model raises PropertyChanged
        ownViewModel.Name = "updated";

        // Assert
        Assert.AreEqual("updated", card.TitleLabel.Text,
            "Bindings to the own view model must refresh without page-VM forwarding");
    }
}
