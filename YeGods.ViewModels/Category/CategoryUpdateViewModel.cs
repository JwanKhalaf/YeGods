namespace YeGods.ViewModels
{
  using Microsoft.AspNetCore.Mvc.Rendering;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;

  public class CategoryUpdateViewModel
  {
    public int Id { get; set; }

    public string Name { get; set; }

    [Display(Name = "Parent Category")]
    public int CategoryId { get; set; }

    public List<SelectListItem> Categories { get; set; }
  }
}
