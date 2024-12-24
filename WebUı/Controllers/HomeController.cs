using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebUı.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WebUı.ViewModel;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using WebUı.Repositories;

namespace WebUı.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NewsRepository _newsRepository;
        private readonly CategoryRepository _categoryRepository;

        public HomeController(
            ILogger<HomeController> logger,
            NewsRepository newsRepository,
            CategoryRepository categoryRepository)
        {
            _logger = logger;
            _newsRepository = newsRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            
            

            var latestNewsByCategory = await _newsRepository.GetLatestNewsByCategories(3);
            
        
            var mostViewedNews = await _newsRepository.GetMostViewedActiveNews(4);
            
          

            var viewModel = new HomeViewModel
            {
                Categories = categories,
                NewsByCategory = latestNewsByCategory,
                MostViewedNews = mostViewedNews
            };

            return View(viewModel);
        }

        public async Task<IActionResult> CategoryNews(int categoryId)
        {
            var news = await _newsRepository.GetActiveNewsByCategory(categoryId);
            var category = await _categoryRepository.GetByIdAsync(categoryId);

            if (category == null)
                return NotFound();

            var viewModel = new CategoryNewsViewModel
            {
                Category = category,
                News = news
            };

            return View(viewModel);
        }

        public async Task<IActionResult> NewsDetail(int id)
        {
            var news = await _newsRepository.GetNewsDetail(id);

            if (news == null)
                return NotFound();

            // Görüntüleme sayısını artır
            await _newsRepository.IncrementViewCount(id);

            return View(news);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
