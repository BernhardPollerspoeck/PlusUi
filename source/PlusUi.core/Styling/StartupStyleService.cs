using Microsoft.Extensions.Hosting;

namespace PlusUi.core;

internal class StartupStyleService(
    IApplicationStyle applicationStyle,
    Style style,
#pragma warning disable CS9113 // Parameter is unread.
    ServiceProviderService _)
#pragma warning restore CS9113 // Parameter is unread.
    : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        applicationStyle.ConfigureStyle(style);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}