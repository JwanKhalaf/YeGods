namespace YeGods.Services
{
  using System;

  public class CategoryNotFoundException : Exception
  {
    public CategoryNotFoundException(
      int categoryId,
      string message)
      : base(message)
    {
      this.CategoryId = categoryId;
    }

    public int CategoryId { get; }
  }
}
