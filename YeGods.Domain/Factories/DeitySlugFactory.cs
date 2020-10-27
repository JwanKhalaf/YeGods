namespace YeGods.Domain
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Common.Extensions;

  public class DeitySlugFactory
  {
    public static List<DeitySlug> CreateList(string[] newSlugs)
    {
      return newSlugs.Select(Create).ToList();
    }

    public static DeitySlug Create(string newSlug)
    {
      DeitySlug newDeitySlug = new DeitySlug();
      newDeitySlug.Name = newSlug.Trim().NormalizeStringForUrl();
      newDeitySlug.CreatedAt = DateTime.UtcNow;

      return newDeitySlug;
    }
  }
}
