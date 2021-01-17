namespace YeGods.Web.Areas.Admin.Controllers
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore.Storage;
  using Services;
  using ViewModels;
  using ViewModels.Shared;

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
      BeliefSystemPageViewModel deities = await beliefSystemService
        .GetPagedBeliefSystemsAsync(search, page);

      return View(deities);
    }

    [Route("[action]")]
    public IActionResult Create()
    {
      BeliefSystemCreateViewModel newBeliefSystem = new BeliefSystemCreateViewModel();

      return View(newBeliefSystem);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Create(BeliefSystemCreateViewModel newBeliefSystem)
    {
      if (!ModelState.IsValid)
      {
        return View(newBeliefSystem);
      }

      await beliefSystemService.CreateNewBeliefSystem(newBeliefSystem);

      return Redirect("Page");
    }

    [Route("[action]")]
    public async Task<IActionResult> Edit(int id)
    {
      BeliefSystemUpdateViewModel beliefSystemToUpdate = await beliefSystemService
        .GetBeliefSystemByIdForUpdateAsync(id);

      return View(beliefSystemToUpdate);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Edit(BeliefSystemUpdateViewModel updatedBeliefSystem)
    {
      if (!ModelState.IsValid)
      {
        return View(updatedBeliefSystem);
      }

      await beliefSystemService.UpdateBeliefSystemAsync(updatedBeliefSystem);
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

      BeliefSystemViewModel viewModel = await beliefSystemService.GetBeliefSystemByIdAsync(id.Value);

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
        await beliefSystemService.DeleteBeliefSystemAsync(id);
      }
      catch (RetryLimitExceededException)
      {
        return RedirectToAction("Delete", new { id, saveChangesError = true });
      }

      return RedirectToAction("Page", new { showDeleted = false });
    }
  }
}
