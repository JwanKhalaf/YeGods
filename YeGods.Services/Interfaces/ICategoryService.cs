namespace YeGods.Services
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using ViewModels;

  public interface ICategoryService
  {
    Task<List<CategoryViewModel>> GetAllCategories();
    Task<List<CategoryViewModel>> GetAllCategoriesExcept(int categoryId);
    Task<CategoryPageViewModel> GetPagedCategoriesAsync(bool showDeleted, int page);
    Task<CategoryViewModel> GetCategoryByIdAsync(int id);
    Task<CategoryUpdateViewModel> GetCategoryByIdForUpdateAsync(int id);
    Task<CategoryDeleteViewModel> GetCategoryByIdForDeleteAsync(int id);
    Task<CategoryViewModel> GetCategoryBySlugAsync(string slug);
    Task CreateNewCategoryAsync(CategoryCreateViewModel newCategory);
    Task UpdateCateogryAsync(CategoryUpdateViewModel updatedCategory);
    Task DeleteCategoryAsync(int id);
  }
}
