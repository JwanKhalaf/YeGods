namespace YeGods.Web.Controllers
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Services;
  using ViewModels;

  public class HomeController : Controller
  {
    private readonly IGlossaryService glossaryService;

    private readonly IDeityService deityService;

    private readonly IEmailService emailService;

    public HomeController(
      IGlossaryService glossaryService,
      IDeityService deityService,
      IEmailService emailService)
    {
      this.glossaryService = glossaryService;
      this.deityService = deityService;
      this.emailService = emailService;
    }

    public async Task<IActionResult> Index()
    {
      if (TempData["NotFound"] != null)
      {
        this.ViewBag.NotFoundMessage = TempData["NotFound"].ToString();
      }
      
      this.ViewData["Title"] = "Home";

      RandomDeityViewModel randomDeity = await this.deityService
        .GetRandomDeity();

      return this.View(randomDeity);
    }

    public IActionResult About()
    {
      this.ViewData["Title"] = "About";
      return this.View();
    }

    public IActionResult Contact()
    {
      return this.View();
    }

    public async Task<IActionResult> Glossary()
    {
      this.ViewData["Title"] = "Glossary";

      Dictionary<char, List<GlossaryViewModel>> glossaries = await this.glossaryService
        .GetGlossaryByLetterGroupingAsync();

      return this.View(glossaries);
    }

    public IActionResult Contribute()
    {
      this.ViewData["Title"] = "How to contribute";

      return this.View();
    }

    public IActionResult SuggestEntry()
    {
      return this.View();
    }

    public IActionResult Acknowledgements()
    {
      return View();
    }

    public IActionResult PrivacyPolicy()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> SuggestEntry(EntrySuggestionViewModel newSuggestion)
    {
      newSuggestion.CreatedAt = DateTime.UtcNow;

      await this.emailService.SendEmailAboutNewSuggestion(newSuggestion);

      return this.Redirect("/home/index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
    }

    public IActionResult NotFound()
    {
      return this.View();
    }
  }
}
