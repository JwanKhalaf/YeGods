namespace YeGods.Domain
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table("BeliefSystemAliases")]
  public class BeliefSystemAlias : BaseEntity
  {
    public string Name { get; set; }

    public int BeliefSystemId { get; set; }

    [ForeignKey("BeliefSystemId")]
    public BeliefSystem BeliefSystem { get; set; }

    public override bool Equals(object obj)
    {
      if (!(obj is BeliefSystemAlias))
      {
        return false;
      }

      BeliefSystemAlias beliefSystemAlias = (BeliefSystemAlias)obj;

      return this.Name == beliefSystemAlias.Name && this.BeliefSystemId == beliefSystemAlias.BeliefSystemId && this.CreatedAt == beliefSystemAlias.CreatedAt;
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(this.Name, this.BeliefSystemId, this.BeliefSystem);
    }
  }
}
