

using WebUı.Models;

public class HomeViewModel
{
    public List<Category> Categories { get; set; }
    public Dictionary<int, List<News>> NewsByCategory { get; set; }
    public List<News> MostViewedNews { get; set; }
}

public class CategoryNewsViewModel
{
    public Category Category { get; set; }
    public List<News> News { get; set; }
} 