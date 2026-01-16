namespace Sandbox.Pages.TreeViewDemo;

public class Category
{
    public string Name { get; set; } = string.Empty;
    public List<Category> SubCategories { get; set; } = [];
    public List<Product> Products { get; set; } = [];
}
