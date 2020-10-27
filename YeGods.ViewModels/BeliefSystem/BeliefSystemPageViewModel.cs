namespace YeGods.ViewModels
{
  using System.Collections.Generic;
  using YeGods.ViewModels.Shared;

  public class BeliefSystemPageViewModel
  {
    public SearchViewModel Search { get; set; }

    public IEnumerable<BeliefSystemViewModel> BeliefSystems { get; set; }

    public PaginationViewModel Pagination { get; set; }
  }
}
