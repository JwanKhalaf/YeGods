namespace YeGods.ViewModels
{
  using System.Collections.Generic;
  using YeGods.ViewModels.Shared;

  public class DeityPageViewModel
  {
    public SearchViewModel Search { get; set; }

    public IEnumerable<DeityViewModel> Deities { get; set; }

    public PaginationViewModel Pagination { get; set; }
  }
}
