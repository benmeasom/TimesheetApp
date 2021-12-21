using DataObjects.DTO;
using DataObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TimesheetApp.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<TimesheetUser> _userManager;
        private readonly SignInManager<TimesheetUser> _signInManager;
        private readonly ILogger<LoginDto> _logger;

        public AccountController(
                UserManager<TimesheetUser> userManager,
                SignInManager<TimesheetUser> signInManager,
                ILogger<LoginDto> logger
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string ReturnUrl)
        {
            return View();
        }
       
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto data)
        {
            if(string.IsNullOrEmpty(data.Email))
                {
                ModelState.AddModelError("Email", "Please enter email.");
                return View();
            }
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var user = await _userManager.FindByEmailAsync(data.Email);
                if(user == null || user.IsDisabled)
                {
                    ModelState.AddModelError("Email", "User doesn't exists.");
                    return View();
                }
                
                var result = await _signInManager.PasswordSignInAsync(data.Email, data.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    user = await _userManager.FindByEmailAsync(data.Email);
                    if(!_userManager.GetClaimsAsync(user).Result.Any(a=>a.Type == "FullName"))
                    {
                        await _userManager.AddClaimAsync(user, new Claim("FullName", $"{user.FirstName} {user.LastName}"));
                        await _signInManager.RefreshSignInAsync(user);
                    }

                    _logger.LogInformation("User logged in.");
                    return RedirectToAction("Timesheet", "Timesheet");
                }
                ModelState.AddModelError("Password", "Invalid password.");
                return View();
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto data)
        {
            if (ModelState.IsValid)
            {
                var user = new TimesheetUser { FirstName = data.FirstName, LastName = data.LastName, UserName = data.Email, Email = data.Email };
                var result = await _userManager.CreateAsync(user, data.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    _logger.LogInformation("User created a new account with password.");
                    return RedirectToAction("Login");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View();
        }
       
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
