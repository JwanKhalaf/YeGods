namespace YeGods.Services
{
  using System;

  public class DeityNotFoundException : Exception
  {
    public DeityNotFoundException(int deityId, string message)
      : base(message)
    {
      this.DeityId = deityId;
    }

    public int DeityId { get; }
  }
}
