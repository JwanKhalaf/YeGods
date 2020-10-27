namespace YeGods.Services
{
  using System;

  public class GlossaryNotFoundException : Exception
  {
    public GlossaryNotFoundException(int glossaryId, string message)
      : base(message)
    {
      this.GlossaryId = glossaryId;
    }

    public int GlossaryId { get; }
  }
}
