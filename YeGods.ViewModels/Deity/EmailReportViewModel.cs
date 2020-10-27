namespace YeGods.ViewModels
{
  using System;
  using System.ComponentModel.DataAnnotations;

  public class EmailReportViewModel
  {
    public string EntrySlug { get; set; }

    public string Subject { get; set; }

    public string Message { get; set; }

    public string EntryName { get; set; }

    public bool IsOfTypeDeity { get; set; }

    [Display(Name = "Your Name")]
    public string ReporterName { get; set; }

    [Display(Name = "Your Email")]
    public string ReporterEmail { get; set; }

    public DateTime CreatedAt { get; set; }
  }
}
