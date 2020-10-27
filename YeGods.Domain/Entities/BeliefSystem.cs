namespace YeGods.Domain
{
  using System.Collections.Generic;

  public class BeliefSystem : BaseEntity
  {
    public string Name { get; set; }

    public string GeographicalRegion { get; set; }

    public string Description { get; set; }

    public ICollection<BeliefSystemSlug> Slugs { get; set; }

    public ICollection<BeliefSystemAlias> Aliases { get; set; }
  }
}
