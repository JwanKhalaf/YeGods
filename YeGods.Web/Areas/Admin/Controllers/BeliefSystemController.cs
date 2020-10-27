namespace YeGods.Web.Areas.Admin.Controllers
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore.Storage;
  using Services;
  using ViewModels;
  using YeGods.ViewModels.Shared;

  [Area("Admin")]
  [Route("admin/beliefsystem")]
  public class BeliefSystemController : Controller
  {
    private readonly IBeliefSystemService beliefSystemService;

    public BeliefSystemController(IBeliefSystemService beliefSystemService)
    {
      this.beliefSystemService = beliefSystemService;
    }

    [Route("[action]")]
    public async Task<IActionResult> Page(SearchViewModel search, int page = 1)
    {
      BeliefSystemPageViewModel deities = await this.beliefSystemService
        .GetPagedBeliefSystemsAsync(search, page);

      return this.View(deities);
    }

    [Route("[action]")]
    public IActionResult Create()
    {
      BeliefSystemCreateViewModel newBeliefSystem = new BeliefSystemCreateViewModel();

      return this.View(newBeliefSystem);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Create(BeliefSystemCreateViewModel newBeliefSystem)
    {
      if (!this.ModelState.IsValid)
      {
        return this.View(newBeliefSystem);
      }

      await this.beliefSystemService.CreateNewBeliefSystem(newBeliefSystem);

      return this.Redirect("Page");
    }

    [Route("[action]")]
    public async Task<IActionResult> Edit(int id)
    {
      BeliefSystemUpdateViewModel beliefSystemToUpdate = await this.beliefSystemService
        .GetBeliefSystemByIdForUpdateAsync(id);

      return this.View(beliefSystemToUpdate);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Edit(BeliefSystemUpdateViewModel updatedBeliefSystem)
    {
      if (!this.ModelState.IsValid)
      {
        return this.View(updatedBeliefSystem);
      }

      await this.beliefSystemService.UpdateBeliefSystemAsync(updatedBeliefSystem);
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

      BeliefSystemViewModel viewModel = await this.beliefSystemService.GetBeliefSystemByIdAsync(id.Value);

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
        await this.beliefSystemService.DeleteBeliefSystemAsync(id);
      }
      catch (RetryLimitExceededException)
      {
        return this.RedirectToAction("Delete", new { id, saveChangesError = true });
      }

      return this.RedirectToAction("Page", new { showDeleted = false });
    }
  }
}
