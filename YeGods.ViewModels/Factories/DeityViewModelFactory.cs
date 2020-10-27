namespace YeGods.ViewModels
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Common.Extensions;
  using Domain;

  public class DeityViewModelFactory
  {
    public static List<DeityViewModel> CreateList(IEnumerable<Deity> deities)
    {
      if (deities == null) throw new ArgumentNullException(nameof(deities));
      return deities.Select(Create).ToList();
    }

    public static DeityViewModel Create(Deity deity)
    {
      if (deity == null) throw new ArgumentNullException(nameof(deity));

      DeitySlug defaultDeitySlug = deity.Slugs.First(s => s.IsDefault);

      DeityViewModel result = new DeityViewModel();
      result.Id = deity.Id;
      result.Name = deity.Name;
      result.Origin = deity.Origin;
      result.Description = deity.Description.Truncate(90);
      result.Slug = defaultDeitySlug.Name;
      result.Aliases = string.Join(", ", deity.Aliases.Select(x => x.Name));
      result.Sex = deity.Sex;
      result.CreatedAt = deity.CreatedAt;
      result.ModifiedAt = deity.ModifiedAt;
      result.IsDeleted = deity.IsDeleted;
      result.Category = CategoryViewModelFactory.Create(deity.Category);

      return result;
    }
  }
}
