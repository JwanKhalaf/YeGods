namespace YeGods.ViewModels
{
  using System;

  public class GlossaryViewModel
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Origin { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public bool IsDeleted { get; set; }
  }
}
