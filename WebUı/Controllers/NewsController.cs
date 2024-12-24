using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebUı.Models;
using WebUı.Repositories;

namespace WebUı.Controllers
{

    [Authorize]
    public class NewsController : Controller
    {
        private readonly NewsRepository _newsRepository;
        private readonly INotyfService _notyfService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppDbContext _context;
        private readonly IHubContext<NewsHub> _hubContext;

        public NewsController(NewsRepository newsRepository, INotyfService notyfService, IWebHostEnvironment webHostEnvironment, AppDbContext context, IHubContext<NewsHub> hubContext)
        {
            _newsRepository = newsRepository;
            _notyfService = notyfService;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var news = await _newsRepository.GetAllNewsWithDetails();
            return View(news);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(News news, IFormFile image)
        {
            if (image != null)
            {
                news.ImagePath = await SaveImage(image);
            }

            news.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            news.CreatedDate = DateTime.Now;

            // Admin veya Gazeteci ise otomatik onaylı olarak ekle
            if (User.IsInRole("Admin") || User.IsInRole("Gazetici"))
            {
                news.Status = NewsStatus.Approved;
                news.ApprovalDate = DateTime.Now;
                news.ApprovedByUserId = news.UserId; // Kendisi onaylamış olur
                news.IsActive = true;
            }
            else
            {
                news.Status = NewsStatus.Pending;
                news.IsActive = false;
            }

            await _newsRepository.AddAsync(news);
            _notyfService.Success("Haber başarıyla eklendi");

            ViewBag.Categories = await _context.Categories.ToListAsync();

            // Normal kullanıcı eklediyse Admin ve Gazetecilere bildirim gönder
            if (news.Status == NewsStatus.Pending)
            {
                await _hubContext.Clients.Groups(new List<string> { "Admins", "Journalists" })
                    .SendAsync("ReceiveNewsNotification", news.Title, news.Id);
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var news = await _newsRepository.GetByIdAsync(id);
            if (news == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(news);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(News news, IFormFile? image)
        {
            var existingNews = await _newsRepository.GetByIdAsync(news.Id);
            if (existingNews == null)
            {
                return NotFound();
            }

            if (image != null)
            {
                DeleteImage(existingNews.ImagePath);
                existingNews.ImagePath = await SaveImage(image);
            }

            existingNews.Title = news.Title;
            existingNews.Content = news.Content;
            existingNews.CategoryId = news.CategoryId;
            existingNews.IsActive = news.IsActive;
            existingNews.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            existingNews.CreatedDate = DateTime.Now;

            // Admin veya Gazeteci ise onay durumunu güncelle
            if (User.IsInRole("Admin") || User.IsInRole("Gazetici"))
            {
                // Eğer haber daha önce onaylanmamışsa
                if (existingNews.Status != NewsStatus.Approved)
                {
                    existingNews.Status = NewsStatus.Approved;
                    existingNews.ApprovalDate = DateTime.Now;
                    existingNews.ApprovedByUserId = existingNews.UserId;
                }
                existingNews.IsActive = news.IsActive;
            }
            else
            {
                // Normal kullanıcı düzenliyorsa tekrar onay sürecine girer
                existingNews.Status = NewsStatus.Pending;
                existingNews.ApprovalDate = null;
                existingNews.ApprovedByUserId = null;
                existingNews.IsActive = false;

                // Admin ve Gazetecilere bildirim gönder
                await _hubContext.Clients.Groups(new List<string> { "Admins", "Journalists" })
                    .SendAsync("ReceiveNewsNotification", news.Title, news.Id);
            }

            await _newsRepository.UpdateAsync(existingNews);
            _notyfService.Success("Haber başarıyla güncellendi");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var news = await _newsRepository.GetByIdAsync(id);
            if (news == null)
            {
                return Json(new { success = false, message = "Haber bulunamadı" });
            }

            DeleteImage(news.ImagePath);
            await _newsRepository.DeleteAsync(id);

            return Json(new { success = true, message = "Haber başarıyla silindi" });
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string uploadDir = Path.Combine(wwwRootPath, "images", "news");

            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            string filePath = Path.Combine(uploadDir, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return "/images/news/" + fileName;
        }

        private void DeleteImage(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetNews()
        {
            try
            {
                var news = await _newsRepository.GetAllNewsWithDetails();
                var newsData = news.Select(n => new
                {
                    id = n.Id,
                    title = n.Title,
                    imagePath = n.ImagePath ?? "/images/default.jpg",
                    category = new { name = n.Category?.Name ?? "Kategorisiz" },
                    viewCount = n.ViewCount,
                    isActive = n.IsActive,
                    createdDate = n.CreatedDate
                }).ToList();

                return Json(new { data = newsData });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyNews()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var news = await _newsRepository.GetNewsByUserId(userId);
            return View(news);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Gazetici")]
        public async Task<IActionResult> ApproveNews(int id)
        {
            var news = await _newsRepository.GetByIdAsync(id);
            if (news == null) return NotFound();

            // Onaylama bilgilerini güncelle
            news.Status = NewsStatus.Approved;
            news.ApprovalDate = DateTime.Now;
            news.ApprovedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            news.RejectionReason = null; // Varsa red sebebini temizle

            await _newsRepository.UpdateAsync(news);

            // Haber sahibine bildirim gönder
            await _hubContext.Clients.User(news.UserId)
                .SendAsync("ReceiveApprovalNotification", news.Title, "onaylandı");

            return Json(new { success = true });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Gazetici")]
        public async Task<IActionResult> RejectNews(int id, string reason)
        {
            var news = await _newsRepository.GetByIdAsync(id);
            if (news == null) return NotFound();

            // Red bilgilerini güncelle
            news.Status = NewsStatus.Rejected;
            news.RejectionReason = reason;
            news.ApprovalDate = null;
            news.ApprovedByUserId = null;

            await _newsRepository.UpdateAsync(news);

            // Haber sahibine bildirim gönder
            await _hubContext.Clients.User(news.UserId)
                .SendAsync("ReceiveApprovalNotification", news.Title, "reddedildi");

            return Json(new { success = true });
        }

        [Authorize(Roles = "Admin,Gazetici")]
        public async Task<IActionResult> PendingNews()
        {
            var pendingNews = await _newsRepository.GetPendingNews();
            return View(pendingNews);
        }

        [Authorize(Roles ="User")]
        [HttpGet]
        public async Task<IActionResult> UserAddNews()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UserAddNews(News news, IFormFile image)
        {
           
                if (image != null)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "images", uniqueFileName);
                    
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    
                    news.ImagePath = "/images/" + uniqueFileName;
                }

                news.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                news.CreatedDate = DateTime.Now;
                news.Status = NewsStatus.Pending;
                news.IsActive = true;

                await _newsRepository.AddAsync(news);

                // Haber eklendiğinde Admin ve Gazetecilere bildirim gönder
                await _hubContext.Clients.Groups(new List<string> { "Admins", "Journalists" })
                    .SendAsync("ReceiveNewsNotification", news.Title, news.Id);

                _notyfService.Success("Haberiniz başarıyla gönderildi ve onay için bekliyor.");
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return RedirectToAction("MyNews","News");
            

       
  
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Gazetici")]
        public async Task<IActionResult> GetPendingNews()
        {
            try
            {
                var pendingNews = await _newsRepository.GetPendingNews();
                var newsData = pendingNews.Select(n => new
                {
                    id = n.Id,
                    title = n.Title,
                    imagePath = n.ImagePath ?? "/images/default.jpg",
                    createdDate = n.CreatedDate,
                    user = new { userName = n.User?.UserName ?? "Bilinmiyor" },
                    category = new { name = n.Category?.Name ?? "Kategorisiz" }
                }).ToList();

                return Json(new { 
                    data = newsData,
                    recordsTotal = newsData.Count,
                    recordsFiltered = newsData.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    data = new List<object>(),
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    error = ex.Message 
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Gazetici")]
        public async Task<IActionResult> Preview(int id)
        {
            var news = await _context.News
                .Include(n => n.User)
                .Include(n => n.Category)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (news == null) return NotFound();

            return View(news);
        }
    }
}
