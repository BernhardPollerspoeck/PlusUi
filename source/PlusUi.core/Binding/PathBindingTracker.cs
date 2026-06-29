using System.ComponentModel;

namespace PlusUi.core.Binding;

internal class PathBindingTracker(BindingPath path, Action updateAction) : IDisposable
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
        if (path.Segments.Length == 0 || _rootContext == null)
            return;

        object? current = _rootContext;

        for (int i = 0; i < path.Segments.Length; i++)
        {
            if (current == null)
                break;

            if (current is INotifyPropertyChanged notifier)
            {
                var segmentIndex = i;
                var isLeaf = i == path.Segments.Length - 1;

                void handler(object? s, PropertyChangedEventArgs e)
                {
                    if (e.PropertyName == path.Segments[segmentIndex])
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

            if (i < path.Segments.Length - 1)
            {
                current = GetSegmentValue(current, i);
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

        if (index >= path.Segments.Length)
            return;

        object? current = _rootContext;
        for (int i = 0; i < index && current != null; i++)
        {
            current = GetSegmentValue(current, i);
        }

        if (current == null)
            return;

        for (int i = index; i < path.Segments.Length; i++)
        {
            if (current == null)
                break;

            if (current is INotifyPropertyChanged notifier)
            {
                var segmentIndex = i;
                var isLeaf = i == path.Segments.Length - 1;

                void handler(object? s, PropertyChangedEventArgs e)
                {
                    if (e.PropertyName == path.Segments[segmentIndex])
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

            if (i < path.Segments.Length - 1)
            {
                current = GetSegmentValue(current, i);
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

    /// <summary>
    /// Reads the value of segment <paramref name="segmentIndex"/> from <paramref name="obj"/> using the
    /// expression-derived accessor. Reflection-free, so it stays Native-AOT / trimming safe.
    /// Returns null when the segment has no accessor (e.g. complex expressions) or the object type
    /// does not match, mirroring the previous reflection-based "property not found" behavior.
    /// </summary>
    private object? GetSegmentValue(object? obj, int segmentIndex)
    {
        if (obj == null)
            return null;

        var accessor = path.SegmentAccessors[segmentIndex];
        return accessor?.Invoke(obj);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        UnsubscribeAll();
        _disposed = true;
    }
}
