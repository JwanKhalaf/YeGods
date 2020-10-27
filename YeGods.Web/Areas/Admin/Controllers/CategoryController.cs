namespace YeGods.Web.Areas.Admin.Controllers
{
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.Rendering;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Services;
  using ViewModels;

  [Area("Admin")]
  [Route("admin/category")]
  public class CategoryController : Controller
  {
    private readonly ICategoryService categoryService;

    public CategoryController(ICategoryService categoryService)
    {
      this.categoryService = categoryService;
    }

    [Route("[action]")]
    public async Task<IActionResult> Page(bool showDeleted, int page = 1)
    {
      CategoryPageViewModel deities = await this.categoryService.GetPagedCategoriesAsync(showDeleted, page);
      return this.View(deities);
    }

    [Route("[action]")]
    public async Task<IActionResult> Create()
    {
      CategoryCreateViewModel newDeity = new CategoryCreateViewModel();
      newDeity.Categories = await this.GetCategories();
      return this.View(newDeity);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Create(CategoryCreateViewModel newCategory)
    {
      if (!this.ModelState.IsValid)
      {
        newCategory.Categories = await this.GetCategories();
        return this.View(newCategory);
      }

      await this.categoryService.CreateNewCategoryAsync(newCategory);
      return this.Redirect("Page");
    }

    [Route("[action]")]
    public async Task<IActionResult> Edit(int id)
    {
      CategoryUpdateViewModel categoryToUpdate = await this.categoryService
        .GetCategoryByIdForUpdateAsync(id);

      categoryToUpdate.Categories = await this.GetCategoriesExcept(categoryToUpdate.Id);

      return this.View(categoryToUpdate);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Edit(CategoryUpdateViewModel updatedCategory)
    {
      if (!this.ModelState.IsValid)
      {
        updatedCategory.Categories = await this.GetCategoriesExcept(updatedCategory.Id);

        return this.View(updatedCategory);
      }

      await this.categoryService
        .UpdateCateogryAsync(updatedCategory);

      return this.Redirect("Page");
    }

    [Route("[action]")]
    public async Task<IActionResult> Delete(int id)
    {
      CategoryDeleteViewModel categoryToDelete = await this.categoryService
        .GetCategoryByIdForDeleteAsync(id);

      if (categoryToDelete.EntitiesCount != 0)
      {
        this.ViewBag.Message = $"You cannot delete this category as there are {categoryToDelete.EntitiesCount} entities associated with it.";
      }

      return this.View(categoryToDelete);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Delete(CategoryDeleteViewModel category)
    {
      if (!this.ModelState.IsValid)
      {
        return this.View(category);
      }

      await this.categoryService.DeleteCategoryAsync(category.Id);
      return this.Redirect("Page");
    }

    #region Helpers
    private async Task<List<SelectListItem>> GetCategories()
    {
      List<CategoryViewModel> categories = await this.categoryService.GetAllCategories();
      return categories.Select(s => new SelectListItem
      {
        Text = s.Name,
        Value = s.Id.ToString()
      }).ToList();
    }

    private async Task<List<SelectListItem>> GetCategoriesExcept(int categoryId)
    {
      List<CategoryViewModel> categories = await this.categoryService.GetAllCategoriesExcept(categoryId);
      return categories.Select(s => new SelectListItem
      {
        Text = s.Name,
        Value = s.Id.ToString()
      }).ToList();
    }
    #endregion
  }
}
