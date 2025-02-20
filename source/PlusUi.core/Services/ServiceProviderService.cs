namespace PlusUi.core;

internal class ServiceProviderService
{
    public static IServiceProvider? ServiceProvider { get; private set; }

    public ServiceProviderService(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}