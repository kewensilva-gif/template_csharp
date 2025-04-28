
namespace RO.DevTest.Persistence.Repositories;

public class PagedResult<T> {
  public PagedResult(IEnumerable<T> items, int pageNumber, int pageSize, int totalRecords)
  {
    Items = items;
    PageNumber = pageNumber;
    PageSize = pageSize;
    TotalRecords = totalRecords;
  }

  public IEnumerable<T> Items {get; set;}
  public int PageNumber {get; set;}
  public int PageSize {get; set;}
  public int TotalRecords {get; set;}
  public int TotalPages => (int)Math.Ceiling((double) TotalRecords / PageSize);
  public bool HasPreviousPage => PageNumber > 0;
  public bool HasNextPage => PageNumber < TotalPages;
}