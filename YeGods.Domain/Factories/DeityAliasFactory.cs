namespace YeGods.Domain
{
  using System;
  using System.Collections.Generic;

  public static class DeityAliasFactory
  {
    public static List<DeityAlias> CreateList(string newAliases)
    {
      string[] aliases = newAliases.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

      List<DeityAlias> newDeityAliases = new List<DeityAlias>();

      foreach (string alias in aliases)
      {
        DeityAlias newDeityAlias = new DeityAlias();

        newDeityAlias.Name = alias.Trim();

        newDeityAlias.CreatedAt = DateTime.UtcNow;

        newDeityAliases.Add(newDeityAlias);
      }

      return newDeityAliases;
    }
  }
}
