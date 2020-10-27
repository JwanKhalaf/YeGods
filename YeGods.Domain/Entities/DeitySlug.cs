namespace YeGods.Domain
{
  using System.ComponentModel.DataAnnotations.Schema;

  [Table("DeitySlugs")]
  public class DeitySlug : BaseEntity
  {
    public string Name { get; set; }

    public bool IsDefault { get; set; }

    public int DeityId { get; set; }

    [ForeignKey("DeityId")]
    public Deity Deity { get; set; }
  }
}
