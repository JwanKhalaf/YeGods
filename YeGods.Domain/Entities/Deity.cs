namespace YeGods.Domain
{
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Linq;
  using Common.Enums;

  public class Deity : BaseEntity
  {
    public string Name { get; set; }

    public string Origin { get; set; }

    public string Description { get; set; }

    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public Category Category { get; set; }

    public Sex Sex { get; set; }

    public ICollection<DeitySlug> Slugs { get; set; }

    public ICollection<DeityAlias> Aliases { get; set; }

    public DeitySlug GetDefaultSlug()
    {
      DeitySlug slug = this.Slugs.FirstOrDefault(s => s.IsDefault && !s.IsDeleted);
      return slug;
    }
  }
}
