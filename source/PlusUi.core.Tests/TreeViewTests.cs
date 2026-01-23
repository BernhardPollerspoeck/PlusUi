using PlusUi.core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PlusUi.core.Tests;

[TestClass]
public class TreeViewTests
{
    #region Test Data Classes

    private class Category
    {
        public string Name { get; set; } = string.Empty;
        public List<Category> SubCategories { get; set; } = [];
    }

    private class Drive
    {
        public string Name { get; set; } = string.Empty;
        public List<Folder> Folders { get; set; } = [];
    }

    private class Folder
    {
        public string Name { get; set; } = string.Empty;
        public List<Folder> SubFolders { get; set; } = [];
        public List<File> Files { get; set; } = [];
    }

    private class File
    {
        public string Name { get; set; } = string.Empty;
    }

    private class TestViewModel : INotifyPropertyChanged
    {
        private object? _selectedItem;
        public object? SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion

    #region Phase 1: Core Structure - Default Values

    [TestMethod]
    public void TreeView_DefaultValues_ItemsSourceIsNull()
    {
        // Arrange & Act
        var treeView = new TreeView();

        // Assert
        Assert.IsNull(treeView.ItemsSource, "ItemsSource should be null by default");
    }

    [TestMethod]
    public void TreeView_DefaultValues_SelectedItemIsNull()
    {
        // Arrange & Act
        var treeView = new TreeView();

        // Assert
        Assert.IsNull(treeView.SelectedItem, "SelectedItem should be null by default");
    }

    [TestMethod]
    public void TreeView_DefaultValues_IndentationIs20()
    {
        // Arrange & Act
        var treeView = new TreeView();

        // Assert
        Assert.AreEqual(20f, treeView.Indentation, "Indentation should be 20 by default");
    }

    [TestMethod]
    public void TreeView_DefaultValues_ItemHeightIs32()
    {
        // Arrange & Act
        var treeView = new TreeView();

        // Assert
        Assert.AreEqual(32f, treeView.ItemHeight, "ItemHeight should be 32 by default");
    }

    [TestMethod]
    public void TreeView_DefaultValues_ExpanderSizeIs16()
    {
        // Arrange & Act
        var treeView = new TreeView();

        // Assert
        Assert.AreEqual(16f, treeView.ExpanderSize, "ExpanderSize should be 16 by default");
    }

    [TestMethod]
    public void TreeView_SetIndentation_PropertyIsSet()
    {
        // Arrange
        var treeView = new TreeView();

        // Act
        var result = treeView.SetIndentation(24f);

        // Assert
        Assert.AreEqual(24f, treeView.Indentation, "Indentation should be set to 24");
        Assert.AreSame(treeView, result, "Method should return the TreeView for chaining");
    }

    [TestMethod]
    public void TreeView_SetItemHeight_PropertyIsSet()
    {
        // Arrange
        var treeView = new TreeView();

        // Act
        var result = treeView.SetItemHeight(40f);

        // Assert
        Assert.AreEqual(40f, treeView.ItemHeight, "ItemHeight should be set to 40");
        Assert.AreSame(treeView, result, "Method should return the TreeView for chaining");
    }

    [TestMethod]
    public void TreeView_SetExpanderSize_PropertyIsSet()
    {
        // Arrange
        var treeView = new TreeView();

        // Act
        var result = treeView.SetExpanderSize(20f);

        // Assert
        Assert.AreEqual(20f, treeView.ExpanderSize, "ExpanderSize should be set to 20");
        Assert.AreSame(treeView, result, "Method should return the TreeView for chaining");
    }

    [TestMethod]
    public void TreeView_MethodChaining_ReturnsTreeView()
    {
        // Arrange
        var treeView = new TreeView();

        // Act
        var result = treeView
            .SetIndentation(24f)
            .SetItemHeight(40f)
            .SetExpanderSize(20f);

        // Assert
        Assert.AreSame(treeView, result, "All methods should return the TreeView for chaining");
        Assert.AreEqual(24f, treeView.Indentation, "Indentation should be set");
        Assert.AreEqual(40f, treeView.ItemHeight, "ItemHeight should be set");
        Assert.AreEqual(20f, treeView.ExpanderSize, "ExpanderSize should be set");
    }

    [TestMethod]
    public void TreeView_AccessibilityRole_IsTree()
    {
        // Arrange & Act
        var treeView = new TreeView();

        // Assert
        Assert.AreEqual(AccessibilityRole.Tree, treeView.AccessibilityRole, "AccessibilityRole should be Tree");
    }

    [TestMethod]
    public void TreeView_SetChildrenSelector_TypeSpecific_Registered()
    {
        // Arrange
        var treeView = new TreeView();

        // Act
        var result = treeView.SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>());

        // Assert
        Assert.AreSame(treeView, result, "Method should return the TreeView for chaining");
        Assert.IsTrue(treeView.HasChildrenSelectorFor<Category>(), "Should have a selector registered for Category type");
    }

