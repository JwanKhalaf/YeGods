namespace YeGods.Services
{
  using System.Threading.Tasks;
  using ViewModels;

  public interface ISearchService
  {
    Task<SearchResultViewModel> SearchDeitiesBySluxAsync(string entitySlug);
  }
}
