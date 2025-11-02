using Android.Content;
using PlusUi.core.Services;

namespace PlusUi.droid;

/// <summary>
/// Android implementation of URL launcher service
/// </summary>
public class UrlLauncherService : IUrlLauncherService
{
    private readonly Context _context;

    public UrlLauncherService(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public bool OpenUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return false;
        }

        try
        {
            var uri = Android.Net.Uri.Parse(url);
            var intent = new Intent(Intent.ActionView, uri);

            // Add FLAG_ACTIVITY_NEW_TASK if we're not in an Activity context
            intent.AddFlags(ActivityFlags.NewTask);

            _context.StartActivity(intent);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
