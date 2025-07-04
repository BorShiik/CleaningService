using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using CleanDeal.Model.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

public class RegisterModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IEmailSender _emailSender;

    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<RegisterModel> logger,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _logger = logger;
        _emailSender = emailSender;
    }

    public class InputModel
    {
        [Required, Display(Name = "Imię i nazwisko")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Płeć")]
        public Gender? Gender { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone, Display(Name = "Telefon")]
        public string DialCode { get; set; } = "+48";
        public string? PhoneNumber { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Hasła nie są takie same")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string SelectedRole { get; set; } = "Client";
    }

    [BindProperty] public InputModel Input { get; set; }

    public string? ReturnUrl { get; set; }

    public void OnGet(string? returnUrl = null, string role = "Client")
    {
        ReturnUrl = returnUrl;
        Input = new InputModel { SelectedRole = role };
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        if (!ModelState.IsValid) return Page();

        var user = new ApplicationUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            FullName = Input.FullName,
            Gender = Input.Gender,
            PhoneNumber = Input.PhoneNumber
        };
        var result = await _userManager.CreateAsync(user, Input.Password);
        if (!result.Succeeded)
        {
            foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e.Description);
            return Page();
        }

        if (!await _roleManager.RoleExistsAsync(Input.SelectedRole))
            await _roleManager.CreateAsync(new IdentityRole(Input.SelectedRole));

        await _userManager.AddToRoleAsync(user, Input.SelectedRole);

        _logger.LogInformation("Nowy użytkownik utworzył konto z rolą {Role}.", Input.SelectedRole);

        await _signInManager.SignInAsync(user, isPersistent: false);
        return LocalRedirect(returnUrl);
    }
}
