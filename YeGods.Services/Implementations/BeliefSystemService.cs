namespace YeGods.Services
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Common.Extensions;
  using DataAccess;
  using Domain;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Options;
  using ViewModels;
  using YeGods.ViewModels.Shared;

  public class BeliefSystemService : IBeliefSystemService
  {
    private readonly int recordsPerPage;

    private readonly YeGodsContext yeGodsContext;

    public BeliefSystemService(
      IOptions<PaginationSettings> paginationSettings,
      YeGodsContext yeGodsContext)
    {
      this.recordsPerPage = paginationSettings.Value.RecordsPerPage;

      this.yeGodsContext = yeGodsContext;
    }

    public async Task<BeliefSystemPageViewModel> GetPagedBeliefSystemsAsync(SearchViewModel search, int page)
    {
      int totalNumberOfRecords;

      IQueryable<BeliefSystem> beliefSystems = this.yeGodsContext
        .BeliefSystems
        .AsNoTracking()
        .Include(bs => bs.Slugs)
        .Include(bs => bs.Aliases)
        .AsQueryable();

      if (!search.ShowDeleted)
      {
        totalNumberOfRecords = this.yeGodsContext.BeliefSystems.Count(d => !d.IsDeleted);

        beliefSystems = beliefSystems.Where(s => !s.IsDeleted);
      }
      else
      {
        totalNumberOfRecords = this.yeGodsContext.BeliefSystems.Count(d => d.IsDeleted);
      }

      if (!string.IsNullOrEmpty(search.SearchTerm))
      {
        string searchTermLowerCase = search
          .SearchTerm
          .ToLowerInvariant()
          .Trim();

        beliefSystems = beliefSystems
                    .Where(bs => EF.Functions.Like(bs.Name.ToLowerInvariant(), $"%{searchTermLowerCase}%"))
                    .OrderBy(bs => bs.Name);

        totalNumberOfRecords = beliefSystems.Count();

        beliefSystems = beliefSystems
          .Skip(this.recordsPerPage * (page - 1))
          .Take(this.recordsPerPage);
      }
      else
      {
        beliefSystems = beliefSystems
          .OrderBy(bs => bs.Name)
          .Skip(this.recordsPerPage * (page - 1))
          .Take(this.recordsPerPage);
      }

      IEnumerable<BeliefSystem> pagedBeliefSystems = await beliefSystems
        .ToListAsync();

      IEnumerable<BeliefSystemViewModel> beliefSystemsAsViewModels = BeliefSystemViewModelFactory.CreateList(pagedBeliefSystems);

      BeliefSystemPageViewModel beliefSystemsPage = new BeliefSystemPageViewModel();

      PaginationViewModel pagination = PaginationViewModelFactory.Create(this.recordsPerPage, totalNumberOfRecords, page);

      beliefSystemsPage.BeliefSystems = beliefSystemsAsViewModels;

      beliefSystemsPage.Pagination = pagination;

      beliefSystemsPage.Search = search;

      return beliefSystemsPage;
    }

    public async Task<BeliefSystemViewModel> GetBeliefSystemByIdAsync(int id)
    {
      BeliefSystem requestedBeliefSystem = await this.yeGodsContext
        .BeliefSystems
        .AsNoTracking()
        .Include(d => d.Slugs)
        .Include(d => d.Aliases)
        .FirstOrDefaultAsync(d => d.Id == id);

      return requestedBeliefSystem?.ToBeliefSystemViewModel();
    }

    public async Task<BeliefSystemViewModel> GetBeliefSystemBySlugAsync(string slug)
    {
      BeliefSystem requestedBeliefSystem = await this.yeGodsContext
        .BeliefSystems
        .AsNoTracking()
        .Include(d => d.Slugs)
        .FirstOrDefaultAsync(d => d.Slugs.Any(s => s.Name == slug && !s.IsDeleted));

      if (requestedBeliefSystem == null) return null;

      requestedBeliefSystem.Aliases = await this.yeGodsContext
        .BeliefSystemAliases
        .Where(da => !da.IsDeleted && da.BeliefSystemId == requestedBeliefSystem.Id)
        .ToListAsync();

      return requestedBeliefSystem.ToBeliefSystemViewModel();
    }

    public async Task<BeliefSystemUpdateViewModel> GetBeliefSystemByIdForUpdateAsync(int id)
    {
      BeliefSystem beliefSystem = await this.yeGodsContext
        .BeliefSystems
        .Include(d => d.Aliases)
        .Include(d => d.Slugs)
        .FirstAsync(d => d.Id == id);

      if (beliefSystem == null) throw new BeliefSystemNotFoundException(id, $"Could not find deity with Id: {id}.");

      beliefSystem.Aliases = await this.yeGodsContext
        .BeliefSystemAliases
        .Where(bsa => !bsa.IsDeleted && bsa.BeliefSystemId == beliefSystem.Id)
        .ToListAsync();

      beliefSystem.Slugs = await this.yeGodsContext
        .BeliefSystemSlugs
        .Where(bss => !bss.IsDefault && bss.BeliefSystemId == beliefSystem.Id)
        .ToListAsync();

      return beliefSystem.ToBeliefSystemUpdateViewModel();
    }

    public async Task UpdateBeliefSystemAsync(BeliefSystemUpdateViewModel updatedBeliefSystem)
    {
      BeliefSystem beliefSystem = await this.yeGodsContext
        .BeliefSystems
        .Include(d => d.Aliases)
        .Include(d => d.Slugs)
        .FirstAsync(d => d.Id == updatedBeliefSystem.Id);

      if (beliefSystem == null)
      {
        throw new BeliefSystemNotFoundException(
          updatedBeliefSystem.Id,
          $"Could not find belief system with Id: {updatedBeliefSystem.Id}.");
      }

      beliefSystem.Name = updatedBeliefSystem.Name;

      beliefSystem.Aliases = this.DeleteAliasesThatNoLongerExist(
        beliefSystem.Aliases.ToList(),
        updatedBeliefSystem.Aliases);

      beliefSystem.Slugs = this.DeleteSlugsThatNoLongerExist(
        beliefSystem.Slugs.ToList(),
        beliefSystem.GetSlugs());

      if (updatedBeliefSystem.Aliases != null)
      {
        beliefSystem.Aliases = this.CreateNewlyAddedAliases(
          beliefSystem.Aliases.ToList(),
          updatedBeliefSystem.Aliases);
      }

      beliefSystem.Slugs = this.CreateNewlyAddedSlugs(
        beliefSystem.Name,
        beliefSystem.GetSlugs(),
        beliefSystem.Slugs.ToList());

      beliefSystem.GeographicalRegion = updatedBeliefSystem.GeographicalRegion;
      beliefSystem.Description = updatedBeliefSystem.Description;
      beliefSystem.ModifiedAt = DateTime.UtcNow;
      this.yeGodsContext.Entry(beliefSystem).State = EntityState.Modified;

      await this.yeGodsContext.SaveChangesAsync();
    }

    public async Task CreateNewBeliefSystem(BeliefSystemCreateViewModel newBeliefSystem)
    {
      BeliefSystem beliefSystem = new BeliefSystem();
      beliefSystem.Aliases = BeliefSystemAliasFactory.CreateList(newBeliefSystem.Aliases);
      beliefSystem.Name = newBeliefSystem.Name;
      beliefSystem.GeographicalRegion = newBeliefSystem.GeographicalRegion;
      beliefSystem.Description = newBeliefSystem.Description;
      beliefSystem.CreatedAt = DateTime.UtcNow;
      beliefSystem.Slugs = BeliefSystemSlugFactory.CreateList(beliefSystem.GetSlugs());
      beliefSystem.Slugs.First(s => s.Name.NormalizeStringForUrl() == beliefSystem.Name.NormalizeStringForUrl())
        .IsDefault = true;

      this.yeGodsContext.BeliefSystems.Add(beliefSystem);
      await this.yeGodsContext.SaveChangesAsync();
    }

    public async Task DeleteBeliefSystemAsync(int id)
    {
      BeliefSystem beliefSystem = await this.yeGodsContext
        .BeliefSystems
        .Include(d => d.Slugs)
        .Include(d => d.Aliases)
        .FirstOrDefaultAsync(d => d.Id == id);

      beliefSystem.IsDeleted = true;

      foreach (BeliefSystemSlug slug in beliefSystem.Slugs)
      {
        slug.IsDeleted = true;
      }

      foreach (BeliefSystemAlias alias in beliefSystem.Aliases)
      {
        alias.IsDeleted = true;
      }

      await this.yeGodsContext.SaveChangesAsync();
    }

    private List<BeliefSystemAlias> DeleteAliasesThatNoLongerExist(
      List<BeliefSystemAlias> existingAliases,
      string updatedAliases)
    {
      string[] aliases;

      if (!string.IsNullOrEmpty(updatedAliases))
      {
        aliases = updatedAliases
        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        .Select(a => a.Trim())
        .ToArray();
      } else
      {
        aliases = new string[0];
      }

      string[] currentAliases = existingAliases
        .Select(a => a.Name)
        .ToArray();

      List<string> deletedAliases = currentAliases
        .Where(a => !aliases.Contains(a.Trim()))
        .ToList();

      List<BeliefSystemAlias> aliasesToDelete = new List<BeliefSystemAlias>();

      foreach (string alias in deletedAliases)
      {
        BeliefSystemAlias beliefSystemAlias = existingAliases
          .Single(a => a.Name == alias);

        aliasesToDelete.Add(beliefSystemAlias);
      }

      for (int i = existingAliases.Count - 1; i >= 0; i--)
      {
        BeliefSystemAlias existingAlias = existingAliases[i];

        foreach (BeliefSystemAlias beliefSystemAlias in aliasesToDelete)
        {
          if (existingAlias.Equals(beliefSystemAlias))
          {
            existingAlias.IsDeleted = true;
            existingAlias.ModifiedAt = DateTime.UtcNow;
          }
        }
      }

      return existingAliases;
    }

    private List<BeliefSystemSlug> DeleteSlugsThatNoLongerExist(
      List<BeliefSystemSlug> existingSlugs,
      string[] updatedSlugs)
    {
      string[] currentSlugs = existingSlugs
        .Select(a => a.Name)
        .ToArray();

      IEnumerable<string> deletedSlugs = currentSlugs
        .Where(a => !updatedSlugs.Contains(a.Trim()));

      List<BeliefSystemSlug> slugsToDelete = new List<BeliefSystemSlug>();

      foreach (string slug in deletedSlugs)
      {
        BeliefSystemSlug beliefSystemSlug = existingSlugs
          .Single(a => a.Name == slug);

        slugsToDelete.Add(beliefSystemSlug);
      }

      for (int i = existingSlugs.Count - 1; i >= 0; i--)
      {
        BeliefSystemSlug existingSlug = existingSlugs[i];

        foreach (BeliefSystemSlug beliefSystemSlug in slugsToDelete)
        {
          if (existingSlug.Name.Equals(beliefSystemSlug.Name))
          {
            existingSlug.IsDefault = false;
            existingSlug.IsDeleted = true;
            existingSlug.ModifiedAt = DateTime.UtcNow;
          }
        }
      }

      return existingSlugs;
    }

    private List<BeliefSystemAlias> CreateNewlyAddedAliases(
      List<BeliefSystemAlias> existingAliases,
      string updatedAliases)
    {
      string[] aliases = updatedAliases
        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

      string[] currentAliases = existingAliases
        .Select(a => a.Name)
        .ToArray();

      IEnumerable<string> newlyAddedAliases = aliases
        .Where(a => !currentAliases.Contains(a.Trim()));

      foreach (string alias in newlyAddedAliases)
      {
        BeliefSystemAlias beliefSystemAlias = new BeliefSystemAlias();
        beliefSystemAlias.Name = alias.Trim();
        beliefSystemAlias.CreatedAt = DateTime.UtcNow;
        existingAliases.Add(beliefSystemAlias);
      }

      return existingAliases;
    }

    private List<BeliefSystemSlug> CreateNewlyAddedSlugs(
      string nameOfBeliefSystem,
      string[] updatedSlugs,
      List<BeliefSystemSlug> currentSlugs)
    {
      foreach (string potentialSlug in updatedSlugs)
      {
        if (currentSlugs.Where(s => !s.IsDeleted).All(s => s.Name != potentialSlug.NormalizeStringForUrl()))
        {
          bool setAsDefaultSlug = nameOfBeliefSystem.NormalizeStringForUrl().Equals(potentialSlug);

          currentSlugs
            .Add(BeliefSystemSlugFactory.Create(potentialSlug, setAsDefaultSlug));
        }
      }

      return currentSlugs;
    }
  }
}
