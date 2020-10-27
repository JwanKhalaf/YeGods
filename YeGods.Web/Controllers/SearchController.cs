namespace YeGods.Web.Controllers
{
  using Common.Extensions;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Services;
  using ViewModels;

  public class SearchController : Controller
  {
    private readonly ISearchService searchService;

    public SearchController(ISearchService searchService)
    {
      this.searchService = searchService;
    }

    [HttpPost("/search/{searchTerm}", Name = "Get deity by slug")]
    public IActionResult Search(string searchTerm, bool isDeity)
    {
      if (string.IsNullOrEmpty(searchTerm))
      {
        return this.RedirectToAction("Index", "Home");
      }

      if (isDeity)
      {
        return this.RedirectToAction("Details", "Deity", new { slug = searchTerm });
      }

      return this.RedirectToAction("Details", "BeliefSystem", new { slug = searchTerm });
    }

    [HttpPost("/search/suggestions/{searchTerm}", Name = "Get search suggestions")]
    public async Task<JsonResult> GetSearchSuggestions(string searchTerm)
    {
      string parsedSearchTerm = searchTerm.NormalizeStringForUrl();
      SearchResultViewModel searchResults = await this.searchService.SearchDeitiesBySluxAsync(parsedSearchTerm);
      return new JsonResult(searchResults);
    }
  }
}
