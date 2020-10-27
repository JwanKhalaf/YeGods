namespace YeGods.Domain
{
  using System;
  using System.Collections.Generic;

  public static class BeliefSystemAliasFactory
  {
    public static List<BeliefSystemAlias> CreateList(string newAliases)
    {
      List<BeliefSystemAlias> newBeliefSystemAliases = new List<BeliefSystemAlias>();

      if (string.IsNullOrEmpty(newAliases))
      {
        return newBeliefSystemAliases;
      }

      string[] aliases = newAliases.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);      

      foreach (string alias in aliases)
      {
        BeliefSystemAlias newBeliefSystemAlias = new BeliefSystemAlias();
        newBeliefSystemAlias.Name = alias.Trim();
        newBeliefSystemAlias.CreatedAt = DateTime.UtcNow;

        newBeliefSystemAliases.Add(newBeliefSystemAlias);
      }

      return newBeliefSystemAliases;
    }
  }
}
