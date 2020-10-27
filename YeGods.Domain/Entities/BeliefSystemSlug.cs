namespace YeGods.Domain
{
  using System.ComponentModel.DataAnnotations.Schema;

  [Table("BeliefSystemSlugs")]
  public class BeliefSystemSlug : BaseEntity
  {
    public string Name { get; set; }

    public bool IsDefault { get; set; }

    public int BeliefSystemId { get; set; }

    [ForeignKey("BeliefSystemId")]
    public BeliefSystem BeliefSystem { get; set; }
  }
}
