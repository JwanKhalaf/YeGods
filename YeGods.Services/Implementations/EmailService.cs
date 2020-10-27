namespace YeGods.Services
{
  using System;
  using System.IO;
  using System.Net;
  using System.Net.Mail;
  using System.Threading.Tasks;
  using Microsoft.Extensions.Options;
  using RazorLight;
  using ViewModels;

  public class EmailService : IEmailService
  {
    private readonly string siteAdminEmail;
    private readonly string smtpHost;
    private readonly int smtpHostPort;
    private readonly string smtpServerUsername;
    private readonly string smtpServerPassword;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
      this.siteAdminEmail = emailSettings.Value.SiteAdminEmail;
      this.smtpHost = emailSettings.Value.SmtpHost;
      this.smtpHostPort = emailSettings.Value.SmtpHostPort;
      this.smtpServerUsername = emailSettings.Value.SmtpServerUsername;
      this.smtpServerPassword = emailSettings.Value.SmtpServerPassword;
    }

    public async Task SendEmailAboutEntry(EmailReportViewModel emailReport)
    {
      string emailTemplateFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates");

      string reportEmailTemplatePath = Path.Combine(emailTemplateFolderPath, "Report.cshtml");

      RazorLightEngine engine = new RazorLightEngineBuilder()
        .UseFilesystemProject(emailTemplateFolderPath)
        .UseMemoryCachingProvider()
        .Build();

      string emailHtmlBody = await engine.CompileRenderAsync(reportEmailTemplatePath, emailReport);

      SmtpClient smtpClient = new SmtpClient(this.smtpHost, this.smtpHostPort);

      smtpClient.Credentials = new NetworkCredential(
        this.smtpServerUsername,
        this.smtpServerPassword);

      MailMessage mailMessage = new MailMessage(
        emailReport.ReporterEmail,
        this.siteAdminEmail,
        emailReport.Subject,
        emailHtmlBody);

      mailMessage.IsBodyHtml = true;

      smtpClient.EnableSsl = true;

      smtpClient.Send(mailMessage);
    }

    public async Task SendEmailAboutNewSuggestion(EntrySuggestionViewModel newSuggestion)
    {
      newSuggestion.Subject = $"A new suggestion ({newSuggestion.EntryName}) has been submitted.";

      string emailTemplateFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates");

      string reportEmailTemplatePath = Path.Combine(emailTemplateFolderPath, "NewSuggestion.cshtml");

      RazorLightEngine engine = new RazorLightEngineBuilder()
        .UseFilesystemProject(emailTemplateFolderPath)
        .UseMemoryCachingProvider()
        .Build();

      string emailHtmlBody = await engine.CompileRenderAsync(reportEmailTemplatePath, newSuggestion);

      SmtpClient smtpClient = new SmtpClient(this.smtpHost, this.smtpHostPort);

      smtpClient.Credentials = new NetworkCredential(
        this.smtpServerUsername,
        this.smtpServerPassword);

      MailMessage mailMessage = new MailMessage(
        newSuggestion.ReporterEmail,
        this.siteAdminEmail,
        newSuggestion.Subject,
        emailHtmlBody);

      mailMessage.IsBodyHtml = true;

      smtpClient.EnableSsl = true;

      smtpClient.Send(mailMessage);
    }
  }
}
