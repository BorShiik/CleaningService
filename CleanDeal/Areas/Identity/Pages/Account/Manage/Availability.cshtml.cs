using System.Security.Claims;
using CleanDeal.Model.Models;
using CleanDeal.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CleanDeal.Areas.Identity.Pages.Account.Manage;

[Authorize(Roles = "Cleaner")]
public class AvailabilityModel : PageModel
{
    private readonly IAvailabilityRepository _repo;
    private readonly UserManager<ApplicationUser> _um;

    public IList<Availability> Availabilities { get; private set; } = new List<Availability>();

    [BindProperty]
    public Availability NewItem { get; set; }

    public AvailabilityModel(IAvailabilityRepository repo, UserManager<ApplicationUser> um)
    {
        _repo = repo;
        _um = um;
    }

    public async Task OnGetAsync()
    {
        var id = _um.GetUserId(User)!;
        Availabilities = await _repo.GetForCleanerAsync(id);

        NewItem = new Availability { Date = DateTime.Today };
    }

    public async Task<IActionResult> OnPostAddAsync()
    {
        var raw = Request.Form["NewItem.Date"].FirstOrDefault();
        Console.WriteLine($"Posted date string: {raw}");
        Console.WriteLine($"Parsed DateTime   : {NewItem?.Date:o}");

        if (!ModelState.IsValid)
        {
            foreach (var e in ModelState.Values.SelectMany(v => v.Errors))
                Console.WriteLine(e.ErrorMessage);
            return await ReloadAsync();
        }

        NewItem.CleanerId = _um.GetUserId(User)!;
        await _repo.AddAsync(NewItem);
        return RedirectToPage();
    }


    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _repo.DeleteAsync(id);
        return RedirectToPage();
    }

    private async Task<IActionResult> ReloadAsync()
    {
        await OnGetAsync();
        return Page();
    }
}