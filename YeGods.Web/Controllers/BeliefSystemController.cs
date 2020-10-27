namespace YeGods.Web.Controllers
{
  using System;
  using Microsoft.AspNetCore.Mvc;
  using Services;
  using System.Threading.Tasks;
  using ViewModels;

  public class BeliefSystemController : Controller
  {
    private readonly IBeliefSystemService beliefSystemService;

    private readonly IEmailService emailService;

    public BeliefSystemController(
      IBeliefSystemService beliefSystemService,
      IEmailService emailService)
    {
      this.beliefSystemService = beliefSystemService;
      this.emailService = emailService;
    }

    [HttpGet("/beliefsystem/{slug}", Name = "Get belief system by name")]
    public async Task<IActionResult> Details(string slug)
    {
      BeliefSystemViewModel requestedBeliefSystem = await this.beliefSystemService
        .GetBeliefSystemBySlugAsync(slug);

      if (requestedBeliefSystem == null)
      {
        return this.RedirectToAction("NotFound", "Home");
      }

      this.ViewData["Title"] = requestedBeliefSystem.Name;

      return this.View(requestedBeliefSystem);
    }

    [HttpGet("/beliefsystem/{slug}/report", Name = "Submit report on a belief system")]
    public async Task<IActionResult> SendEmail(string slug)
    {
      BeliefSystemViewModel requestedBeliefSystem = await this.beliefSystemService
        .GetBeliefSystemBySlugAsync(slug);

      if (requestedBeliefSystem == null) return this.NotFound();

      EmailReportViewModel emailReport = new EmailReportViewModel();

      emailReport.EntrySlug = requestedBeliefSystem.Slug;

      emailReport.EntryName = requestedBeliefSystem.Name;

      emailReport.IsOfTypeDeity = false;

      this.ViewData["Title"] = $"Report {requestedBeliefSystem.Name}";

      return this.View(emailReport);
    }

    [HttpPost("/beliefsystem/{slug}/report", Name = "Submit report on a belief system")]
    public async Task<IActionResult> SendEmail(EmailReportViewModel emailReport)
    {
      emailReport.CreatedAt = DateTime.UtcNow;

      await this.emailService.SendEmailAboutEntry(emailReport);

      return this.Redirect("/home/index");
    }
  }
}
