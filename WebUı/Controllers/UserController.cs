using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebUı.Models;
using WebUı.Repositories;
using WebUı.ViewModel;

namespace WebUı.Controllers
{
    public class UserController : Controller
    {
        private readonly INotyfService _notyfService;
        private readonly UserRepository _userRepository;

        public UserController(INotyfService notyfService, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _notyfService = notyfService;
            _userRepository = new UserRepository(userManager, signInManager);
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _userRepository.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        var roles = await _userRepository.GetUserRolesAsync(user);
                        if (roles.Contains("Admin"))
                        {
                            _notyfService.Success("Giriş Başarılı");
                            return RedirectToAction("Index", "Admin");
                        }
                        else if (roles.Contains("Gazetici"))
                        {
                            _notyfService.Success("Giriş Başarılı");
                            return RedirectToAction("Index", "Admin");
                        }
                        else
                        {
                            _notyfService.Success("Giriş Başarılı");
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                _notyfService.Warning("Email veya şifre hatalı");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)
             {
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true
                };

                var result = await _userRepository.CreateUserAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userRepository.AddToRoleAsync(user, "User");
                    await _userRepository.SignInAsync(user, false);
                    _notyfService.Success("Kayıt işlemi başarılı bir şekilde gerçekleşti.");
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    _notyfService.Warning(error.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userRepository.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