    [TestMethod]
    public void TreeView_SetChildrenSelector_MultipleTypes_AllRegistered()
    {
        // Arrange
        var treeView = new TreeView();

        // Act
        var result = treeView
            .SetChildrenSelector<Drive>(d => d.Folders.Cast<object>())
            .SetChildrenSelector<Folder>(f => f.SubFolders.Cast<object>().Concat(f.Files))
            .SetChildrenSelector<File>(f => Enumerable.Empty<object>());

        // Assert
        Assert.AreSame(treeView, result, "Method should return the TreeView for chaining");
        Assert.IsTrue(treeView.HasChildrenSelectorFor<Drive>(), "Should have selector for Drive");
        Assert.IsTrue(treeView.HasChildrenSelectorFor<Folder>(), "Should have selector for Folder");
        Assert.IsTrue(treeView.HasChildrenSelectorFor<File>(), "Should have selector for File");
    }

    [TestMethod]
    public void TreeView_SetItemsSource_PropertyIsSet()
    {
        // Arrange
        var treeView = new TreeView();
        var items = new List<object> { new Category { Name = "Root" } };

        // Act
        var result = treeView.SetItemsSource(items);

        // Assert
        Assert.AreSame(items, treeView.ItemsSource, "ItemsSource should be set");
        Assert.AreSame(treeView, result, "Method should return the TreeView for chaining");
    }

    [TestMethod]
    public void TreeView_SetItemTemplate_PropertyIsSet()
    {
        // Arrange
        var treeView = new TreeView();
        UiElement template(object item, int depth) => new Label().SetText(item.ToString() ?? "");

        // Act
        var result = treeView.SetItemTemplate(template);

        // Assert
        Assert.IsNotNull(treeView.ItemTemplate, "ItemTemplate should be set");
        Assert.AreSame(treeView, result, "Method should return the TreeView for chaining");
    }

    #endregion

    #region Phase 2: Hierarchy & Node Structure

    [TestMethod]
    public void TreeView_BuildNodes_CreatesRootNodes()
    {
        // Arrange
        var treeView = new TreeView();
        var root1 = new Category { Name = "Root1" };
        var root2 = new Category { Name = "Root2" };
        var items = new List<object> { root1, root2 };

        // Act
        treeView
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(items);
        treeView.BuildNodes();

        // Assert
        Assert.AreEqual(2, treeView.RootNodeCount, "Should have 2 root nodes");
    }

