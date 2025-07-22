namespace PlusUi.core.Services;


public class ApplicationTimeProvider(TimeProvider timeProvider) : IApplicationTimeProvider
{
    internal DateTime UtcStartTime { get; set; }
    public DateTime Now => timeProvider.GetLocalNow().DateTime;
    public DateTime UtcNow => timeProvider.GetUtcNow().DateTime;
    public TimeSpan Elapsed => timeProvider.GetUtcNow().DateTime - UtcStartTime;

}
