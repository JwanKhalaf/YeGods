namespace YeGods.ViewModels
{
  using System.Linq;
  using Domain;

  public static class EntityToViewModel
  {
    public static DeityViewModel ToDeityViewModel(this Deity deity)
    {
      DeitySlug defaultDeitySlug = deity.Slugs.First(s => s.IsDefault);
      DeityViewModel result = new DeityViewModel();
      result.Id = deity.Id;
      result.Name = deity.Name;
      result.Slug = defaultDeitySlug.Name;
      result.Aliases = deity.Aliases.Any() ? string.Join(", ", deity.Aliases.Select(x => x.Name)) : "None";
      result.Sex = deity.Sex;
      result.Description = CommonMark.CommonMarkConverter.Convert(deity.Description);
      result.Origin = deity.Origin;
      result.Category = deity.Category.ToCategoryViewModel();
      return result;
    }

    public static DeityUpdateViewModel ToDeityUpdateViewModel(this Deity deity)
    {
      DeityUpdateViewModel result = new DeityUpdateViewModel();
      result.Id = deity.Id;
      result.Name = deity.Name;
      result.Aliases = string.Join(", ", deity.Aliases.Select(a => a.Name).ToArray());
      result.Description = deity.Description;
      result.Sex = deity.Sex;
      result.Origin = deity.Origin;
      result.CategoryId = deity.CategoryId;
      return result;
    }

    public static CategoryViewModel ToCategoryViewModel(this Category category)
    {
      CategoryViewModel result = new CategoryViewModel();
      result.Id = category.Id;
      result.Name = category.Name;
      result.CreatedAt = category.CreatedAt;
      result.ModifiedAt = category.ModifiedAt;
      result.IsDeleted = category.IsDeleted;
      return result;
    }

    public static CategoryUpdateViewModel ToCategoryUpdateViewModel(this Category category)
    {
      CategoryUpdateViewModel result = new CategoryUpdateViewModel();
      result.Id = category.Id;
      result.Name = category.Name;
      return result;
    }

    public static GlossaryViewModel ToGlossaryViewModel(this Glossary glossary)
    {
      GlossaryViewModel result = new GlossaryViewModel();
      result.Id = glossary.Id;
      result.Name = glossary.Name;
      result.Origin = glossary.Origin;
      result.Description = glossary.Description;
      result.CreatedAt = glossary.CreatedAt;
      result.ModifiedAt = glossary.ModifiedAt;
      result.IsDeleted = glossary.IsDeleted;
      return result;
    }

    public static GlossaryUpdateViewModel ToGlossaryUpdateViewModel(this Glossary glossary)
    {
      GlossaryUpdateViewModel result = new GlossaryUpdateViewModel();
      result.Id = glossary.Id;
      result.Name = glossary.Name;
      result.Origin = glossary.Origin;
      result.Description = glossary.Description;
      return result;
    }

    public static BeliefSystemViewModel ToBeliefSystemViewModel(this BeliefSystem beliefSystem)
    {
      BeliefSystemSlug defaultBeliefSystemSlug = beliefSystem.Slugs.First(s => s.IsDefault);
      BeliefSystemViewModel result = new BeliefSystemViewModel();
      result.Id = beliefSystem.Id;
      result.Name = beliefSystem.Name;
      result.GeographicalRegion = beliefSystem.GeographicalRegion;
      result.Description = CommonMark.CommonMarkConverter.Convert(beliefSystem.Description);
      result.CreatedAt = beliefSystem.CreatedAt;
      result.Aliases = beliefSystem.Aliases.Any() ? string.Join(", ", beliefSystem.Aliases.Select(x => x.Name)) : "None";
      result.Slug = defaultBeliefSystemSlug.Name;

      return result;
    }

    public static BeliefSystemUpdateViewModel ToBeliefSystemUpdateViewModel(this BeliefSystem beliefSystem)
    {
      BeliefSystemUpdateViewModel result = new BeliefSystemUpdateViewModel();
      result.Id = beliefSystem.Id;
      result.Name = beliefSystem.Name;
      result.GeographicalRegion = beliefSystem.GeographicalRegion;
      result.Description = beliefSystem.Description;
      result.Aliases = string.Join(", ", beliefSystem.Aliases.Select(a => a.Name).ToArray());

      return result;
    }
  }
}
