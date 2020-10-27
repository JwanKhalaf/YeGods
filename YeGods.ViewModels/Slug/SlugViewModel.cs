namespace YeGods.ViewModels
{
  using System;

  public class SlugViewModel
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public bool IsDefault { get; set; }

    public int DeityId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }
  }
}
