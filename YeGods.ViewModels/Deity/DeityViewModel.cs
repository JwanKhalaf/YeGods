namespace YeGods.ViewModels
{
  using System;
  using Common.Enums;

  public class DeityViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Aliases { get; set; }
    public string Origin { get; set; }
    public string Description { get; set; }
    public Sex Sex { get; set; }
    public CategoryViewModel Category { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public bool IsDeleted { get; set; }
  }
}
