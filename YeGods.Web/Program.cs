namespace YeGods.Web
{
  using Microsoft.AspNetCore;
  using Microsoft.AspNetCore.Hosting;

  public class Program
  {
    public static void Main(string[] args)
    {
      IWebHost host = BuildWebHost(args);

      host.Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
      WebHost
        .CreateDefaultBuilder(args)
        .UseStartup<Startup>()
        .Build();
  }
}
