using Android.Views.Accessibility;
using PlusUi.core;

namespace PlusUi.droid.Accessibility;

/// <summary>
/// Provides virtual accessibility nodes for PlusUi elements.
/// </summary>
public sealed class PlusUiAccessibilityNodeProvider : AccessibilityNodeProvider
{
    private readonly Android.Views.View _hostView;
    private readonly Func<UiElement?> _rootProvider;
    private int _focusedVirtualViewId = Android.Views.View.NoId;

    public PlusUiAccessibilityNodeProvider(Android.Views.View hostView, Func<UiElement?> rootProvider)
    {
        _hostView = hostView;
        _rootProvider = rootProvider;
    }

    public void NotifyFocusChanged(int virtualViewId)
    {
        _focusedVirtualViewId = virtualViewId;
    }

    public void InvalidateVirtualView()
    {
        // Force rebuild of accessibility tree
    }

    public override AccessibilityNodeInfo? CreateAccessibilityNodeInfo(int virtualViewId)
    {
        if (virtualViewId == Android.Views.View.NoId)
        {
            // Return info for the host view itself
            var rootInfo = AccessibilityNodeInfo.Obtain(_hostView);
            if (rootInfo != null)
            {
                _hostView.OnInitializeAccessibilityNodeInfo(rootInfo);
                rootInfo.ClassName = "android.view.ViewGroup";

                // Add children from root element
                var root = _rootProvider();
                if (root != null)
                {
                    AddVirtualChildren(rootInfo, root);
                }
            }
            return rootInfo;
        }

        // Find element by virtual view ID and create node info
        var element = FindElementByVirtualId(virtualViewId);
        if (element != null)
        {
            return CreateAccessibilityNodeInfo(element);
        }

        return null;
    }

    public AccessibilityNodeInfo? CreateAccessibilityNodeInfo(UiElement element)
    {
        var info = AccessibilityNodeInfo.Obtain(_hostView, element.GetHashCode());
        if (info == null)
        {
            return null;
        }

        // Set basic properties
        info.ContentDescription = element.GetComputedAccessibilityLabel();

        // Set accessibility class based on role
        info.ClassName = GetAccessibilityClassName(element.AccessibilityRole);

        // Set bounds
        var rect = new Android.Graphics.Rect(
            (int)element.Position.X,
            (int)element.Position.Y,
            (int)(element.Position.X + element.ElementSize.Width),
            (int)(element.Position.Y + element.ElementSize.Height));
        info.SetBoundsInParent(rect);

        // Set enabled/focusable state
        var traits = element.GetComputedAccessibilityTraits();
        info.Enabled = !traits.HasFlag(AccessibilityTrait.Disabled);
        info.Focusable = traits.HasFlag(AccessibilityTrait.Focusable);
        info.Focused = traits.HasFlag(AccessibilityTrait.Focused);
        info.Checkable = element.AccessibilityRole == AccessibilityRole.Checkbox ||
                        element.AccessibilityRole == AccessibilityRole.RadioButton ||
                        element.AccessibilityRole == AccessibilityRole.Toggle;
        info.Checked = traits.HasFlag(AccessibilityTrait.Checked);
        info.Clickable = element.AccessibilityRole == AccessibilityRole.Button ||
                        element.AccessibilityRole == AccessibilityRole.Link;
        info.Scrollable = element.AccessibilityRole == AccessibilityRole.ScrollView;

        // Set text if applicable
        var value = element.GetComputedAccessibilityValue();
        if (!string.IsNullOrEmpty(value))
        {
            info.Text = value;
        }

        // Set hint
        if (!string.IsNullOrEmpty(element.AccessibilityHint))
        {
            info.TooltipText = element.AccessibilityHint;
        }

        // Set visible
        info.VisibleToUser = element.IsVisible && element.IsAccessibilityElement;

        return info;
    }

