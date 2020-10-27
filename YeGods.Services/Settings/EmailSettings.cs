namespace YeGods.Services
{
  public class EmailSettings
  {
    public string SiteAdminEmail { get; set; }
    public string SmtpHost { get; set; }
    public int SmtpHostPort { get; set; }
    public string SmtpServerUsername { get; set; }
    public string SmtpServerPassword { get; set; }
  }
}
