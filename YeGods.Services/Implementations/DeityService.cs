namespace YeGods.Services
{
  using Common.Extensions;
  using DataAccess;
  using Domain;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Options;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using ViewModels;
  using YeGods.ViewModels.Shared;

  public class DeityService : IDeityService
  {
    private readonly int recordsPerPage;
    private readonly YeGodsContext yeGodsContext;

    public DeityService(
      IOptions<PaginationSettings> paginationSettings,
      YeGodsContext yeGodsContext)
    {
      this.recordsPerPage = paginationSettings.Value.RecordsPerPage;

      this.yeGodsContext = yeGodsContext ?? throw new ArgumentNullException(nameof(yeGodsContext));
    }

    public async Task<DeityPageViewModel> GetPagedDeitiesAsync(
      SearchViewModel search,
      int page)
    {
      int totalNumberOfRecords;

      if (search.SearchTermHasChanged())
      {
        page = 1;
      }

      IQueryable<Deity> deities = this.yeGodsContext
        .Deities
        .AsNoTracking()
        .Include(d => d.Slugs)
        .Include(d => d.Aliases)
        .Include(d => d.Category)
        .AsQueryable();

      if (!search.ShowDeleted)
      {
        totalNumberOfRecords = this.yeGodsContext.Deities.Count(d => !d.IsDeleted);

        deities = deities.Where(s => !s.IsDeleted);
      }
      else
      {
        totalNumberOfRecords = this.yeGodsContext.Deities.Count(d => d.IsDeleted);
      }

      if (!string.IsNullOrEmpty(search.SearchTerm))
      {
        string searchTermLowerCase = search
          .SearchTerm
          .ToLowerInvariant()
          .Trim();

        deities = deities
                    .Where(d => EF.Functions.Like(d.Name.ToLowerInvariant(), $"%{searchTermLowerCase}%"))
                    .OrderBy(d => d.Name);

        totalNumberOfRecords = deities.Count();

        deities = deities
          .Skip(this.recordsPerPage * (page - 1))
          .Take(this.recordsPerPage);
      }
      else
      {
        deities = deities
          .OrderBy(d => d.Name)
          .Skip(this.recordsPerPage * (page - 1))
          .Take(this.recordsPerPage);
      }

      IEnumerable<Deity> pagedDeities = await deities
      .ToListAsync();

      IEnumerable<DeityViewModel> deitiesAsViewModels = DeityViewModelFactory.CreateList(pagedDeities);

      DeityPageViewModel deitiesPage = new DeityPageViewModel();

      PaginationViewModel pagination = PaginationViewModelFactory.Create(this.recordsPerPage, totalNumberOfRecords, page);

      deitiesPage.Deities = deitiesAsViewModels;

      deitiesPage.Pagination = pagination;

      deitiesPage.Search = search;

      return deitiesPage;
    }

    public async Task<DeityViewModel> GetDeityByIdAsync(int id)
    {
      Deity requestedDeity = await this.yeGodsContext
        .Deities
        .AsNoTracking()
        .Include(d => d.Slugs)
        .Include(d => d.Aliases)
        .Include(d => d.Category)
        .FirstOrDefaultAsync(d => d.Id == id);

      return requestedDeity?.ToDeityViewModel();
    }

    public async Task<DeityViewModel> GetDeityBySlugAsync(string slug)
    {
      Deity requestedDeity = await this.yeGodsContext
        .Deities
        .AsNoTracking()
        .Include(d => d.Slugs)
        .Include(d => d.Category)
        .FirstOrDefaultAsync(d => d.Slugs.Any(s => s.Name == slug && !s.IsDeleted));

      if (requestedDeity != null)
      {
        requestedDeity.Aliases = await this.yeGodsContext
          .DeityAliases
          .Where(da => !da.IsDeleted && da.DeityId == requestedDeity.Id)
          .ToListAsync();
      }

      return requestedDeity?.ToDeityViewModel();
    }

    public async Task<RandomDeityViewModel> GetRandomDeity()
    {
      List<string> backgroundImageNames = new List<string>();

      backgroundImageNames.Add("zeus");

      backgroundImageNames.Add("goddess");

      Random random = new Random();

      int randomIndex = random.Next(backgroundImageNames.Count);

      int totalNumberOfDeities = this.yeGodsContext
        .Deities
        .Count(d => !d.IsDeleted);

      if (totalNumberOfDeities == 0)
      {
        return new RandomDeityViewModel
        {
          Name = "",
          Slug = "",
          BackgroundImageName = backgroundImageNames[randomIndex]
        };
      }

      int numberToSkip = (int)(random.NextDouble() * totalNumberOfDeities);

      RandomDeityViewModel randomDeity = await this.yeGodsContext
        .Deities
        .Include(d => d.Slugs)
        .AsNoTracking()
        .Where(d => !d.IsDeleted)
        .OrderBy(d => d.Id)
        .Skip(numberToSkip)
        .Take(1)
        .Select(d => new RandomDeityViewModel
        {
          Name = d.Name,
          Slug = d.GetDefaultSlug().Name,
          BackgroundImageName = backgroundImageNames[randomIndex]
        })
        .FirstAsync();

      return randomDeity;
    }

    public async Task<DeityUpdateViewModel> GetDeityByIdForUpdateAsync(int id)
    {
      Deity deity = await this.yeGodsContext
        .Deities
        .Include(d => d.Aliases)
        .Include(d => d.Slugs)
        .Include(d => d.Category)
        .FirstAsync(d => d.Id == id);

      if (deity == null) throw new DeityNotFoundException(id, $"Could not find deity with Id: {id}.");

      deity.Aliases = await this.yeGodsContext
        .DeityAliases
        .Where(da => !da.IsDeleted && da.DeityId == deity.Id)
        .ToListAsync();

      deity.Slugs = await this.yeGodsContext
        .DeitySlugs
        .Where(ds => !ds.IsDefault && ds.DeityId == deity.Id)
        .ToListAsync();

      return deity.ToDeityUpdateViewModel();
    }

    public async Task UpdateDeityAsync(DeityUpdateViewModel updatedDeity)
    {
      Deity deity = await this.yeGodsContext
        .Deities
        .Include(d => d.Aliases)
        .Include(d => d.Slugs)
        .Include(d => d.Category)
        .FirstAsync(d => d.Id == updatedDeity.Id);

      if (deity == null)
      {
        throw new DeityNotFoundException(
          updatedDeity.Id,
          $"Could not find deity with Id: {updatedDeity.Id}.");
      }

      deity.Aliases = this.DeleteAliasesThatNoLongerExist(
        deity.Aliases.ToList(),
        updatedDeity.Aliases);

      deity.Slugs = this.DeleteSlugsThatNoLongerExist(
        deity.Slugs.ToList(),
        deity.GetSlugs());

      deity.Aliases = this.CreateNewlyAddedAliases(
        deity.Aliases.ToList(),
        updatedDeity.Aliases);

      deity.Slugs = this.CreateNewlyAddedSlugs(
        deity.Aliases.ToList(),
        deity.Slugs.ToList());

      deity.Name = updatedDeity.Name;
      deity.Sex = updatedDeity.Sex;
      deity.Origin = updatedDeity.Origin;
      deity.CategoryId = updatedDeity.CategoryId;
      deity.Description = updatedDeity.Description;
      deity.ModifiedAt = DateTime.UtcNow;
      this.yeGodsContext.Entry(deity).State = EntityState.Modified;

      await this.yeGodsContext.SaveChangesAsync();
    }

    private List<DeitySlug> CreateNewlyAddedSlugs(
      List<DeityAlias> updatedDeityAliases,
      List<DeitySlug> currentSlugs)
    {
      foreach (DeityAlias deityAlias in updatedDeityAliases)
      {
        if (currentSlugs.All(s => s.Name != deityAlias.Name.NormalizeStringForUrl()))
        {
          currentSlugs.Add(DeitySlugFactory.Create(deityAlias.Name));
        }
      }

      return currentSlugs;
    }

    private List<DeitySlug> DeleteSlugsThatNoLongerExist(
      List<DeitySlug> existingSlugs,
      string[] updatedSlugs)
    {
      string[] currentSlugs = existingSlugs.Select(a => a.Name).ToArray();
      IEnumerable<string> deletedSlugs = currentSlugs.Where(a => !updatedSlugs.Contains(a.Trim()));
      List<DeitySlug> slugsToDelete = new List<DeitySlug>();

      foreach (string slug in deletedSlugs)
      {
        DeitySlug deitySlug = existingSlugs.Single(a => a.Name == slug);
        slugsToDelete.Add(deitySlug);
      }

      for (int i = existingSlugs.Count - 1; i >= 0; i--)
      {
        DeitySlug existingSlug = existingSlugs[i];
        foreach (DeitySlug deitySlug in slugsToDelete)
        {
          if (existingSlug.Equals(deitySlug))
          {
            existingSlug.IsDeleted = true;
            existingSlug.ModifiedAt = DateTime.UtcNow;
          }
        }
      }

      return existingSlugs;
    }

    private List<DeityAlias> CreateNewlyAddedAliases(
      List<DeityAlias> existingAliases,
      string updatedAliases)
    {
      string[] aliases = updatedAliases.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      string[] currentAliases = existingAliases.Select(a => a.Name).ToArray();
      IEnumerable<string> newlyAddedAliases = aliases.Where(a => !currentAliases.Contains(a.Trim()));

      foreach (string alias in newlyAddedAliases)
      {
        DeityAlias deityAlias = new DeityAlias();
        deityAlias.Name = alias.Trim();
        deityAlias.CreatedAt = DateTime.UtcNow;
        existingAliases.Add(deityAlias);
      }

      return existingAliases;
    }

    private List<DeityAlias> DeleteAliasesThatNoLongerExist(
      List<DeityAlias> existingAliases,
      string updatedAliases)
    {
      string[] aliases = updatedAliases
        .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        .Select(a => a.Trim())
        .ToArray();
      string[] currentAliases = existingAliases.Select(a => a.Name).ToArray();
      List<string> deletedAliases = currentAliases.Where(a => !aliases.Contains(a.Trim())).ToList();
      List<DeityAlias> aliasesToDelete = new List<DeityAlias>();

      foreach (string alias in deletedAliases)
      {
        DeityAlias deityAlias = existingAliases.Single(a => a.Name == alias);
        aliasesToDelete.Add(deityAlias);
      }

      for (int i = existingAliases.Count - 1; i >= 0; i--)
      {
        DeityAlias existingAlias = existingAliases[i];
        foreach (DeityAlias deityAlias in aliasesToDelete)
        {
          if (existingAlias.Equals(deityAlias))
          {
            existingAlias.IsDeleted = true;
            existingAlias.ModifiedAt = DateTime.UtcNow;
          }
        }
      }

      return existingAliases;
    }

    public async Task CreateNewDeity(DeityCreateViewModel newDeity)
    {
      Deity deity = new Deity();
      deity.Aliases = DeityAliasFactory.CreateList(newDeity.Aliases);
      deity.Name = newDeity.Name;
      deity.Sex = newDeity.Sex;
      deity.Origin = newDeity.Origin;
      deity.CategoryId = newDeity.CategoryId;
      deity.Description = newDeity.Description;
      deity.CreatedAt = DateTime.UtcNow;
      deity.Slugs = DeitySlugFactory.CreateList(deity.GetSlugs());
      deity.Slugs.First(s => s.Name.NormalizeStringForUrl() == deity.Name.NormalizeStringForUrl()).IsDefault = true;
      this.yeGodsContext.Deities.Add(deity);
      await this.yeGodsContext.SaveChangesAsync();
    }

    public async Task DeleteDeityAsync(int id)
    {
      Deity deity = await this.yeGodsContext
        .Deities
        .Include(d => d.Slugs)
        .Include(d => d.Aliases)
        .FirstOrDefaultAsync(d => d.Id == id);

      deity.IsDeleted = true;

      foreach (DeitySlug slug in deity.Slugs)
      {
        slug.IsDeleted = true;
      }

      foreach (DeityAlias alias in deity.Aliases)
      {
        alias.IsDeleted = true;
      }

      await this.yeGodsContext.SaveChangesAsync();
    }
  }
}
