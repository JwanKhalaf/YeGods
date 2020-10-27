namespace YeGods.Web.Areas.Admin.Controllers
{
  using Microsoft.AspNetCore.Mvc;
  using System.Threading.Tasks;
  using Services;
  using ViewModels;
  using YeGods.ViewModels.Shared;

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
      GlossaryPageViewModel glossaries = await this.glossaryService
        .GetPagedGlossariesAsync(search, page);

      return this.View(glossaries);
    }

    [Route("[action]")]
    public IActionResult Create()
    {
      GlossaryCreateViewModel newGlossary = new GlossaryCreateViewModel();
      return this.View(newGlossary);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Create(GlossaryCreateViewModel newGlossary)
    {
      if (!this.ModelState.IsValid)
      {
        return this.View(newGlossary);
      }

      await this.glossaryService.CreateNewGlossary(newGlossary);
      return this.Redirect("Page");
    }

    [Route("[action]")]
    public async Task<IActionResult> Edit(int id)
    {
      GlossaryUpdateViewModel glossaryToUpdate = await this.glossaryService.GetGlossaryByIdForUpdateAsync(id);
      return this.View(glossaryToUpdate);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Edit(GlossaryUpdateViewModel updatedGlossary)
    {
      if (!this.ModelState.IsValid)
      {
        return this.View(updatedGlossary);
      }

      await this.glossaryService.UpdateGlossaryAsync(updatedGlossary);
      return this.Redirect("Page");
    }

    [Route("[action]")]
    public async Task<IActionResult> Delete(int id)
    {
      await this.glossaryService.DeleteGlossary(id);
      return this.Redirect("Page");
    }
  }
}
