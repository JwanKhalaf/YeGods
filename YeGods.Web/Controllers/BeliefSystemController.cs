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
      BeliefSystemViewModel requestedBeliefSystem = await beliefSystemService
        .GetBeliefSystemBySlugAsync(slug);

      if (requestedBeliefSystem == null)
      {
        return RedirectToAction("NotFound", "Home");
      }

      ViewData["Title"] = requestedBeliefSystem.Name;

      return View(requestedBeliefSystem);
    }

    [HttpGet("/beliefsystem/{slug}/report", Name = "Submit report on a belief system")]
    public async Task<IActionResult> SendEmail(string slug)
    {
      BeliefSystemViewModel requestedBeliefSystem = await beliefSystemService
        .GetBeliefSystemBySlugAsync(slug);

      if (requestedBeliefSystem == null) return NotFound();

      EmailReportViewModel emailReport = new EmailReportViewModel();

      emailReport.EntrySlug = requestedBeliefSystem.Slug;

      emailReport.EntryName = requestedBeliefSystem.Name;

      emailReport.IsOfTypeDeity = false;

      ViewData["Title"] = $"Report {requestedBeliefSystem.Name}";

      return View(emailReport);
    }

    [HttpPost("/beliefsystem/{slug}/report", Name = "Submit report on a belief system")]
    public async Task<IActionResult> SendEmail(EmailReportViewModel emailReport)
    {
      emailReport.CreatedAt = DateTime.UtcNow;

      await emailService.SendEmailAboutEntry(emailReport);

      return Redirect("/home/index");
    }
  }
}
