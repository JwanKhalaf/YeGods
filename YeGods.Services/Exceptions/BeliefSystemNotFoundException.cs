namespace YeGods.Services
{
  using System;

  public class BeliefSystemNotFoundException : Exception
  {
    public BeliefSystemNotFoundException(
      int beliefSystemId,
      string message)
      : base(message)
    {
      this.BeliefSystemId = beliefSystemId;
    }

    public int BeliefSystemId { get; }
  }
}
