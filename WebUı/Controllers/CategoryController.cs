using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebUı.Models;
using WebUı.Repositories;

namespace WebUı.Controllers
{
    [Authorize(Roles="Admin")]
    public class CategoryController : Controller
    {
        private readonly CategoryRepository _categoryRepository;
        private readonly INotyfService _notyfService;

        public CategoryController(AppDbContext context, INotyfService notyfService)
    {
        _categoryRepository = new CategoryRepository(context);
        _notyfService = notyfService;
    }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            
            var categories = _categoryRepository.GetCategoryList().ToList();
            return Json(new { data = categories });
            
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
             try
            {
                category.CreatedDate = DateTime.Now;
                await _categoryRepository.AddAsync(category);
                _notyfService.Success("Kategori başarıyla eklendi");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return Json(category);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory([FromBody] Category category)
        {
            try
        {
            await _categoryRepository.UpdateAsync(category);
            _notyfService.Success("Kategori başarıyla güncellendi");
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryRepository.DeleteAsync(id);
                _notyfService.Success("Kategori başarıyla silindi");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
