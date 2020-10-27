namespace YeGods.Services
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using DataAccess;
  using Domain;
  using Microsoft.EntityFrameworkCore;
  using ViewModels;

  public class SearchService : ISearchService
  {
    private readonly YeGodsContext yeGodsContext;

    public SearchService(YeGodsContext yeGodsContext)
    {
      this.yeGodsContext = yeGodsContext;
    }

    public async Task<SearchResultViewModel> SearchDeitiesBySluxAsync(string entitySlug)
    {
      IEnumerable<Deity> deities = await this.yeGodsContext.Deities
        .AsNoTracking()
        .Include(d => d.Slugs)
        .Include(d => d.Aliases)
        .Include(d => d.Category)
        .Where(d => d.Slugs.Any(s => EF.Functions.Like(s.Name, $"%{entitySlug}%")))
        .ToListAsync();

      IEnumerable<BeliefSystem> beliefSystems = await this.yeGodsContext.BeliefSystems
        .AsNoTracking()
        .Include(bs => bs.Slugs)
        .Where(bs => bs.Slugs.Any(s => EF.Functions.Like(s.Name, $"%{entitySlug}%")))
        .ToListAsync();

      SearchResultViewModel searchResults = new SearchResultViewModel();

      searchResults.Matches = deities.Select(d => new SearchResultItemViewModel
      {
        Name = d.Name,
        Slug = d.Slugs.First(s => s.IsDefault).Name,
        IsDeity = true
      }).ToList();

      List<SearchResultItemViewModel> beliefSystemsAsMatches = beliefSystems.Select(bs => new SearchResultItemViewModel
      {
        Name = bs.Name,
        Slug = bs.Slugs.First(s => s.IsDefault).Name,
        IsDeity = false

      }).ToList();

      searchResults.Matches.AddRange(beliefSystemsAsMatches);

      return searchResults;
    }
  }
}
