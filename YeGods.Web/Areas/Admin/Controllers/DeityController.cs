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
  using YeGods.ViewModels.Shared;

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
      DeityPageViewModel deities = await this.deityService.GetPagedDeitiesAsync(search, page);
      return this.View(deities);
    }

    [Route("[action]")]
    public async Task<IActionResult> Create()
    {
      DeityCreateViewModel newDeity = new DeityCreateViewModel();
      newDeity.Categories = await this.GetCategories();
      return this.View(newDeity);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Create(DeityCreateViewModel newDeity)
    {
      if (!this.ModelState.IsValid)
      {
        newDeity.Categories = await this.GetCategories();
        return this.View(newDeity);
      }

      await this.deityService.CreateNewDeity(newDeity);
      return this.Redirect("Page");
    }

    [Route("[action]")]
    public async Task<IActionResult> Edit(int id)
    {
      DeityUpdateViewModel deityToUpdate = await this.deityService.GetDeityByIdForUpdateAsync(id);
      deityToUpdate.Categories = await this.GetCategories();
      return this.View(deityToUpdate);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Edit(DeityUpdateViewModel updatedDeity)
    {
      if (!this.ModelState.IsValid)
      {
        updatedDeity.Categories = await this.GetCategories();
        return this.View(updatedDeity);
      }

      await this.deityService.UpdateDeityAsync(updatedDeity);
      return this.Redirect("Page");
    }

    public async Task<IActionResult> Delete(
      int? id,
      bool? saveChangesError = false)
    {
      if (id == null) return this.BadRequest();

      if (saveChangesError.GetValueOrDefault())
      {
        this.ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
      }

      DeityViewModel viewModel = await this.deityService.GetDeityByIdAsync(id.Value);

      if (viewModel == null) return this.NotFound();

      return this.View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
      if (id == 0) return this.BadRequest();

      try
      {
        await this.deityService.DeleteDeityAsync(id);
      }
      catch (RetryLimitExceededException)
      {
        return this.RedirectToAction("Delete", new { id, saveChangesError = true });
      }

      return this.RedirectToAction("Page", new { showDeleted = false });
    }

    private async Task<List<SelectListItem>> GetCategories()
    {
      List<CategoryViewModel> categories = await this.categoryService.GetAllCategories();
      return categories.Select(s => new SelectListItem
      {
        Text = s.Name,
        Value = s.Id.ToString()
      }).ToList();
    }
  }
}
