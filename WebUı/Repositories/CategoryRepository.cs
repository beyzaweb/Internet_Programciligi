using WebUı.Models;
using Microsoft.EntityFrameworkCore;

namespace WebUı.Repositories
{
    public class CategoryRepository : GenericRepository<Category>
    {
        public CategoryRepository(AppDbContext context) : base(context, context.Categories)
        {
        }

        public IQueryable<object> GetCategoryList()
        {
            return _context.Categories.Select(x => new
            {
                x.Id,
                x.Name,
                x.Description,
                x.IsActive,
                CreatedDate = x.CreatedDate.ToString("dd/MM/yyyy HH:mm"),
                NewsCount = x.News.Count
            });
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }
    }
}
