namespace YeGods.Domain
{
  using System.Collections.Generic;
  using Common.Extensions;

  public static class DeityHelpers
  {
    public static string[] GetSlugs(this Deity deity)
    {
      List<string> slugs = new List<string>();

      slugs.Add(deity.Name.NormalizeStringForUrl());

      foreach (DeityAlias deityAlias in deity.Aliases)
      {
        if (!deityAlias.IsDeleted)
        {
          slugs.Add(deityAlias.Name.NormalizeStringForUrl());
        }
      }

      return slugs.ToArray();
    }
  }
}
