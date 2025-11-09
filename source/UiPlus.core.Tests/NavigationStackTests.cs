using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlusUi.core;
using System.ComponentModel;

namespace UiPlus.core.Tests;

[TestClass]
public class NavigationStackTests
{
    // Test ViewModels
    private class TestViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
    }

    // Test Pages
    private class TestPageA(TestViewModel vm) : UiPageElement(vm)
    {
        public bool AppearingCalled { get; private set; }
        public bool DisappearingCalled { get; private set; }
        public bool OnNavigatedToCalled { get; private set; }
        public bool OnNavigatedFromCalled { get; private set; }
        public object? ReceivedParameter { get; private set; }

        protected override UiElement Build() => new Label();

        public override void Appearing()
        {
            AppearingCalled = true;
            base.Appearing();
        }

        public override void Disappearing()
        {
            DisappearingCalled = true;
            base.Disappearing();
        }

        public override void OnNavigatedTo(object? parameter)
        {
            OnNavigatedToCalled = true;
            ReceivedParameter = parameter;
            base.OnNavigatedTo(parameter);
        }

        public override void OnNavigatedFrom()
        {
            OnNavigatedFromCalled = true;
            base.OnNavigatedFrom();
        }
    }

    private class TestPageB(TestViewModel vm) : UiPageElement(vm)
    {
        public bool AppearingCalled { get; private set; }
        public bool OnNavigatedToCalled { get; private set; }
        public object? ReceivedParameter { get; private set; }

        protected override UiElement Build() => new Label();

        public override void Appearing()
        {
            AppearingCalled = true;
            base.Appearing();
        }

        public override void OnNavigatedTo(object? parameter)
        {
            OnNavigatedToCalled = true;
            ReceivedParameter = parameter;
            base.OnNavigatedTo(parameter);
        }
    }

    private class TestPageC(TestViewModel vm) : UiPageElement(vm)
    {
        protected override UiElement Build() => new Label();
    }

    private static ServiceProvider CreateServiceProvider(PlusUiConfiguration? config = null)
    {
        var services = new ServiceCollection();
        services.AddSingleton<TestViewModel>();
        services.AddSingleton<TestPageA>();
        services.AddSingleton<TestPageB>();
        services.AddSingleton<TestPageC>();

        config ??= new PlusUiConfiguration();

        // Register NavigationContainer
        services.AddSingleton(new NavigationContainer(config));

        return services.BuildServiceProvider();
    }

    #region NavigationContainer Tests

    [TestMethod]
    public void TestNavigationContainer_DefaultConfiguration_StackDisabled()
    {
        // Arrange
        var config = new PlusUiConfiguration();
        var vm = new TestViewModel();
        var rootPage = new TestPageA(vm);

        // Act
        var container = new NavigationContainer(config);
        container.Push(rootPage);

        // Assert
        Assert.IsFalse(container.IsStackEnabled);
        Assert.AreEqual(1, container.StackDepth);
        Assert.IsFalse(container.CanGoBack);
    }

    [TestMethod]
    public void TestNavigationContainer_StackEnabled_CorrectConfiguration()
    {
        // Arrange
        var config = new PlusUiConfiguration { EnableNavigationStack = true };
        var vm = new TestViewModel();
        var rootPage = new TestPageA(vm);

        // Act
        var container = new NavigationContainer(config);
        container.Push(rootPage);

        // Assert
        Assert.IsTrue(container.IsStackEnabled);
        Assert.AreEqual(1, container.StackDepth);
        Assert.IsFalse(container.CanGoBack);
    }

    [TestMethod]
    public void TestNavigationContainer_Push_IncreasesStackDepth()
    {
        // Arrange
        var config = new PlusUiConfiguration { EnableNavigationStack = true };
        var vm = new TestViewModel();
        var rootPage = new TestPageA(vm);
        var container = new NavigationContainer(config);
        container.Push(rootPage);
        var newPage = new TestPageB(vm);

        // Act
        container.Push(newPage);

        // Assert
        Assert.AreEqual(2, container.StackDepth);
        Assert.IsTrue(container.CanGoBack);
        Assert.AreEqual(newPage, container.CurrentPage);
    }

    [TestMethod]
    public void TestNavigationContainer_Push_WithParameter_StoresParameter()
    {
        // Arrange
        var config = new PlusUiConfiguration { EnableNavigationStack = true };
        var vm = new TestViewModel();
        var rootPage = new TestPageA(vm);
        var container = new NavigationContainer(config);
        container.Push(rootPage);
        var newPage = new TestPageB(vm);
        var parameter = new { Id = 42, Name = "Test" };

        // Act
        container.Push(newPage, parameter);

        // Assert
        Assert.AreEqual(parameter, container.CurrentParameter);
    }

    [TestMethod]
    public void TestNavigationContainer_Pop_DecreasesStackDepth()
    {
        // Arrange
        var config = new PlusUiConfiguration { EnableNavigationStack = true };
        var vm = new TestViewModel();
        var rootPage = new TestPageA(vm);
        var container = new NavigationContainer(config);
        container.Push(rootPage);
        var newPage = new TestPageB(vm);
        container.Push(newPage);

        // Act
        container.Pop();

        // Assert
        Assert.AreEqual(1, container.StackDepth);
        Assert.IsFalse(container.CanGoBack);
        Assert.AreEqual(rootPage, container.CurrentPage);
    }

    [TestMethod]
    public void TestNavigationContainer_Pop_AtRoot_ThrowsException()
    {
        // Arrange
        var config = new PlusUiConfiguration { EnableNavigationStack = true };
        var vm = new TestViewModel();
        var rootPage = new TestPageA(vm);
        var container = new NavigationContainer(config);

        // Act
        Assert.ThrowsExactly<InvalidOperationException>(container.Pop);

        // Assert - Exception expected
    }

    [TestMethod]
    public void TestNavigationContainer_PopToRoot_ClearsStack()
    {
        // Arrange
        var config = new PlusUiConfiguration { EnableNavigationStack = true };
        var vm = new TestViewModel();
        var rootPage = new TestPageA(vm);
        var container = new NavigationContainer(config);
        container.Push(rootPage);
        var pageB = new TestPageB(vm);
        var pageC = new TestPageC(vm);
        container.Push(pageB);
        container.Push(pageC);

        // Act
        container.PopToRoot();

        // Assert
        Assert.AreEqual(1, container.StackDepth);
        Assert.IsFalse(container.CanGoBack);
        Assert.AreEqual(rootPage, container.CurrentPage);
    }

    [TestMethod]
    public void TestNavigationContainer_MaxStackDepth_ThrowsException()
    {
        // Arrange
        var config = new PlusUiConfiguration { EnableNavigationStack = true, MaxStackDepth = 3 };
        var vm = new TestViewModel();
        var rootPage = new TestPageA(vm);
        var container = new NavigationContainer(config);

        // Act - Push 4 pages (total 4 exceeds max of 3)
        container.Push(new TestPageB(vm));
        container.Push(new TestPageC(vm));
        container.Push(new TestPageC(vm));
        Assert.ThrowsExactly<InvalidOperationException>(() => container.Push(new TestPageA(vm))); // This should throw

        // Assert - Exception expected
    }

    [TestMethod]
    public void TestNavigationContainer_StackDisabled_ReplacesPage()
    {
        // Arrange
        var config = new PlusUiConfiguration { EnableNavigationStack = false };
        var vm = new TestViewModel();
        var rootPage = new TestPageA(vm);
        var container = new NavigationContainer(config);
        container.Push(rootPage);
        var newPage = new TestPageB(vm);

        // Act
        container.Push(newPage);

        // Assert
        Assert.AreEqual(1, container.StackDepth);
        Assert.IsFalse(container.CanGoBack);
        Assert.AreEqual(newPage, container.CurrentPage);
    }

    [TestMethod]
    public void TestNavigationContainer_PreservePageState_False_DisposesPages()
    {
        // Arrange
        var config = new PlusUiConfiguration
        {
            EnableNavigationStack = true,
            PreservePageState = false
        };
        var vm = new TestViewModel();
        var rootPage = new TestPageA(vm);
        var container = new NavigationContainer(config);
        container.Push(rootPage);
        var pageB = new TestPageB(vm);
        var pageC = new TestPageC(vm);

        // Act
        container.Push(pageB);
        container.Push(pageC);

        // Assert
        // When PreservePageState is false, old pages are disposed except current
        Assert.AreEqual(2, container.StackDepth); // Only root and current should remain
    }

    #endregion

    #region NavigationService Tests

    [TestMethod]
    public void TestNavigationService_NavigateTo_CallsLifecycleMethods()
    {
        // Arrange
        var config = new PlusUiConfiguration { EnableNavigationStack = true };
        var serviceProvider = CreateServiceProvider(config);
        var rootPage = serviceProvider.GetRequiredService<TestPageA>();
        var navigationService = new PlusUiNavigationService(serviceProvider, rootPage);
        navigationService.Initialize();
        var container = serviceProvider.GetRequiredService<NavigationContainer>();

        // Act
        navigationService.NavigateTo<TestPageB>();
        var pageB = (TestPageB)container.CurrentPage;

        // Assert
        Assert.IsTrue(rootPage.DisappearingCalled);
        Assert.IsTrue(rootPage.OnNavigatedFromCalled);
        Assert.IsTrue(pageB.AppearingCalled);
        Assert.IsTrue(pageB.OnNavigatedToCalled);
    }

    [TestMethod]
    public void TestNavigationService_NavigateTo_WithParameter_PassesParameter()
    {
        // Arrange
        var config = new PlusUiConfiguration { EnableNavigationStack = true };
        var serviceProvider = CreateServiceProvider(config);
        var rootPage = serviceProvider.GetRequiredService<TestPageA>();
        var navigationService = new PlusUiNavigationService(serviceProvider, rootPage);
        navigationService.Initialize();
        var container = serviceProvider.GetRequiredService<NavigationContainer>();
        var parameter = new { Id = 123, Name = "TestParam" };

        // Act
        navigationService.NavigateTo<TestPageB>(parameter);
        var pageB = (TestPageB)container.CurrentPage;

        // Assert
        Assert.IsNotNull(pageB.ReceivedParameter);
        Assert.AreEqual(parameter, pageB.ReceivedParameter);
    }

    [TestMethod]
    public void TestNavigationService_GoBack_CallsLifecycleMethods()
    {
        // Arrange
        var config = new PlusUiConfiguration { EnableNavigationStack = true };
        var serviceProvider = CreateServiceProvider(config);
        var rootPage = serviceProvider.GetRequiredService<TestPageA>();
        var navigationService = new PlusUiNavigationService(serviceProvider, rootPage);
        navigationService.Initialize();
        var container = serviceProvider.GetRequiredService<NavigationContainer>();

        // Navigate forward
        navigationService.NavigateTo<TestPageB>();

        // Reset flags
        var pageB = (TestPageB)container.CurrentPage;

        // Act
        navigationService.GoBack();

        // Assert
        Assert.IsTrue(pageB.OnNavigatedToCalled); // Called during navigation
        Assert.AreEqual(rootPage, container.CurrentPage);
    }

    [TestMethod]
    public void TestNavigationService_GoBack_StackDisabled_ThrowsException()
    {
        // Arrange
        var config = new PlusUiConfiguration { EnableNavigationStack = false };
        var serviceProvider = CreateServiceProvider(config);
        var rootPage = serviceProvider.GetRequiredService<TestPageA>();
        var navigationService = new PlusUiNavigationService(serviceProvider, rootPage);
        navigationService.Initialize();

        // Act
        Assert.ThrowsExactly<InvalidOperationException>(navigationService.GoBack);

        // Assert - Exception expected
    }

    [TestMethod]
    public void TestNavigationService_GoBack_AtRoot_ThrowsException()
    {
        // Arrange
        var config = new PlusUiConfiguration { EnableNavigationStack = true };
        var serviceProvider = CreateServiceProvider(config);
        var rootPage = serviceProvider.GetRequiredService<TestPageA>();
        var navigationService = new PlusUiNavigationService(serviceProvider, rootPage);
        navigationService.Initialize();

        // Act
        Assert.ThrowsExactly<InvalidOperationException>(navigationService.GoBack);

        // Assert - Exception expected
    }

    [TestMethod]
    public void TestNavigationService_PopToRoot_ReturnsToRoot()
    {
        // Arrange
        var config = new PlusUiConfiguration { EnableNavigationStack = true };
        var serviceProvider = CreateServiceProvider(config);
        var rootPage = serviceProvider.GetRequiredService<TestPageA>();
        var navigationService = new PlusUiNavigationService(serviceProvider, rootPage);
        navigationService.Initialize();
        var container = serviceProvider.GetRequiredService<NavigationContainer>();

        // Navigate through multiple pages
        navigationService.NavigateTo<TestPageB>();
        navigationService.NavigateTo<TestPageC>();

        // Act
        navigationService.PopToRoot();

        // Assert
        Assert.AreEqual(1, navigationService.StackDepth);
        Assert.IsFalse(navigationService.CanGoBack);
        Assert.AreEqual(rootPage, container.CurrentPage);
    }

    [TestMethod]
    public void TestNavigationService_CanGoBack_ReflectsStackState()
    {
        // Arrange
        var config = new PlusUiConfiguration { EnableNavigationStack = true };
        var serviceProvider = CreateServiceProvider(config);
        var rootPage = serviceProvider.GetRequiredService<TestPageA>();
        var navigationService = new PlusUiNavigationService(serviceProvider, rootPage);
        navigationService.Initialize();

        // Act & Assert
        Assert.IsFalse(navigationService.CanGoBack);

        navigationService.NavigateTo<TestPageB>();
        Assert.IsTrue(navigationService.CanGoBack);

        navigationService.GoBack();
        Assert.IsFalse(navigationService.CanGoBack);
    }

    [TestMethod]
    public void TestNavigationService_StackDepth_CorrectValue()
    {
        // Arrange
        var config = new PlusUiConfiguration { EnableNavigationStack = true };
        var serviceProvider = CreateServiceProvider(config);
        var rootPage = serviceProvider.GetRequiredService<TestPageA>();
        var navigationService = new PlusUiNavigationService(serviceProvider, rootPage);
        navigationService.Initialize();

        // Act & Assert
        Assert.AreEqual(1, navigationService.StackDepth);

        navigationService.NavigateTo<TestPageB>();
        Assert.AreEqual(2, navigationService.StackDepth);

        navigationService.NavigateTo<TestPageC>();
        Assert.AreEqual(3, navigationService.StackDepth);

        navigationService.GoBack();
        Assert.AreEqual(2, navigationService.StackDepth);
    }

    [TestMethod]
    public void TestNavigationService_NavigateToSamePage_Ignored()
    {
        // Arrange
        var config = new PlusUiConfiguration { EnableNavigationStack = true };
        var serviceProvider = CreateServiceProvider(config);
        var rootPage = serviceProvider.GetRequiredService<TestPageA>();
        var navigationService = new PlusUiNavigationService(serviceProvider, rootPage);
        navigationService.Initialize();

        // Act
        var initialDepth = navigationService.StackDepth;
        navigationService.NavigateTo<TestPageA>(); // Same as root page

        // Assert
        Assert.AreEqual(initialDepth, navigationService.StackDepth);
    }

    #endregion

    #region PlusUiConfiguration Tests

    [TestMethod]
    public void TestPlusUiConfiguration_DefaultValues()
    {
        // Arrange & Act
        var config = new PlusUiConfiguration();

        // Assert
        Assert.IsFalse(config.EnableNavigationStack);
        Assert.IsTrue(config.PreservePageState);
        Assert.AreEqual(50, config.MaxStackDepth);
    }

    [TestMethod]
    public void TestPlusUiConfiguration_CustomValues()
    {
        // Arrange & Act
        var config = new PlusUiConfiguration
        {
            EnableNavigationStack = true,
            PreservePageState = false,
            MaxStackDepth = 100
        };

        // Assert
        Assert.IsTrue(config.EnableNavigationStack);
        Assert.IsFalse(config.PreservePageState);
        Assert.AreEqual(100, config.MaxStackDepth);
    }

    #endregion
}
