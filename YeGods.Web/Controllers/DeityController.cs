namespace YeGods.Web.Controllers
{
  using System;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Services;
  using ViewModels;

  public class DeityController : Controller
  {
    private readonly IDeityService deityService;

    private readonly IEmailService emailService;

    public DeityController(
      IDeityService deityService,
      IEmailService emailService)
    {
      this.deityService = deityService;

      this.emailService = emailService;
    }

    [HttpGet("/deity/{slug}", Name = "Get deity by name")]
    public async Task<IActionResult> Details(string slug)
    {
      DeityViewModel requestedDeity = await deityService.GetDeityBySlugAsync(slug);

      if (requestedDeity == null)
      {
        return RedirectToAction("NotFound", "Home");
      }

      ViewData["Title"] = requestedDeity.Name;

      return View(requestedDeity);
    }

    [HttpGet("/deity/{slug}/report", Name = "Submit report on deity")]
    public async Task<IActionResult> SendEmail(string slug)
    {
      DeityViewModel requestedDeity = await deityService.GetDeityBySlugAsync(slug);

      if (requestedDeity == null) return NotFound();

      EmailReportViewModel emailReport = new EmailReportViewModel();

      emailReport.EntrySlug = requestedDeity.Slug;

      emailReport.EntryName = requestedDeity.Name;

      emailReport.IsOfTypeDeity = true;

      ViewData["Title"] = $"Report {requestedDeity.Name}";

      return View(emailReport);
    }

    [HttpPost("/deity/{slug}/report", Name = "Submit report on deity")]
    public async Task<IActionResult> SendEmail(EmailReportViewModel emailReport)
    {
      emailReport.CreatedAt = DateTime.UtcNow;
      
      await emailService.SendEmailAboutEntry(emailReport);

      return Redirect("/home/index");
    }
  }
}
