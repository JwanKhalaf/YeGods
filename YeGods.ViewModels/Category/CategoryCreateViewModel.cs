namespace YeGods.ViewModels
{
  using System.Collections.Generic;
  using Microsoft.AspNetCore.Mvc.Rendering;

  public class CategoryCreateViewModel
  {
    public string Name { get; set; }

    public int CategoryId { get; set; }

    public List<SelectListItem> Categories { get; set; }
  }
}
