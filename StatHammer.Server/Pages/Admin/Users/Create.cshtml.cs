using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StatHammer.Server.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace StatHammer.Server.Pages.Admin.Users
{
    public class CreateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public CreateUserInputModel Input { get; set; } = new();

        public string? SuccessMessage { get; set; }

        public string? ErrorMessage { get; set; }

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
                    ErrorMessage = "Не удалось создать роль User: " +
                                   string.Join("; ", roleResult.Errors.Select(e => e.Description));
                    return Page();
                }
            }

            var existingUser = await _userManager.FindByEmailAsync(Input.Email);

            if (existingUser != null)
            {
                ErrorMessage = "Пользователь с таким email уже существует.";
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
                ErrorMessage = "Не удалось создать пользователя: " +
                               string.Join("; ", createResult.Errors.Select(e => e.Description));
                return Page();
            }

            var roleAssignResult = await _userManager.AddToRoleAsync(user, userRoleName);

            if (!roleAssignResult.Succeeded)
            {
                ErrorMessage = "Пользователь создан, но не удалось назначить роль User: " +
                               string.Join("; ", roleAssignResult.Errors.Select(e => e.Description));
                return Page();
            }

            SuccessMessage = $"Пользователь {Input.Email} создан с ролью User.";
            ModelState.Clear();
            Input = new CreateUserInputModel();

            return Page();
        }

        public class CreateUserInputModel
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
