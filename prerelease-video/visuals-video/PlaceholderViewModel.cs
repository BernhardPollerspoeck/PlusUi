using CommunityToolkit.Mvvm.ComponentModel;

public partial class PlaceholderViewModel : ObservableObject
{
    public string SectionTitle { get; set; } = "Section";
    public TimeSpan StartTime { get; set; }
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(5);
}
