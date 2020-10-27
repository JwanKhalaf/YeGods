namespace YeGods.ViewModels
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Domain;

  public class SlugViewModelFactory
  {
    public static List<SlugViewModel> CreateList(IEnumerable<DeitySlug> slugs)
    {
      if (slugs == null) throw new ArgumentNullException(nameof(slugs));
      return slugs.Select(Create).ToList();
    }

    public static SlugViewModel Create(DeitySlug deitySlug)
    {
      if (deitySlug == null) throw new ArgumentNullException(nameof(deitySlug));
      SlugViewModel result = new SlugViewModel();
      result.Id = deitySlug.Id;
      result.Name = deitySlug.Name;
      result.DeityId = deitySlug.DeityId;
      result.IsDefault = deitySlug.IsDefault;
      result.CreatedAt = deitySlug.CreatedAt;
      result.ModifiedAt = deitySlug.ModifiedAt;
      result.IsDeleted = deitySlug.IsDeleted;
      return result;
    }
  }
}
