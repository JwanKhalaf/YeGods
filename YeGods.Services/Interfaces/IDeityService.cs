namespace YeGods.Services
{
  using System.Threading.Tasks;
  using ViewModels;
  using YeGods.ViewModels.Shared;

  public interface IDeityService
  {
    Task<DeityPageViewModel> GetPagedDeitiesAsync(SearchViewModel search, int page);

    Task<DeityViewModel> GetDeityByIdAsync(int id);

    Task<DeityViewModel> GetDeityBySlugAsync(string slug);

    Task<RandomDeityViewModel> GetRandomDeity();

    Task<DeityUpdateViewModel> GetDeityByIdForUpdateAsync(int id);

    Task UpdateDeityAsync(DeityUpdateViewModel updatedDeity);

    Task CreateNewDeity(DeityCreateViewModel newDeity);

    Task DeleteDeityAsync(int id);
  }
}
