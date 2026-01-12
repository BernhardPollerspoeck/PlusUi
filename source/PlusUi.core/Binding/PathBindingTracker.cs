using System.ComponentModel;
using System.Reflection;

namespace PlusUi.core.Binding;

internal class PathBindingTracker(string[] pathSegments, Action updateAction) : IDisposable
{
    private readonly List<(INotifyPropertyChanged obj, PropertyChangedEventHandler handler)> _subscriptions = [];
    private object? _rootContext;
    private bool _disposed;

    public void SetContext(object? context)
    {
        UnsubscribeAll();
        _rootContext = context;
        SubscribeToPath();
        updateAction();
    }

    private void SubscribeToPath()
    {
        if (pathSegments.Length == 0 || _rootContext == null)
            return;

        object? current = _rootContext;

        for (int i = 0; i < pathSegments.Length; i++)
        {
            if (current == null)
                break;

            if (current is INotifyPropertyChanged notifier)
            {
                var segmentIndex = i;
                var isLeaf = i == pathSegments.Length - 1;

                void handler(object? s, PropertyChangedEventArgs e)
                {
                    if (e.PropertyName == pathSegments[segmentIndex])
                    {
                        if (isLeaf)
                        {
                            updateAction();
                        }
                        else
                        {
                            ResubscribeFromIndex(segmentIndex + 1);
                            updateAction();
                        }
                    }
                }

                notifier.PropertyChanged += handler;
                _subscriptions.Add((notifier, handler));
            }

            if (i < pathSegments.Length - 1)
            {
                current = GetPropertyValue(current, pathSegments[i]);
            }
        }
    }

    private void ResubscribeFromIndex(int index)
    {
        for (int i = _subscriptions.Count - 1; i >= index; i--)
        {
            var (obj, handler) = _subscriptions[i];
            obj.PropertyChanged -= handler;
            _subscriptions.RemoveAt(i);
        }

        if (index >= pathSegments.Length)
            return;

        object? current = _rootContext;
        for (int i = 0; i < index && current != null; i++)
        {
            current = GetPropertyValue(current, pathSegments[i]);
        }

        if (current == null)
            return;

        for (int i = index; i < pathSegments.Length; i++)
        {
            if (current == null)
                break;

            if (current is INotifyPropertyChanged notifier)
            {
                var segmentIndex = i;
                var isLeaf = i == pathSegments.Length - 1;

                void handler(object? s, PropertyChangedEventArgs e)
                {
                    if (e.PropertyName == pathSegments[segmentIndex])
                    {
                        if (isLeaf)
                        {
                            updateAction();
                        }
                        else
                        {
                            ResubscribeFromIndex(segmentIndex + 1);
                            updateAction();
                        }
                    }
                }

                notifier.PropertyChanged += handler;
                _subscriptions.Add((notifier, handler));
            }

            if (i < pathSegments.Length - 1)
            {
                current = GetPropertyValue(current, pathSegments[i]);
            }
        }
    }

    private void UnsubscribeAll()
    {
        foreach (var (obj, handler) in _subscriptions)
        {
            obj.PropertyChanged -= handler;
        }
        _subscriptions.Clear();
    }

    private static object? GetPropertyValue(object? obj, string propertyName)
    {
        if (obj == null)
            return null;

        var property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        return property?.GetValue(obj);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        UnsubscribeAll();
        _disposed = true;
    }
}
