namespace YeGods.Web.Areas.Admin.Controllers
{
  using Microsoft.AspNetCore.Mvc;
  using System.Threading.Tasks;
  using Services;
  using ViewModels;
  using ViewModels.Shared;

  [Area("Admin")]
  [Route("admin/glossary")]
  public class GlossaryController : Controller
  {
    private readonly IGlossaryService glossaryService;

    public GlossaryController(IGlossaryService glossaryService)
    {
      this.glossaryService = glossaryService;
    }

    [Route("[action]")]
    public async Task<IActionResult> Page(SearchViewModel search, int page = 1)
    {
      GlossaryPageViewModel glossaries = await glossaryService
        .GetPagedGlossariesAsync(search, page);

      return View(glossaries);
    }

    [Route("[action]")]
    public IActionResult Create()
    {
      GlossaryCreateViewModel newGlossary = new GlossaryCreateViewModel();
      return View(newGlossary);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Create(GlossaryCreateViewModel newGlossary)
    {
      if (!ModelState.IsValid)
      {
        return View(newGlossary);
      }

      await glossaryService.CreateNewGlossary(newGlossary);
      return Redirect("Page");
    }

    [Route("[action]")]
    public async Task<IActionResult> Edit(int id)
    {
      GlossaryUpdateViewModel glossaryToUpdate = await glossaryService.GetGlossaryByIdForUpdateAsync(id);
      return View(glossaryToUpdate);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Edit(GlossaryUpdateViewModel updatedGlossary)
    {
      if (!ModelState.IsValid)
      {
        return View(updatedGlossary);
      }

      await glossaryService.UpdateGlossaryAsync(updatedGlossary);
      return Redirect("Page");
    }

    [Route("[action]")]
    public async Task<IActionResult> Delete(int id)
    {
      await glossaryService.DeleteGlossary(id);
      return Redirect("Page");
    }
  }
}
