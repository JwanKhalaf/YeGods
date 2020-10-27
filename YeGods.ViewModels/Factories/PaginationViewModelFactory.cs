namespace YeGods.ViewModels
{
  public class PaginationViewModelFactory
  {
    public static PaginationViewModel Create(int recordsPerPage, int totalNumberOfRecords, int pageNumber)
    {
      PaginationViewModel pagination = new PaginationViewModel();
      pagination.RecordsPerPage = recordsPerPage;
      pagination.TotalRecords = totalNumberOfRecords;
      pagination.CurrentPage = pageNumber;
      return pagination;
    }
  }
}
