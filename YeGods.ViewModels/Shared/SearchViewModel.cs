using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace YeGods.ViewModels.Shared
{
  public class SearchViewModel
  {
    [Display(Name = "Search Term")]
    public string SearchTerm { get; set; }

    public string PreviousSearchTerm { get; set; }

    [Display(Name = "Show Deleted")]
    public bool ShowDeleted { get; set; }

    public bool SearchTermHasChanged()
    {
      return !string.Equals(this.SearchTerm, this.PreviousSearchTerm);
    }

    public bool IsNumeric()
    {
      return Regex.IsMatch(this.SearchTerm, @"^\d+$");
    }
  }
}
