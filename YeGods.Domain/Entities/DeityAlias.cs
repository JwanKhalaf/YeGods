namespace YeGods.Domain
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;

  [Table("DeityAliases")]
  public class DeityAlias : BaseEntity
  {
    public string Name { get; set; }

    public int DeityId { get; set; }

    [ForeignKey("DeityId")]
    public Deity Deity { get; set; }

    public override bool Equals(object obj)
    {
      if (!(obj is DeityAlias))
      {
        return false;
      }

      DeityAlias deityAlias = (DeityAlias) obj;

      return this.Name == deityAlias.Name && this.DeityId == deityAlias.DeityId && this.CreatedAt == deityAlias.CreatedAt;
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(this.Name, this.DeityId, this.Deity);
    }
  }
}
