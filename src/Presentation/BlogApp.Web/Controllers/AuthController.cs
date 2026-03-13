using BlogApp.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        //private readonly RoleManager<IdentityUser> _roleManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;


        //Register
        //Login
        //LogOut

        public AuthController(UserManager<IdentityUser> userManger, RoleManager<IdentityRole> roleManager
            , SignInManager<IdentityUser> signInManager
            )
        {
            _userManager = userManger;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email

                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("User"))
                    {

                        await _roleManager.CreateAsync(new IdentityRole("User"));


                    }
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Post");

                }



            }
            return View(model);
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
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "Email or Password is Incorrect");
                    return View(model);
                }

               
                if (await _userManager.IsLockedOutAsync(user))
                {
                    ModelState.AddModelError("", "Your account is locked. Please contact admin.");
                    return View(model);
                }

                var signinResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, true);

                if (signinResult.IsLockedOut)
                {
                    ModelState.AddModelError("", "Your account is locked. Please contact admin.");
                    return View(model);
                }

                if (!signinResult.Succeeded)
                {
                    ModelState.AddModelError("", "Email or Password is Incorrect");
                    return View(model);
                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]


        public IActionResult AccessDenied()
        {

            return View();

        }

    }
}
