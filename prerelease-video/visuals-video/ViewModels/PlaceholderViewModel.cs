using CommunityToolkit.Mvvm.ComponentModel;

namespace PrereleaseVideo.ViewModels;

public partial class PlaceholderViewModel : ObservableObject
{
    public string SectionTitle { get; set; } = "Section";
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(15);
}
