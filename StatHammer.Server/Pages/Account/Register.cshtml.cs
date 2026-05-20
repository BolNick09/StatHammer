using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.Models.Entities;

namespace StatHammer.Server.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            const string userRoleName = "User";

            if (!await _roleManager.RoleExistsAsync(userRoleName))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(userRoleName));

                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return Page();
                }
            }

            var existingUser = await _userManager.FindByEmailAsync(Input.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Пользователь с таким email уже существует.");
                return Page();
            }

            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, Input.Password);

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            var roleResultAssign = await _userManager.AddToRoleAsync(user, userRoleName);

            if (!roleResultAssign.Succeeded)
            {
                foreach (var error in roleResultAssign.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToPage("/Simulations/Index");
        }

        public class RegisterInputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required]
            [MinLength(6)]
            [Display(Name = "Пароль")]
            public string Password { get; set; } = string.Empty;

            [Required]
            [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают.")]
            [Display(Name = "Подтверждение пароля")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }
    }
}