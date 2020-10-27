namespace YeGods.Tests
{
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.EntityFrameworkCore;
  using System;
  using NUnit.Framework;

  public abstract class DataAccessTestBase<T> where T : DbContext
  {
    protected IServiceProvider ServiceProvider { get; private set; }

    public ServiceCollection Services { get; private set; } = new ServiceCollection();

    public T DbInMemoryContext { get; private set; }

    [SetUp]
    public void SetUpDatabase()
    {
      this.Services.AddDbContext<T>(options =>
      {
        options.UseInMemoryDatabase("YeGodsContext" + Guid.NewGuid().ToString());
        options.EnableSensitiveDataLogging(true);
      });
    }

    protected void BuildServiceProvider()
    {
      this.ServiceProvider = this.Services.BuildServiceProvider();
      this.DbInMemoryContext = this.ServiceProvider.GetService<T>();
      this.DbInMemoryContext.Database.EnsureDeleted();
    }
  }
}
