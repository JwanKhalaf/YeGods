namespace YeGods.Services
{
  using System.Threading.Tasks;
  using ViewModels;

  public interface IEmailService
  {
    Task SendEmailAboutEntry(EmailReportViewModel emailReport);

    Task SendEmailAboutNewSuggestion(EntrySuggestionViewModel newSuggestion);
  }
}
