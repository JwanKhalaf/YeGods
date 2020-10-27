namespace YeGods.Services
{
  using System.Threading.Tasks;
  using ViewModels;
  using YeGods.ViewModels.Shared;

  public interface IBeliefSystemService
  {
    Task<BeliefSystemPageViewModel> GetPagedBeliefSystemsAsync(
      SearchViewModel search,
      int page);

    Task<BeliefSystemViewModel> GetBeliefSystemByIdAsync(int id);

    Task<BeliefSystemViewModel> GetBeliefSystemBySlugAsync(string slug);

    Task<BeliefSystemUpdateViewModel> GetBeliefSystemByIdForUpdateAsync(int id);

    Task UpdateBeliefSystemAsync(BeliefSystemUpdateViewModel updatedBeliefSystem);

    Task CreateNewBeliefSystem(BeliefSystemCreateViewModel newBeliefSystem);

    Task DeleteBeliefSystemAsync(int id);
  }
}
