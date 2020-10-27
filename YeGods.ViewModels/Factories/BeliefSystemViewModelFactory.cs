namespace YeGods.ViewModels
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Common.Extensions;
  using Domain;

  public class BeliefSystemViewModelFactory
  {
    public static List<BeliefSystemViewModel> CreateList(IEnumerable<BeliefSystem> beliefSystems)
    {
      if (beliefSystems == null) throw new ArgumentNullException(nameof(beliefSystems));

      return beliefSystems.Select(Create).ToList();
    }

    public static BeliefSystemViewModel Create(BeliefSystem beliefSystem)
    {
      if (beliefSystem == null) throw new ArgumentNullException(nameof(beliefSystem));

      BeliefSystemSlug defaultBeliefSystemSlug = beliefSystem.Slugs.First(s => s.IsDefault);

      BeliefSystemViewModel result = new BeliefSystemViewModel();
      result.Id = beliefSystem.Id;
      result.Name = beliefSystem.Name;
      result.GeographicalRegion = beliefSystem.GeographicalRegion;
      result.Description = beliefSystem.Description.Truncate(90);
      result.Slug = defaultBeliefSystemSlug.Name;
      result.Aliases = string.Join(", ", beliefSystem.Aliases.Select(x => x.Name));
      result.CreatedAt = beliefSystem.CreatedAt;
      result.ModifiedAt = beliefSystem.ModifiedAt;
      result.IsDeleted = beliefSystem.IsDeleted;

      return result;
    }
  }
}
