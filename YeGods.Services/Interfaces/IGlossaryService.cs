namespace YeGods.Services
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using ViewModels;
  using YeGods.ViewModels.Shared;

  public interface IGlossaryService
  {
    Task<Dictionary<char, List<GlossaryViewModel>>> GetGlossaryByLetterGroupingAsync();
    Task<GlossaryPageViewModel> GetPagedGlossariesAsync(SearchViewModel search, int page);
    Task<GlossaryViewModel> GetGlossaryByIdAsync(int id);
    Task<GlossaryUpdateViewModel> GetGlossaryByIdForUpdateAsync(int id);
    Task UpdateGlossaryAsync(GlossaryUpdateViewModel updatedGlossary);
    Task CreateNewGlossary(GlossaryCreateViewModel newGlossary);
    Task DeleteGlossary(int glossaryId);
  }
}
