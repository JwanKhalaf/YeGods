namespace YeGods.ViewModels
{
  using System.Collections.Generic;
  using YeGods.ViewModels.Shared;

  public class GlossaryPageViewModel
  {
    public SearchViewModel Search { get; set; }

    public IEnumerable<GlossaryViewModel> Glossaries { get; set; }

    public PaginationViewModel Pagination { get; set; }
  }
}
