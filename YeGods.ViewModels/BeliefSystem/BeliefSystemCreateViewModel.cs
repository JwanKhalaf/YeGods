namespace YeGods.ViewModels
{
  using System.ComponentModel.DataAnnotations;

  public class BeliefSystemCreateViewModel
  {
    public string Name { get; set; }

    public string Aliases { get; set; }

    [Display(Name = "Geographical Region")]
    public string GeographicalRegion { get; set; }

    public string Description { get; set; }
  }
}