    [TestMethod]
    public void TreeView_Node_ExpandedHeight_EqualsItemHeightWhenCollapsed()
    {
        // Arrange
        var treeView = new TreeView();
        var root = new Category
        {
            Name = "Root",
            SubCategories =
            [
                new Category { Name = "Child1" },
                new Category { Name = "Child2" }
            ]
        };
        treeView
            .SetItemHeight(32f)
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root });
        treeView.BuildNodes();

        // Act
        var expandedHeight = treeView.GetNodeExpandedHeight(root);

        // Assert - collapsed, so only its own height
        Assert.AreEqual(32f, expandedHeight, "Collapsed node should have ExpandedHeight equal to ItemHeight");
    }

    [TestMethod]
    public void TreeView_Node_ExpandedHeight_IncludesChildrenWhenExpanded()
    {
        // Arrange
        var treeView = new TreeView();
        var root = new Category
        {
            Name = "Root",
            SubCategories =
            [
                new Category { Name = "Child1" },
                new Category { Name = "Child2" }
            ]
        };
        treeView
            .SetItemHeight(32f)
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root });
        treeView.BuildNodes();

        // Act
        treeView.ExpandNode(root);
        var expandedHeight = treeView.GetNodeExpandedHeight(root);

        // Assert - root + 2 children = 3 * 32 = 96
        Assert.AreEqual(96f, expandedHeight, "Expanded node should include children heights");
    }

    [TestMethod]
    public void TreeView_ExpandNode_LoadsChildrenLazily()
    {
        // Arrange
        var treeView = new TreeView();
        var child1 = new Category { Name = "Child1" };
        var child2 = new Category { Name = "Child2" };
        var root = new Category
        {
            Name = "Root",
            SubCategories = [child1, child2]
        };
        treeView
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root });
        treeView.BuildNodes();

        // Act
        treeView.ExpandNode(root);

        // Assert
        Assert.IsTrue(treeView.IsNodeExpanded(root), "Root should be expanded");
        Assert.IsTrue(treeView.HasNode(child1), "Child1 should be registered after expand");
        Assert.IsTrue(treeView.HasNode(child2), "Child2 should be registered after expand");
    }

    [TestMethod]
    public void TreeView_CollapseNode_UpdatesExpandedHeight()
    {
        // Arrange
        var treeView = new TreeView();
        var root = new Category
        {
            Name = "Root",
            SubCategories =
            [
                new Category { Name = "Child1" },
                new Category { Name = "Child2" }
            ]
        };
        treeView
            .SetItemHeight(32f)
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root });
        treeView.BuildNodes();
        treeView.ExpandNode(root);

        // Act
        treeView.CollapseNode(root);
        var expandedHeight = treeView.GetNodeExpandedHeight(root);

        // Assert - collapsed again
        Assert.AreEqual(32f, expandedHeight, "Collapsed node should revert to ItemHeight");
        Assert.IsFalse(treeView.IsNodeExpanded(root), "Node should be collapsed");
    }

    [TestMethod]
    public void TreeView_ToggleExpand_TogglesState()
    {
        // Arrange
        var treeView = new TreeView();
        var root = new Category
        {
            Name = "Root",
            SubCategories = [new Category { Name = "Child" }]
        };
        treeView
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root });
        treeView.BuildNodes();

        // Act & Assert
        Assert.IsFalse(treeView.IsNodeExpanded(root), "Initially collapsed");

        treeView.ToggleNode(root);
        Assert.IsTrue(treeView.IsNodeExpanded(root), "Should be expanded after first toggle");

        treeView.ToggleNode(root);
        Assert.IsFalse(treeView.IsNodeExpanded(root), "Should be collapsed after second toggle");
    }

    [TestMethod]
    public void TreeView_TotalHeight_SumOfRootExpandedHeights()
    {
        // Arrange
        var treeView = new TreeView();
        var root1 = new Category
        {
            Name = "Root1",
            SubCategories =
            [
                new Category { Name = "Child1" },
                new Category { Name = "Child2" }
            ]
        };
        var root2 = new Category { Name = "Root2" };
        treeView
            .SetItemHeight(32f)
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root1, root2 });
        treeView.BuildNodes();

        // Act - expand root1
        treeView.ExpandNode(root1);
        var totalHeight = treeView.TotalHeight;

        // Assert - root1 (32 + 2*32 = 96) + root2 (32) = 128
        Assert.AreEqual(128f, totalHeight, "TotalHeight should be sum of all root ExpandedHeights");
    }

    [TestMethod]
    public void TreeView_ExpandNode_UpdatesParentHeights()
    {
        // Arrange
        var grandchild = new Category { Name = "Grandchild" };
        var child = new Category
        {
            Name = "Child",
            SubCategories = [grandchild]
        };
        var root = new Category
        {
            Name = "Root",
            SubCategories = [child]
        };
        var treeView = new TreeView();
        treeView
            .SetItemHeight(32f)
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root });
        treeView.BuildNodes();

        // Expand root first
        treeView.ExpandNode(root);
        var rootHeightAfterExpandRoot = treeView.GetNodeExpandedHeight(root);
        Assert.AreEqual(64f, rootHeightAfterExpandRoot, "Root + 1 child = 64");

        // Act - expand child
        treeView.ExpandNode(child);
        var rootHeightAfterExpandChild = treeView.GetNodeExpandedHeight(root);

        // Assert - root's height should now include grandchild
        // Root (32) + Child expanded (32 + 32 = 64) = 96
        Assert.AreEqual(96f, rootHeightAfterExpandChild, "Parent height should update when child expands");
    }

    [TestMethod]
    public void TreeView_HeterogeneousTypes_ExpandWithDifferentSelectors()
    {
        // Arrange
        var file1 = new File { Name = "file1.txt" };
        var file2 = new File { Name = "file2.txt" };
        var folder = new Folder
        {
            Name = "Folder",
            SubFolders = [],
            Files = [file1, file2]
        };
        var drive = new Drive
        {
            Name = "C:",
            Folders = [folder]
        };

        var treeView = new TreeView();
        treeView
            .SetItemHeight(32f)
            .SetChildrenSelector<Drive>(d => d.Folders.Cast<object>())
            .SetChildrenSelector<Folder>(f => f.SubFolders.Cast<object>().Concat(f.Files))
            .SetItemsSource(new List<object> { drive });
        treeView.BuildNodes();

        // Act - expand drive
        treeView.ExpandNode(drive);
        Assert.IsTrue(treeView.HasNode(folder), "Folder should be registered");

        // Expand folder
        treeView.ExpandNode(folder);

        // Assert
        Assert.IsTrue(treeView.HasNode(file1), "File1 should be registered after expanding folder");
        Assert.IsTrue(treeView.HasNode(file2), "File2 should be registered after expanding folder");

        // Total height: Drive(32) + Folder expanded(32 + 2*32 = 96) = 128
        var totalHeight = treeView.TotalHeight;
        Assert.AreEqual(128f, totalHeight, "Heterogeneous tree should calculate heights correctly");
    }

    #endregion

    #region Phase 3: Selection & Binding

    [TestMethod]
    public void TreeView_SelectItem_UpdatesSelectedItem()
    {
        // Arrange
        var root = new Category { Name = "Root" };
        var treeView = new TreeView();
        treeView
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root });
        treeView.BuildNodes();

        // Act
        treeView.SetSelectedItem(root);

        // Assert
        Assert.AreSame(root, treeView.SelectedItem, "SelectedItem should be set to root");
    }

    [TestMethod]
    public void TreeView_SelectItem_NotifiesSetter()
    {
        // Arrange
        var root = new Category { Name = "Root" };
        object? notifiedValue = null;
        var treeView = new TreeView();
        treeView
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root });
        treeView.BuildNodes();

        // Bind with setter
        treeView.BindSelectedItem(
            () => (object?)null,
            value => notifiedValue = value);

        // Act
        treeView.SetSelectedItem(root);

        // Assert
        Assert.AreSame(root, notifiedValue, "Setter should be notified with the selected item");
    }

    [TestMethod]
    public void TreeView_BindSelectedItem_TwoWay()
    {
        // Arrange
        var root1 = new Category { Name = "Root1" };
        var root2 = new Category { Name = "Root2" };
        var vm = new TestViewModel();

        var treeView = new TreeView();
        treeView
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root1, root2 })
            .BindSelectedItem(
                () => vm.SelectedItem,
                v => vm.SelectedItem = v);
        treeView.BuildNodes();

        // Act - set from TreeView
        treeView.SetSelectedItem(root1);

        // Assert
        Assert.AreSame(root1, vm.SelectedItem, "ViewModel should be updated when TreeView selection changes");

        // Act - update bindings (simulates VM -> TreeView)
        vm.SelectedItem = root2;
        treeView.UpdateBindings(nameof(TestViewModel.SelectedItem));

        // Assert
        Assert.AreSame(root2, treeView.SelectedItem, "TreeView should update when ViewModel changes");
    }

    [TestMethod]
    public void TreeView_ObservableCollection_Add_RebuildsList()
    {
        // Arrange
        var items = new ObservableCollection<object>();
        var root1 = new Category { Name = "Root1" };

        var treeView = new TreeView();
        treeView
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(items);
        treeView.BuildNodes();

        Assert.AreEqual(0, treeView.RootNodeCount, "Initially no nodes");

        // Act - add item
        items.Add(root1);
        treeView.BuildNodes(); // Manually rebuild (auto-rebuild in INotifyCollectionChanged would be Phase 4)

        // Assert
        Assert.AreEqual(1, treeView.RootNodeCount, "Should have 1 node after add");
        Assert.IsTrue(treeView.HasNode(root1), "New item should be in nodes");
    }

    [TestMethod]
    public void TreeView_ObservableCollection_Remove_RebuildsList()
    {
        // Arrange
        var root1 = new Category { Name = "Root1" };
        var root2 = new Category { Name = "Root2" };
        var items = new ObservableCollection<object> { root1, root2 };

        var treeView = new TreeView();
        treeView
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(items);
        treeView.BuildNodes();

        Assert.AreEqual(2, treeView.RootNodeCount, "Initially 2 nodes");

        // Act - remove item
        items.Remove(root1);
        treeView.BuildNodes(); // Manually rebuild

        // Assert
        Assert.AreEqual(1, treeView.RootNodeCount, "Should have 1 node after remove");
        Assert.IsFalse(treeView.HasNode(root1), "Removed item should not be in nodes");
        Assert.IsTrue(treeView.HasNode(root2), "Remaining item should still be in nodes");
    }

    [TestMethod]
    public void TreeView_FindNodeByItem_ReturnsCorrectNode()
    {
        // Arrange
        var root = new Category { Name = "Root" };
        var treeView = new TreeView();
        treeView
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root });
        treeView.BuildNodes();

        // Act & Assert
        Assert.IsTrue(treeView.HasNode(root), "Root should be findable");
        Assert.AreEqual(0, treeView.GetNodeDepth(root), "Root should have depth 0");
    }

    [TestMethod]
    public void TreeView_FindNodeByItem_DeepChild_ReturnsNode()
    {
        // Arrange
        var grandchild = new Category { Name = "Grandchild" };
        var child = new Category { Name = "Child", SubCategories = [grandchild] };
        var root = new Category { Name = "Root", SubCategories = [child] };

        var treeView = new TreeView();
        treeView
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root });
        treeView.BuildNodes();

        // Expand to reveal children
        treeView.ExpandNode(root);
        treeView.ExpandNode(child);

        // Act & Assert
        Assert.IsTrue(treeView.HasNode(grandchild), "Grandchild should be findable after expansion");
        Assert.AreEqual(2, treeView.GetNodeDepth(grandchild), "Grandchild should have depth 2");
    }

    [TestMethod]
    public void TreeView_SelectionClearing_SetsToNull()
    {
        // Arrange
        var root = new Category { Name = "Root" };
        var treeView = new TreeView();
        treeView
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root });
        treeView.BuildNodes();
        treeView.SetSelectedItem(root);
        Assert.AreSame(root, treeView.SelectedItem);

        // Act
        treeView.SetSelectedItem(null);

        // Assert
        Assert.IsNull(treeView.SelectedItem, "SelectedItem should be null after clearing");
    }

    #endregion

    #region Phase 4: Virtualization & Scrolling

    [TestMethod]
    public void TreeView_ImplementsIScrollableControl()
    {
        // Arrange & Act
        var treeView = new TreeView();

        // Assert
        Assert.IsTrue(treeView is IScrollableControl, "TreeView should implement IScrollableControl");
    }

    [TestMethod]
    public void TreeView_HandleScroll_VerticalScrollUpdatesOffset()
    {
        // Arrange
        var items = Enumerable.Range(0, 20)
            .Select(i => new Category { Name = $"Item{i}" })
            .Cast<object>()
            .ToList();

        var treeView = new TreeView();
        treeView
            .SetItemHeight(32f)
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(items);
        treeView.BuildNodes();

        // Act
        treeView.HandleScroll(0, 100);

        // Assert
        Assert.AreEqual(100f, treeView.ScrollOffset, "ScrollOffset should be updated by deltaY");
    }

    [TestMethod]
    public void TreeView_ScrollOffset_ClampsToValidRange()
    {
        // Arrange
        var items = Enumerable.Range(0, 5)
            .Select(i => new Category { Name = $"Item{i}" })
            .Cast<object>()
            .ToList();

        var treeView = new TreeView();
        treeView
            .SetItemHeight(32f)
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(items);
        treeView.BuildNodes();
        // Total height = 5 * 32 = 160

        // Act - try to scroll past end
        treeView.HandleScroll(0, 1000);

        // Assert - should clamp to max
        Assert.IsLessThanOrEqualTo(treeView.ScrollOffset, treeView.TotalHeight, "ScrollOffset should be clamped to TotalHeight");

        // Act - try to scroll before start
        treeView.HandleScroll(0, -2000);

        // Assert - should clamp to 0
        Assert.AreEqual(0f, treeView.ScrollOffset, "ScrollOffset should not go negative");
    }

    [TestMethod]
    public void TreeView_GetVisibleNodes_ReturnsOnlyViewportNodes()
    {
        // Arrange
        var items = Enumerable.Range(0, 10)
            .Select(i => new Category { Name = $"Item{i}" })
            .Cast<object>()
            .ToList();

        var treeView = new TreeView();
        treeView
            .SetItemHeight(32f)
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(items);
        treeView.BuildNodes();

        // Act - viewport of 100px shows 3-4 items (100/32 = 3.125)
        var visibleNodes = treeView.GetVisibleNodes(0, 100).ToList();

        // Assert
        Assert.IsTrue(visibleNodes.Count >= 3 && visibleNodes.Count <= 4,
            $"With viewport 100px and itemHeight 32, should see 3-4 nodes, got {visibleNodes.Count}");
    }

    [TestMethod]
    public void TreeView_GetVisibleNodes_SkipsCollapsedSubtrees()
    {
        // Arrange
        var child1 = new Category { Name = "Child1" };
        var child2 = new Category { Name = "Child2" };
        var root1 = new Category { Name = "Root1", SubCategories = [child1, child2] };
        var root2 = new Category { Name = "Root2" };

        var treeView = new TreeView();
        treeView
            .SetItemHeight(32f)
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root1, root2 });
        treeView.BuildNodes();

        // Act - get all visible (collapsed root1 + root2)
        var visibleNodes = treeView.GetVisibleNodes(0, 200).ToList();

        // Assert - only 2 nodes visible (children hidden)
        Assert.HasCount(2, visibleNodes, "Should only see root nodes when collapsed");
        Assert.IsTrue(visibleNodes.Any(n => n.Item == root1), "Root1 should be visible");
        Assert.IsTrue(visibleNodes.Any(n => n.Item == root2), "Root2 should be visible");
        Assert.IsFalse(visibleNodes.Any(n => n.Item == child1), "Child1 should NOT be visible when collapsed");
    }

    [TestMethod]
    public void TreeView_GetVisibleNodes_IncludesExpandedChildren()
    {
        // Arrange
        var child1 = new Category { Name = "Child1" };
        var child2 = new Category { Name = "Child2" };
        var root1 = new Category { Name = "Root1", SubCategories = [child1, child2] };
        var root2 = new Category { Name = "Root2" };

        var treeView = new TreeView();
        treeView
            .SetItemHeight(32f)
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root1, root2 });
        treeView.BuildNodes();
        treeView.ExpandNode(root1);

        // Act - get all visible
        var visibleNodes = treeView.GetVisibleNodes(0, 200).ToList();

        // Assert - 4 nodes visible (root1 + 2 children + root2)
        Assert.HasCount(4, visibleNodes, "Should see root1 + 2 children + root2 when expanded");
        Assert.IsTrue(visibleNodes.Any(n => n.Item == child1), "Child1 should be visible when parent expanded");
        Assert.IsTrue(visibleNodes.Any(n => n.Item == child2), "Child2 should be visible when parent expanded");
    }

    [TestMethod]
    public void TreeView_GetVisibleNodes_ScrolledView()
    {
        // Arrange
        var items = Enumerable.Range(0, 10)
            .Select(i => new Category { Name = $"Item{i}" })
            .Cast<object>()
            .ToList();

        var treeView = new TreeView();
        treeView
            .SetItemHeight(32f)
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(items);
        treeView.BuildNodes();

        // Act - scroll down 64px (2 items) and get viewport of 64px
        var visibleNodes = treeView.GetVisibleNodes(64, 64).ToList();

        // Assert - should see items 2 and 3
        Assert.HasCount(2, visibleNodes, "Should see 2 items in 64px viewport");
        Assert.AreEqual("Item2", ((Category)visibleNodes[0].Item).Name, "First visible should be Item2");
        Assert.AreEqual("Item3", ((Category)visibleNodes[1].Item).Name, "Second visible should be Item3");
    }

    [TestMethod]
    public void TreeView_ExpandNode_UpdatesScrollBounds()
    {
        // Arrange
        var child1 = new Category { Name = "Child1" };
        var child2 = new Category { Name = "Child2" };
        var root = new Category { Name = "Root", SubCategories = [child1, child2] };

        var treeView = new TreeView();
        treeView
            .SetItemHeight(32f)
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root });
        treeView.BuildNodes();

        var initialHeight = treeView.TotalHeight;
        Assert.AreEqual(32f, initialHeight, "Initial height should be 1 item");

        // Act
        treeView.ExpandNode(root);

        // Assert
        Assert.AreEqual(96f, treeView.TotalHeight, "After expand, height should include children");
    }

    #endregion

    #region Phase 5: Rendering & Interaction

    [TestMethod]
    public void TreeView_HitTest_InsideBounds_ReturnsTreeView()
    {
        // Arrange
        var root = new Category { Name = "Root" };
        var treeView = new TreeView();
        treeView
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root });
        treeView.BuildNodes();

        // Simulate measure/arrange
        treeView.Measure(new Size(200, 200), true);
        treeView.Arrange(new Rect(0, 0, 200, 200));

        // Act
        var hit = treeView.HitTest(new Point(50, 16));

        // Assert
        Assert.IsNotNull(hit, "HitTest inside bounds should return control");
        Assert.AreSame(treeView, hit, "HitTest should return the TreeView");
    }

    [TestMethod]
    public void TreeView_HitTest_OutsideBounds_ReturnsNull()
    {
        // Arrange
        var root = new Category { Name = "Root" };
        var treeView = new TreeView();
        treeView
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root });
        treeView.BuildNodes();

        // Simulate measure/arrange
        treeView.Measure(new Size(200, 200), true);
        treeView.Arrange(new Rect(0, 0, 200, 200));

        // Act
        var hit = treeView.HitTest(new Point(300, 300));

        // Assert
        Assert.IsNull(hit, "HitTest outside bounds should return null");
    }

    [TestMethod]
    public void TreeView_HitTest_TracksHoveredNode()
    {
        // Arrange
        var root1 = new Category { Name = "Root1" };
        var root2 = new Category { Name = "Root2" };
        var treeView = new TreeView();
        treeView
            .SetItemHeight(32f)
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root1, root2 });
        treeView.BuildNodes();

        // Simulate measure/arrange
        treeView.Measure(new Size(200, 100), true);
        treeView.Arrange(new Rect(0, 0, 200, 100));

        // Act - click on second item (Y = 48, which is in second row starting at Y=32)
        var hit = treeView.HitTest(new Point(50, 48));

        // Assert
        Assert.IsNotNull(hit, "HitTest should return control");
        Assert.AreSame(root2, treeView.HoveredItem, "HoveredItem should be root2");
    }

    [TestMethod]
    public void TreeView_InvokeCommand_SelectsHoveredItem()
    {
        // Arrange
        var root1 = new Category { Name = "Root1" };
        var root2 = new Category { Name = "Root2" };
        var treeView = new TreeView();
        treeView
            .SetItemHeight(32f)
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root1, root2 });
        treeView.BuildNodes();

        // Simulate measure/arrange
        treeView.Measure(new Size(200, 100), true);
        treeView.Arrange(new Rect(0, 0, 200, 100));

        // Hit test to set hovered item
        treeView.HitTest(new Point(50, 48));

        // Act
        treeView.InvokeCommand();

        // Assert
        Assert.AreSame(root2, treeView.SelectedItem, "SelectedItem should be the clicked item");
    }

    [TestMethod]
    public void TreeView_HitTest_OnExpander_SetsExpanderHit()
    {
        // Arrange
        var child = new Category { Name = "Child" };
        var root = new Category { Name = "Root", SubCategories = [child] };
        var treeView = new TreeView();
        treeView
            .SetItemHeight(32f)
            .SetExpanderSize(16f)
            .SetIndentation(20f)
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root });
        treeView.BuildNodes();

        // Simulate measure/arrange
        treeView.Measure(new Size(200, 100), true);
        treeView.Arrange(new Rect(0, 0, 200, 100));

        // Act - click on expander area (left side of first item)
        treeView.HitTest(new Point(8, 16));

        // Assert
        Assert.IsTrue(treeView.IsExpanderHit, "Should detect expander hit");
    }

    [TestMethod]
    public void TreeView_InvokeCommand_OnExpander_TogglesExpand()
    {
        // Arrange
        var child = new Category { Name = "Child" };
        var root = new Category { Name = "Root", SubCategories = [child] };
        var treeView = new TreeView();
        treeView
            .SetItemHeight(32f)
            .SetExpanderSize(16f)
            .SetIndentation(20f)
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemsSource(new List<object> { root });
        treeView.BuildNodes();

        // Simulate measure/arrange
        treeView.Measure(new Size(200, 100), true);
        treeView.Arrange(new Rect(0, 0, 200, 100));

        Assert.IsFalse(treeView.IsNodeExpanded(root), "Initially collapsed");

        // Hit expander
        treeView.HitTest(new Point(8, 16));

        // Act
        treeView.InvokeCommand();

        // Assert
        Assert.IsTrue(treeView.IsNodeExpanded(root), "Should be expanded after clicking expander");
    }

    [TestMethod]
    public void TreeView_ImplementsIHoverableControl()
    {
        // Arrange & Act
        var treeView = new TreeView();

        // Assert
        Assert.IsTrue(treeView is IHoverableControl, "TreeView should implement IHoverableControl");
    }

    [TestMethod]
    public void TreeView_ImplementsIInputControl()
    {
        // Arrange & Act
        var treeView = new TreeView();

        // Assert
        Assert.IsTrue(treeView is IInputControl, "TreeView should implement IInputControl");
    }

    #endregion

    #region Bug Investigation: Two TreeViews

    [TestMethod]
    public void TreeView_TwoTreeViewsInGrid_ChildrenConsistentAfterLayout()
    {
        // Arrange - Two TreeViews side by side (like DebugServer Inspector)
        var items1 = new List<object>
        {
            new Category { Name = "Item1A" },
            new Category { Name = "Item1B" }
        };
        var items2 = new List<object>
        {
            new Category { Name = "Item2A" },
            new Category { Name = "Item2B" },
            new Category { Name = "Item2C" }
        };

        var treeView1 = new TreeView()
            .SetItemHeight(28)
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemTemplate((item, depth) => new Label().SetText(((Category)item).Name))
            .SetItemsSource(items1);

        var treeView2 = new TreeView()
            .SetItemHeight(28)
            .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
            .SetItemTemplate((item, depth) => new Label().SetText(((Category)item).Name))
            .SetItemsSource(items2);

        var grid = new Grid()
            .AddRow(Row.Star)
            .AddColumn(Column.Star)
            .AddColumn(Column.Star)
            .AddChild(treeView1, row: 0, column: 0)
            .AddChild(treeView2, row: 0, column: 1);

        // Act - Full layout cycle
        var availableSize = new Size(800, 600);
        grid.Measure(availableSize);
        grid.Arrange(new Rect(0, 0, 800, 600));

        // Capture children count after arrange
        var tree1ChildrenAfterArrange = treeView1.Children.Count;
        var tree2ChildrenAfterArrange = treeView2.Children.Count;

        // Trigger another measure (simulating what might happen during render)
        grid.Measure(availableSize);

        // Check if children count changed
        var tree1ChildrenAfterSecondMeasure = treeView1.Children.Count;
        var tree2ChildrenAfterSecondMeasure = treeView2.Children.Count;

        // Assert
        Assert.AreEqual(2, tree1ChildrenAfterArrange, "TreeView1 should have 2 children after arrange");
        Assert.AreEqual(3, tree2ChildrenAfterArrange, "TreeView2 should have 3 children after arrange");
        Assert.AreEqual(tree1ChildrenAfterArrange, tree1ChildrenAfterSecondMeasure,
            "TreeView1 children count should be same after second measure");
        Assert.AreEqual(tree2ChildrenAfterArrange, tree2ChildrenAfterSecondMeasure,
            "TreeView2 children count should be same after second measure");
    }

    #endregion

    #region VisualOffset Bug Tests

    /// <summary>
    /// Test Label that captures its VisualOffset and Position during Render
    /// </summary>
    private class TestLabel : Label
    {
        public Point CapturedVisualOffset { get; private set; }
        public Point CapturedPosition { get; private set; }
        public bool WasRendered { get; private set; }

        public override void Render(SkiaSharp.SKCanvas canvas)
        {
            CapturedVisualOffset = VisualOffset;
            CapturedPosition = Position;
            WasRendered = true;
            base.Render(canvas);
        }
    }

    /// <summary>
    /// Test UserControl that wraps a TreeView
    /// </summary>
    private class TestTreeViewUserControl : UserControl
    {
        public TreeView TreeView { get; }

        public TestTreeViewUserControl(List<object> items, string prefix)
        {
            TreeView = new TreeView()
                .SetItemsSource(items)
                .SetChildrenSelector<Category>(c => c.SubCategories.Cast<object>())
                .SetItemTemplate((item, depth) =>
                {
                    var label = new TestLabel();
                    label.SetText(prefix + ": " + ((Category)item).Name);
                    label.SetTextColor(Colors.White);
                    return label;
                })
                .SetItemHeight(28);
        }

        protected override UiElement Build() => TreeView;

        /// <summary>
        /// Get the TestLabels that are currently in the TreeView's Children (after layout)
        /// </summary>
        public List<TestLabel> GetCurrentTestLabels() =>
            TreeView.Children.OfType<TestLabel>().ToList();
    }

    [TestMethod]
    public void TreeView_TwoInUserControls_VisualOffsetIsCorrectDuringRender()
    {
        // Arrange - Create two TreeViews wrapped in UserControls
        var items1 = new List<object>
        {
            new Category { Name = "Cat1" },
            new Category { Name = "Cat2" }
        };
        var items2 = new List<object>
        {
            new Category { Name = "Other1" },
            new Category { Name = "Other2" },
            new Category { Name = "Other3" }
        };

        var userControl1 = new TestTreeViewUserControl(items1, "UC1");
        var userControl2 = new TestTreeViewUserControl(items2, "UC2");

        var grid = new Grid()
            .AddRow(Row.Star)
            .AddColumn(Column.Star)
            .AddColumn(Column.Star)
            .AddChild(userControl1, row: 0, column: 0)
            .AddChild(userControl2, row: 0, column: 1);

        // Act - Full layout cycle
        var availableSize = new Size(800, 600);
        grid.BuildContent();
        grid.Measure(availableSize);
        grid.Arrange(new Rect(0, 0, 800, 600));

        // Render with a real canvas
        using var bitmap = new SkiaSharp.SKBitmap(800, 600);
        using var canvas = new SkiaSharp.SKCanvas(bitmap);
        grid.Render(canvas);

        // Get labels that are actually in Children (after all layout passes)
        var labelsUC1 = userControl1.GetCurrentTestLabels();
        var labelsUC2 = userControl2.GetCurrentTestLabels();

        // Assert - Check that labels were rendered
        Assert.IsNotEmpty(labelsUC1, "UC1 should have labels");
        Assert.IsNotEmpty(labelsUC2, "UC2 should have labels");

        // Check that all labels in UC1 were rendered
        foreach (var label in labelsUC1)
        {
            Assert.IsTrue(label.WasRendered, $"Label '{label.Text}' in UC1 should have been rendered");
        }

        // Check that all labels in UC2 were rendered
        foreach (var label in labelsUC2)
        {
            Assert.IsTrue(label.WasRendered, $"Label '{label.Text}' in UC2 should have been rendered");
        }

        // The critical check: Labels in UC2 should have Position.X > 400 (right half)
        // AND their VisualOffset should NOT push them outside the visible area
        foreach (var label in labelsUC2)
        {
            var effectiveX = label.CapturedPosition.X + label.CapturedVisualOffset.X;
            Assert.IsGreaterThanOrEqualTo(400, effectiveX,
                $"Label '{label.Text}' effective X ({effectiveX}) should be >= 400 (right column). " +
                $"Position.X={label.CapturedPosition.X}, VisualOffset.X={label.CapturedVisualOffset.X}");
            Assert.IsLessThan(800, effectiveX,
                $"Label '{label.Text}' effective X ({effectiveX}) should be < 800 (within grid). " +
                $"Position.X={label.CapturedPosition.X}, VisualOffset.X={label.CapturedVisualOffset.X}");
        }

        // Labels in UC1 should have Position.X < 400 (left half)
        foreach (var label in labelsUC1)
        {
            var effectiveX = label.CapturedPosition.X + label.CapturedVisualOffset.X;
            Assert.IsGreaterThanOrEqualTo(0, effectiveX,
                $"Label '{label.Text}' effective X ({effectiveX}) should be >= 0. " +
                $"Position.X={label.CapturedPosition.X}, VisualOffset.X={label.CapturedVisualOffset.X}");
            Assert.IsLessThan(400, effectiveX,
                $"Label '{label.Text}' effective X ({effectiveX}) should be < 400 (left column). " +
                $"Position.X={label.CapturedPosition.X}, VisualOffset.X={label.CapturedVisualOffset.X}");
        }
    }

    #endregion
}
