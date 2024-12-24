using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebUı.Models;
using WebUı.Repositories;





public class NewsRepository : GenericRepository<News>
{
    private readonly AppDbContext _context;
    public NewsRepository(AppDbContext context) : base(context, context.News)
    {
        _context = context;
    }

    public async Task<List<News>> GetAllNewsWithDetails()
    {
        return await _context.News
            .Include(n => n.Category)
            .Include(n => n.User)
            .ToListAsync();
    }

    // Kullanıcının haberlerini getiren metod
    public async Task<List<News>> GetNewsByUserId(string userId)
    {
        return await _context.News
            .Include(n => n.Category)
            .Include(n => n.User)
            .Include(n => n.ApprovedByUser)
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    // Onay bekleyen haberleri getiren metod
    public async Task<List<News>> GetPendingNews()
    {
        return await _context.News
            .Include(n => n.Category)
            .Include(n => n.User)
            .Where(n => n.Status == NewsStatus.Pending)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    // Onaylanmış haberleri getiren metod
    public async Task<List<News>> GetApprovedNews()
    {
        return await _context.News
            .Include(n => n.Category)
            .Include(n => n.User)
            .Include(n => n.ApprovedByUser)
            .Where(n => n.Status == NewsStatus.Approved && n.IsActive)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    public async Task<int> GetPendingNewsCount()
    {
        return await _context.News
            .Where(n => n.Status == NewsStatus.Pending)
            .CountAsync();
    }

    // Aktif haberleri getiren metod
    public async Task<List<News>> GetActiveNews()
    {
        return await _context.News
            .Include(n => n.Category)
            .Include(n => n.User)
            .Where(n => n.IsActive && n.Status == NewsStatus.Approved)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    // Kategoriye göre aktif haberleri getiren metod
    public async Task<List<News>> GetActiveNewsByCategory(int categoryId)
    {
        return await _context.News
            .Include(n => n.Category)
            .Include(n => n.User)
            .Where(n => n.IsActive && 
                       n.Status == NewsStatus.Approved && 
                       n.CategoryId == categoryId)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    // En çok okunan aktif haberleri getiren metod
    public async Task<List<News>> GetMostViewedActiveNews(int take = 4)
    {
        // Debug için önce tüm haberleri çekelim
        var allNews = await _context.News
            .Include(n => n.Category)
            .Include(n => n.User)
            .ToListAsync();

        // Debug bilgisi
        var activeAndApprovedNews = allNews.Where(n => n.IsActive && n.Status == NewsStatus.Approved).ToList();

        return await _context.News
            .Include(n => n.Category)
            .Include(n => n.User)
            .Where(n => n.IsActive && n.Status == NewsStatus.Approved)
            .OrderByDescending(n => n.ViewCount)
            .Take(take)
            .ToListAsync();
    }

    // Son eklenen aktif haberleri getiren metod
    //public async Task<List<News>> GetLatestActiveNews(int take = 10)
    //{
    //    return await _context.News
    //        .Include(n => n.Category)
    //        .Include(n => n.User)
    //        .Where(n => n.IsActive && n.Status == NewsStatus.Approved)
    //        .OrderByDescending(n => n.CreatedDate)
    //        .Take(take)
    //        .ToListAsync();
    //}

    // Haber detayını getiren metod
    public async Task<News> GetNewsDetail(int id)
    {
        return await _context.News
            .Include(n => n.User)
            .Include(n => n.Category)
            .FirstOrDefaultAsync(n => n.Id == id && 
                                    n.IsActive && 
                                    n.Status == NewsStatus.Approved);
    }

    // Görüntüleme sayısını artıran metod
    public async Task IncrementViewCount(int id)
    {
        var news = await _context.News.FindAsync(id);
        if (news != null)
        {
            news.ViewCount++;
            await _context.SaveChangesAsync();
        }
    }

    // Kategoriye göre son haberleri getiren metod
    public async Task<Dictionary<int, List<News>>> GetLatestNewsByCategories(int takePerCategory = 3)
    {
        // Önce tüm haberleri çekelim ve durumlarını kontrol edelim
        var allNews = await _context.News
            .Include(n => n.Category)
            .Include(n => n.User)
            .ToListAsync();

        // Debug için kontrol edelim
        var activeNewsCount = allNews.Count(n => n.IsActive);
        var approvedNewsCount = allNews.Count(n => n.Status == NewsStatus.Approved);
        var totalNewsCount = allNews.Count;

        // Filtrelenmiş haberler
        var activeNews = await _context.News
            .Include(n => n.Category)
            .Include(n => n.User)
            .Where(n => n.IsActive && n.Status == NewsStatus.Approved)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();

        return activeNews.GroupBy(n => n.CategoryId)
                        .ToDictionary(g => g.Key, g => g.Take(takePerCategory).ToList());
    }

    // En son eklenen aktif haberleri getiren metod
    public async Task<List<News>> GetLatestActiveNews(int take = 10)
    {
        return await _context.News
            .Include(n => n.Category)
            .Include(n => n.User)
            .Where(n => n.IsActive && n.Status == NewsStatus.Approved)
            .OrderByDescending(n => n.CreatedDate)
            .Take(take)
            .ToListAsync();
    }
} 