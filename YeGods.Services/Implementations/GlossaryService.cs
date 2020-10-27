namespace YeGods.Services
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Threading.Tasks;
  using DataAccess;
  using Domain;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Options;
  using ViewModels;
  using YeGods.ViewModels.Shared;

  public class GlossaryService : IGlossaryService
  {
    private readonly int recordsPerPage;
    private readonly YeGodsContext yeGodsContext;

    public GlossaryService(
      IOptions<PaginationSettings> paginationSettings,
      YeGodsContext yeGodsContext)
    {
      this.recordsPerPage = paginationSettings.Value.RecordsPerPage;
      this.yeGodsContext = yeGodsContext ?? throw new ArgumentNullException(nameof(yeGodsContext));
    }

    public async Task<Dictionary<char, List<GlossaryViewModel>>> GetGlossaryByLetterGroupingAsync()
    {
      Dictionary<char, List<GlossaryViewModel>> glossaryLetterGroups = new Dictionary<char, List<GlossaryViewModel>>();

      IEnumerable <Glossary> glossaries = await this.yeGodsContext
        .Glossaries
        .Where(g => !g.IsDeleted)
        .OrderBy(g => g.Name)
        .ToListAsync();

      foreach (Glossary glossary in glossaries)
      {
        char letter = glossary.Name[0];

        if (!glossaryLetterGroups.ContainsKey(letter))
        {
          glossaryLetterGroups.Add(letter, new List<GlossaryViewModel>());
        }

        List<GlossaryViewModel> letterGlossaries = glossaryLetterGroups[letter];

        letterGlossaries.Add(glossary.ToGlossaryViewModel());
      }

      return glossaryLetterGroups;
    }

    public async Task<GlossaryPageViewModel> GetPagedGlossariesAsync(
      SearchViewModel search,
      int page)
    {
      int totalNumberOfRecords;

      IQueryable<Glossary> glossaries = this.yeGodsContext
        .Glossaries
        .AsNoTracking()
        .AsQueryable();

      if (!search.ShowDeleted)
      {
        totalNumberOfRecords = this.yeGodsContext
          .Glossaries
          .Count(d => !d.IsDeleted);

        glossaries = glossaries.Where(s => !s.IsDeleted);
      }
      else
      {
        totalNumberOfRecords = this.yeGodsContext
          .Glossaries
          .Count(d => d.IsDeleted);
      }

      if (!string.IsNullOrEmpty(search.SearchTerm))
      {
        string searchTermLowerCase = search
          .SearchTerm
          .ToLowerInvariant()
          .Trim();

        glossaries = glossaries
                    .Where(g => EF.Functions.Like(g.Name.ToLowerInvariant(), $"%{searchTermLowerCase}%"))
                    .OrderBy(g => g.Name);

        totalNumberOfRecords = glossaries.Count();

        glossaries = glossaries
          .Skip(this.recordsPerPage * (page - 1))
          .Take(this.recordsPerPage);
      }
      else
      {
        glossaries = glossaries
          .OrderBy(g => g.Name)
          .Skip(this.recordsPerPage * (page - 1))
          .Take(this.recordsPerPage);
      }

      IEnumerable<Glossary> pagedGlossaries = await glossaries
        .ToListAsync();

      IEnumerable<GlossaryViewModel> glossariesAsViewModels = GlossaryViewModelFactory.CreateList(pagedGlossaries);

      GlossaryPageViewModel glossariesPage = new GlossaryPageViewModel();

      PaginationViewModel pagination = PaginationViewModelFactory.Create(
        this.recordsPerPage,
        totalNumberOfRecords, page);

      glossariesPage.Glossaries = glossariesAsViewModels;

      glossariesPage.Pagination = pagination;

      glossariesPage.Search = search;

      return glossariesPage;
    }

    public async Task<GlossaryViewModel> GetGlossaryByIdAsync(int id)
    {
      Glossary requestedGlossary = await this.yeGodsContext
        .Glossaries
        .AsNoTracking()
        .FirstOrDefaultAsync(d => d.Id == id);

      return requestedGlossary?.ToGlossaryViewModel();
    }

    public async Task<GlossaryUpdateViewModel> GetGlossaryByIdForUpdateAsync(int id)
    {
      Glossary glossary = await this.yeGodsContext
        .Glossaries
        .FirstAsync(d => d.Id == id);

      if (glossary == null) throw new GlossaryNotFoundException(id, $"Could not find glossary with Id: {id}.");

      return glossary.ToGlossaryUpdateViewModel();
    }

    public async Task UpdateGlossaryAsync(GlossaryUpdateViewModel updatedGlossary)
    {
      Glossary glossary = await this.yeGodsContext
        .Glossaries
        .FirstAsync(d => d.Id == updatedGlossary.Id);

      if (glossary == null)
      {
        throw new GlossaryNotFoundException(updatedGlossary.Id, $"Could not find glossary with Id: {updatedGlossary.Id}.");
      }

      glossary.Name = updatedGlossary.Name;
      glossary.Origin = updatedGlossary.Origin;
      glossary.Description = updatedGlossary.Description;
      glossary.ModifiedAt = DateTime.UtcNow;
      this.yeGodsContext.Entry(glossary).State = EntityState.Modified;
      await this.yeGodsContext.SaveChangesAsync();
    }

    public async Task CreateNewGlossary(GlossaryCreateViewModel newGlossary)
    {
      TextInfo textInfo = new CultureInfo("en-GB", false).TextInfo;
      Glossary glossary = new Glossary();
      glossary.Name = textInfo.ToTitleCase(newGlossary.Name.ToLower());
      glossary.Origin = textInfo.ToTitleCase(newGlossary.Origin.ToLower());
      glossary.Description = newGlossary.Description;
      glossary.CreatedAt = DateTime.UtcNow;
      this.yeGodsContext.Glossaries.Add(glossary);

      await this.yeGodsContext.SaveChangesAsync();
    }

    public async Task DeleteGlossary(int glossaryId)
    {
      Glossary glossary = this.yeGodsContext.Glossaries.Find(glossaryId);

      if (glossary == null)
      {
        throw new GlossaryNotFoundException(glossaryId, $"Could not find glossary with Id: {glossaryId}.");
      }

      glossary.IsDeleted = true;
      this.yeGodsContext.Entry(glossary).State = EntityState.Modified;

      await this.yeGodsContext.SaveChangesAsync();
    }
  }
}
