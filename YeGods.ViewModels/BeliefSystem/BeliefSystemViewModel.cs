namespace YeGods.ViewModels
{
  using System;

  public class BeliefSystemViewModel
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public string GeographicalRegion { get; set; }

    public string Description { get; set; }

    public string Slug { get; set; }

    public string Aliases { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }
  }
}
