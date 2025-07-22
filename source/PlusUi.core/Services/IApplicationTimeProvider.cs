namespace PlusUi.core.Services;

public interface IApplicationTimeProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    TimeSpan Elapsed { get; }
}
