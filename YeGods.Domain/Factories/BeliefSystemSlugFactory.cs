namespace YeGods.Domain
{
  using Common.Extensions;
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public class BeliefSystemSlugFactory
  {
    public static List<BeliefSystemSlug> CreateList(string[] newSlugs)
    {
      return newSlugs.Select(Create).ToList();
    }

    public static BeliefSystemSlug Create(string newSlug)
    {
      BeliefSystemSlug newBeliefSystemSlug = new BeliefSystemSlug();
      newBeliefSystemSlug.Name = newSlug.Trim().NormalizeStringForUrl();
      newBeliefSystemSlug.CreatedAt = DateTime.UtcNow;

      return newBeliefSystemSlug;
    }

    public static BeliefSystemSlug Create(string newSlug, bool isDefault)
    {
      BeliefSystemSlug newBeliefSystemSlug = new BeliefSystemSlug();
      newBeliefSystemSlug.Name = newSlug.Trim().NormalizeStringForUrl();
      newBeliefSystemSlug.IsDefault = isDefault;
      newBeliefSystemSlug.CreatedAt = DateTime.UtcNow;

      return newBeliefSystemSlug;
    }
  }
}
