namespace Sandbox.Pages.TreeViewDemo;

public class Folder
{
    public string Name { get; set; } = string.Empty;
    public List<Folder> SubFolders { get; set; } = [];
    public List<FileItem> Files { get; set; } = [];
}
