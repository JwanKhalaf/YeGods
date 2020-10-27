namespace YeGods.Importer
{
  using DataAccess;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Design;
  using Microsoft.Extensions.Configuration;

  public class YeGodsContextFactory : IDesignTimeDbContextFactory<YeGodsContext>
  {
    private static string _connectionString;

    public YeGodsContext CreateDbContext()
    {
      return this.CreateDbContext(null);
    }

    public YeGodsContext CreateDbContext(string[] args)
    {
      if (string.IsNullOrEmpty(_connectionString))
      {
        LoadConnectionString();
      }

      DbContextOptionsBuilder<YeGodsContext> builder = new DbContextOptionsBuilder<YeGodsContext>();

      builder.UseNpgsql(_connectionString);

      return new YeGodsContext(builder.Options);
    }

    private static void LoadConnectionString()
    {
      ConfigurationBuilder builder = new ConfigurationBuilder();

      builder.AddJsonFile("appsettings.json", optional: false);

      IConfigurationRoot configuration = builder.Build();

      _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
  }
}
