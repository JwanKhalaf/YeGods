namespace YeGods.Services
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using DataAccess;
  using Domain;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Options;
  using ViewModels;

  public class CategoryService : ICategoryService
  {
    private readonly int recordsPerPage;

    private readonly YeGodsContext yeGodsContext;

    public CategoryService(
      IOptions<PaginationSettings> paginationSettings,
      YeGodsContext yeGodsContext)
    {
      this.recordsPerPage = paginationSettings.Value.RecordsPerPage;

      this.yeGodsContext = yeGodsContext ?? throw new ArgumentNullException(nameof(yeGodsContext));
    }

    public async Task<List<CategoryViewModel>> GetAllCategories()
    {
      IEnumerable<Category> categories = await this.yeGodsContext
        .Categories
        .Where(c => !c.IsDeleted)
        .OrderBy(c => c.Name)
        .AsNoTracking()
        .ToListAsync();

      return CategoryViewModelFactory.CreateList(categories);
    }

    public async Task<List<CategoryViewModel>> GetAllCategoriesExcept(int categoryId)
    {
      IEnumerable<Category> categories = await this.yeGodsContext
        .Categories
        .Where(c => !c.IsDeleted && c.Id != categoryId)
        .AsNoTracking()
        .ToListAsync();

      return CategoryViewModelFactory.CreateList(categories);
    }

    public async Task<CategoryPageViewModel> GetPagedCategoriesAsync(bool showDeleted, int page)
    {
      int totalNumberOfRecords;
      IQueryable<Category> categories = this.yeGodsContext.Categories
        .AsNoTracking()
        .AsQueryable();

      if (!showDeleted)
      {
        totalNumberOfRecords = this.yeGodsContext.Categories.Count(c => !c.IsDeleted);
        categories = categories.Where(c => !c.IsDeleted);
      }
      else
      {
        totalNumberOfRecords = this.yeGodsContext.Categories.Count(c => c.IsDeleted);
      }

      IEnumerable<Category> pagedCategories = await categories
        .Skip(this.recordsPerPage * (page - 1))
        .Take(this.recordsPerPage)
        .ToListAsync();

      IEnumerable<CategoryViewModel> categoriesAsViewModels = CategoryViewModelFactory.CreateList(pagedCategories);

      foreach (CategoryViewModel categoryViewModel in categoriesAsViewModels)
      {
        if(categoryViewModel.CategoryId != 0)
        {
          categoryViewModel.ParentCategoryName = this.yeGodsContext.Categories.Find(categoryViewModel.CategoryId).Name;
        }
      }

      CategoryPageViewModel categoriesPage = new CategoryPageViewModel();

      PaginationViewModel pagination = PaginationViewModelFactory
        .Create(this.recordsPerPage, totalNumberOfRecords, page);

      categoriesPage.Categories = categoriesAsViewModels;
      categoriesPage.Pagination = pagination;

      return categoriesPage;

    }

    public Task<CategoryViewModel> GetCategoryByIdAsync(int id)
    {
      throw new NotImplementedException();
    }

    public async Task<CategoryUpdateViewModel> GetCategoryByIdForUpdateAsync(int id)
    {
      Category category = await this.yeGodsContext.Categories.FindAsync(id);
      if (category == null) throw new CategoryNotFoundException(id, $"Could not find category with Id: {id}.");
      return category.ToCategoryUpdateViewModel();
    }

    public Task<CategoryViewModel> GetCategoryBySlugAsync(string slug)
    {
      throw new NotImplementedException();
    }

    public async Task CreateNewCategoryAsync(CategoryCreateViewModel newCategory)
    {
      Category category = new Category();

      category.Name = newCategory.Name;

      category.CreatedAt = DateTime.UtcNow;

      this.yeGodsContext
        .Categories
        .Add(category);

      await this.yeGodsContext.SaveChangesAsync();
    }

    public async Task UpdateCateogryAsync(CategoryUpdateViewModel updatedCategory)
    {
      Category category = this.yeGodsContext
        .Categories
        .Find(updatedCategory.Id);

      category.Name = updatedCategory.Name;

      category.ModifiedAt = DateTime.UtcNow;

      this.yeGodsContext.Entry(category).State = EntityState.Modified;

      await this.yeGodsContext.SaveChangesAsync();
    }

    public async Task<CategoryDeleteViewModel> GetCategoryByIdForDeleteAsync(int id)
    {
      CategoryDeleteViewModel categoryToDelete = new CategoryDeleteViewModel();

      Category category = await this.yeGodsContext.Categories.FindAsync(id);

      if (category == null) return null;

      int entitiesCount = await this.yeGodsContext
        .Deities
        .AsNoTracking()
        .Where(d => d.CategoryId == category.Id)
        .CountAsync();

      categoryToDelete.Id = category.Id;
      categoryToDelete.Name = category.Name;
      categoryToDelete.EntitiesCount = entitiesCount;

      return categoryToDelete;
    }

    public async Task DeleteCategoryAsync(int id)
    {
      Category category = await this.yeGodsContext.Categories.FindAsync(id);

      if (category == null)
      {
        throw new CategoryNotFoundException(id, $"Could not find category with Id {id}");
      }

      bool categoryIsAssociatedWithDeities = this.yeGodsContext.Deities.Any(d => d.CategoryId == id);

      if (categoryIsAssociatedWithDeities)
      {
        throw new InvalidOperationException("Cannot delete category, it has deities assigned to it.");
      }

      category.IsDeleted = true;
      category.ModifiedAt = DateTime.UtcNow;
      this.yeGodsContext.Entry(category).State = EntityState.Modified;
      await this.yeGodsContext.SaveChangesAsync();
    }
  }
}
