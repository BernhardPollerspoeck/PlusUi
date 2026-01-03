namespace Sandbox.Pages.TreeViewDemo;

public class Folder
{
    public string Name { get; set; } = string.Empty;
    public List<Folder> SubFolders { get; set; } = new();
    public List<FileItem> Files { get; set; } = new();
}
