namespace YeGods.ViewModels
{
  using System.Collections.Generic;

  public class GlossaryLetterGroup
  {
    public char Letter { get; set; }
    public IEnumerable<GlossaryViewModel> Glossaries { get; set; }
  }
}
