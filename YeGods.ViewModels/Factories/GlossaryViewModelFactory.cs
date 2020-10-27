namespace YeGods.ViewModels
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Common.Extensions;
  using Domain;

  public class GlossaryViewModelFactory
  {
    public static List<GlossaryViewModel> CreateList(IEnumerable<Glossary> glossaries)
    {
      if (glossaries == null) throw new ArgumentNullException(nameof(glossaries));

      return glossaries.Select(Create).ToList();
    }

    public static GlossaryViewModel Create(Glossary glossary)
    {
      if (glossary == null) throw new ArgumentNullException(nameof(glossary));

      GlossaryViewModel result = new GlossaryViewModel();
      result.Id = glossary.Id;
      result.Name = glossary.Name;
      result.Origin = glossary.Origin;
      result.Description = glossary.Description.Truncate(90);
      result.CreatedAt = glossary.CreatedAt;
      result.ModifiedAt = glossary.ModifiedAt;
      result.IsDeleted = glossary.IsDeleted;

      return result;
    }
  }
}
