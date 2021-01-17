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
      CategoryPageViewModel deities = await categoryService
        .GetPagedCategoriesAsync(showDeleted, page);

      return View(deities);
    }

    [Route("[action]")]
    public async Task<IActionResult> Create()
    {
      CategoryCreateViewModel newDeity = new CategoryCreateViewModel();

      newDeity.Categories = await GetCategories();

      return View(newDeity);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Create(CategoryCreateViewModel newCategory)
    {
      if (!ModelState.IsValid)
      {
        newCategory.Categories = await GetCategories();

        return View(newCategory);
      }

      await categoryService.CreateNewCategoryAsync(newCategory);

      return Redirect("Page");
    }

    [Route("[action]")]
    public async Task<IActionResult> Edit(int id)
    {
      CategoryUpdateViewModel categoryToUpdate = await categoryService
        .GetCategoryByIdForUpdateAsync(id);

      categoryToUpdate.Categories = await GetCategoriesExcept(categoryToUpdate.Id);

      return View(categoryToUpdate);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Edit(CategoryUpdateViewModel updatedCategory)
    {
      if (!ModelState.IsValid)
      {
        updatedCategory.Categories = await GetCategoriesExcept(updatedCategory.Id);

        return View(updatedCategory);
      }

      await categoryService
        .UpdateCateogryAsync(updatedCategory);

      return Redirect("Page");
    }

    [Route("[action]")]
    public async Task<IActionResult> Delete(int id)
    {
      CategoryDeleteViewModel categoryToDelete = await categoryService
        .GetCategoryByIdForDeleteAsync(id);

      if (categoryToDelete.EntitiesCount != 0)
      {
        ViewBag.Message = $"You cannot delete this category as there are {categoryToDelete.EntitiesCount} entities associated with it.";
      }

      return View(categoryToDelete);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Delete(CategoryDeleteViewModel category)
    {
      if (!ModelState.IsValid)
      {
        return View(category);
      }

      await categoryService.DeleteCategoryAsync(category.Id);

      return Redirect("Page");
    }

    #region Helpers
    private async Task<List<SelectListItem>> GetCategories()
    {
      List<CategoryViewModel> categories = await categoryService.GetAllCategories();
      return categories.Select(s => new SelectListItem
      {
        Text = s.Name,
        Value = s.Id.ToString()
      }).ToList();
    }

    private async Task<List<SelectListItem>> GetCategoriesExcept(int categoryId)
    {
      List<CategoryViewModel> categories = await categoryService.GetAllCategoriesExcept(categoryId);
      return categories.Select(s => new SelectListItem
      {
        Text = s.Name,
        Value = s.Id.ToString()
      }).ToList();
    }
    #endregion
  }
}
