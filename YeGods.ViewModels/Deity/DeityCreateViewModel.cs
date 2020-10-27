namespace YeGods.ViewModels
{
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using Common.Enums;
  using Microsoft.AspNetCore.Mvc.Rendering;

  public class DeityCreateViewModel
  {
    public string Name { get; set; }

    public string Aliases { get; set; }

    public string Origin { get; set; }

    public string Description { get; set; }

    public Sex Sex { get; set; }

    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    public List<SelectListItem> Categories { get; set; }
  }
}
