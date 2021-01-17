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
        ViewBag.NotFoundMessage = TempData["NotFound"].ToString();
      }
      
      ViewData["Title"] = "Home";

      RandomDeityViewModel randomDeity = await deityService
        .GetRandomDeity();

      return View(randomDeity);
    }

    public IActionResult About()
    {
      ViewData["Title"] = "About";
      return View();
    }

    public IActionResult Contact()
    {
      return View();
    }

    public async Task<IActionResult> Glossary()
    {
      ViewData["Title"] = "Glossary";

      Dictionary<char, List<GlossaryViewModel>> glossaries = await glossaryService
        .GetGlossaryByLetterGroupingAsync();

      return View(glossaries);
    }

    public IActionResult Contribute()
    {
      ViewData["Title"] = "How to contribute";

      return View();
    }

    public IActionResult SuggestEntry()
    {
      return View();
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

      await emailService.SendEmailAboutNewSuggestion(newSuggestion);

      return Redirect("/home/index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult NotFound()
    {
      return View();
    }
  }
}