    public override bool PerformAction(int virtualViewId, Android.Views.Accessibility.Action action, Android.OS.Bundle? arguments)
    {
        // Handle accessibility actions (click, scroll, etc.)
        var element = FindElementByVirtualId(virtualViewId);
        if (element == null)
        {
            return false;
        }

        switch (action)
        {
            case Android.Views.Accessibility.Action.Click:
                // Trigger click on the element
                return true;

            case Android.Views.Accessibility.Action.AccessibilityFocus:
                _focusedVirtualViewId = virtualViewId;
                return true;

            case Android.Views.Accessibility.Action.ClearAccessibilityFocus:
                if (_focusedVirtualViewId == virtualViewId)
                {
                    _focusedVirtualViewId = Android.Views.View.NoId;
                }
                return true;

            default:
                return false;
        }
    }

    private void AddVirtualChildren(AccessibilityNodeInfo parentInfo, UiElement parent)
    {
        // Add this element as a virtual child if it's an accessibility element
        if (parent.IsAccessibilityElement && parent.IsVisible)
        {
            parentInfo.AddChild(_hostView, parent.GetHashCode());
        }

        // Recursively add children
        if (parent is UiLayoutElement layoutElement)
        {
            foreach (var child in layoutElement.Children)
            {
                AddVirtualChildren(parentInfo, child);
            }
        }
    }

    private UiElement? FindElementByVirtualId(int virtualViewId)
    {
        var root = _rootProvider();
        if (root == null)
        {
            return null;
        }

        return FindElementRecursive(root, virtualViewId);
    }

    private static UiElement? FindElementRecursive(UiElement element, int virtualViewId)
    {
        if (element.GetHashCode() == virtualViewId)
        {
            return element;
        }

        if (element is UiLayoutElement layoutElement)
        {
            foreach (var child in layoutElement.Children)
            {
                var found = FindElementRecursive(child, virtualViewId);
                if (found != null)
                {
                    return found;
                }
            }
        }

        return null;
    }

    private static string GetAccessibilityClassName(AccessibilityRole role)
    {
        return role switch
        {
            AccessibilityRole.Button => "android.widget.Button",
            AccessibilityRole.Checkbox => "android.widget.CheckBox",
            AccessibilityRole.RadioButton => "android.widget.RadioButton",
            AccessibilityRole.TextInput => "android.widget.EditText",
            AccessibilityRole.Label => "android.widget.TextView",
            AccessibilityRole.Heading => "android.widget.TextView",
            AccessibilityRole.Link => "android.widget.TextView",
            AccessibilityRole.Image => "android.widget.ImageView",
            AccessibilityRole.Slider => "android.widget.SeekBar",
            AccessibilityRole.Toggle => "android.widget.Switch",
            AccessibilityRole.List => "android.widget.ListView",
            AccessibilityRole.ListItem => "android.widget.TextView",
            AccessibilityRole.ComboBox => "android.widget.Spinner",
            AccessibilityRole.ProgressBar => "android.widget.ProgressBar",
            AccessibilityRole.ScrollView => "android.widget.ScrollView",
            AccessibilityRole.Container => "android.view.ViewGroup",
            AccessibilityRole.Dialog => "android.app.Dialog",
            AccessibilityRole.Alert => "android.app.AlertDialog",
            AccessibilityRole.Toolbar => "android.widget.Toolbar",
            AccessibilityRole.Menu => "android.widget.PopupMenu",
            AccessibilityRole.MenuItem => "android.view.MenuItem",
            AccessibilityRole.Tab => "android.widget.TabWidget",
            AccessibilityRole.TabPanel => "android.widget.TabHost",
            AccessibilityRole.DatePicker => "android.widget.DatePicker",
            AccessibilityRole.TimePicker => "android.widget.TimePicker",
            AccessibilityRole.Spinner => "android.widget.ProgressBar",
            AccessibilityRole.Tooltip => "android.widget.Toast",
            _ => "android.view.View"
        };
    }
}
