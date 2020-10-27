namespace YeGods.ViewModels
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Domain;

  public class CategoryViewModelFactory
  {
    public static List<CategoryViewModel> CreateList(IEnumerable<Category> categories)
    {
      if (categories == null) throw new ArgumentNullException(nameof(categories));
      return categories.Select(Create).ToList();
    }

    public static CategoryViewModel Create(Category category)
    {
      if (category == null) throw new ArgumentNullException(nameof(category));
      CategoryViewModel result = new CategoryViewModel();
      result.Id = category.Id;
      result.Name = category.Name;
      result.CreatedAt = category.CreatedAt;
      result.ModifiedAt = category.ModifiedAt;
      result.IsDeleted = category.IsDeleted;
      return result;
    }
  }
}
