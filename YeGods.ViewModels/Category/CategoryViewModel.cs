namespace YeGods.ViewModels
{
  using System;

  public class CategoryViewModel
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public string Slug { get; set; }

    public string ParentCategoryName { get; set; }

    public int CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }
  }
}
