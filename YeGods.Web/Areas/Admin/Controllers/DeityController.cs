namespace YeGods.Web.Areas.Admin.Controllers
{
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.Rendering;
  using Microsoft.EntityFrameworkCore.Storage;
  using Services;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using ViewModels;
  using ViewModels.Shared;

  [Area("Admin")]
  [Route("admin/deity")]
  [Authorize]
  public class DeityController : Controller
  {
    private readonly IDeityService deityService;
    private readonly ICategoryService categoryService;

    public DeityController(IDeityService deityService, ICategoryService categoryService)
    {
      this.deityService = deityService;
      this.categoryService = categoryService;
    }

    [Route("[action]")]
    public async Task<IActionResult> Page(SearchViewModel search, int page = 1)
    {
      DeityPageViewModel deities = await deityService
        .GetPagedDeitiesAsync(search, page);

      return View(deities);
    }

    [Route("[action]")]
    public async Task<IActionResult> Create()
    {
      DeityCreateViewModel newDeity = new DeityCreateViewModel();

      newDeity.Categories = await GetCategories();

      return View(newDeity);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Create(DeityCreateViewModel newDeity)
    {
      if (!ModelState.IsValid)
      {
        newDeity.Categories = await GetCategories();

        return View(newDeity);
      }

      await deityService.CreateNewDeity(newDeity);

      return Redirect("Page");
    }

    [Route("[action]")]
    public async Task<IActionResult> Edit(int id)
    {
      DeityUpdateViewModel deityToUpdate = await deityService.GetDeityByIdForUpdateAsync(id);

      deityToUpdate.Categories = await GetCategories();

      return View(deityToUpdate);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Edit(DeityUpdateViewModel updatedDeity)
    {
      if (!ModelState.IsValid)
      {
        updatedDeity.Categories = await GetCategories();

        return View(updatedDeity);
      }

      await deityService.UpdateDeityAsync(updatedDeity);

      return Redirect("Page");
    }

    public async Task<IActionResult> Delete(
      int? id,
      bool? saveChangesError = false)
    {
      if (id == null) return BadRequest();

      if (saveChangesError.GetValueOrDefault())
      {
        ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
      }

      DeityViewModel viewModel = await deityService.GetDeityByIdAsync(id.Value);

      if (viewModel == null) return NotFound();

      return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
      if (id == 0) return BadRequest();

      try
      {
        await deityService.DeleteDeityAsync(id);
      }
      catch (RetryLimitExceededException)
      {
        return RedirectToAction("Delete", new { id, saveChangesError = true });
      }

      return RedirectToAction("Page", new { showDeleted = false });
    }

    private async Task<List<SelectListItem>> GetCategories()
    {
      List<CategoryViewModel> categories = await categoryService.GetAllCategories();

      return categories.Select(s => new SelectListItem
      {
        Text = s.Name,
        Value = s.Id.ToString()
      }).ToList();
    }
  }
}
