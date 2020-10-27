namespace YeGods.Domain
{
  using System.Collections.Generic;
  using Common.Extensions;

  public static class BeliefSystemHelpers
  {
    public static string[] GetSlugs(this BeliefSystem beliefSystem)
    {
      List<string> slugs = new List<string>();

      slugs.Add(beliefSystem.Name.NormalizeStringForUrl());

      foreach (BeliefSystemAlias beliefSystemAlias in beliefSystem.Aliases)
      {
        if (!beliefSystemAlias.IsDeleted)
        {
          slugs.Add(beliefSystemAlias.Name.NormalizeStringForUrl());
        }
      }

      return slugs.ToArray();
    }
  }
}
