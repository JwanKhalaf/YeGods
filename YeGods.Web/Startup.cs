namespace YeGods.Web
{
  using System;
  using DataAccess;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.HttpOverrides;
  using Microsoft.AspNetCore.Identity;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Services;

  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      this.Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      string databaseConnectionString = Configuration["Database:DefaultConnectionString"];

      Console.WriteLine($"Here is the connection string: {databaseConnectionString}");

      services
        .Configure<PaginationSettings>(this.Configuration.GetSection("PaginationSettings"));

      services
        .Configure<EmailSettings>(this.Configuration.GetSection("EmailSettings"));

      services
        .Configure<CookiePolicyOptions>(options =>
        {
          // This lambda determines whether user consent for non-essential cookies is needed for a given request.
          options.CheckConsentNeeded = context => true;
          options.MinimumSameSitePolicy = SameSiteMode.None;
        });

      services
        .AddDbContext<YeGodsContext>(options => options.UseNpgsql(databaseConnectionString));

      services
        .AddDefaultIdentity<IdentityUser>()
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<YeGodsContext>();

      services
        .AddScoped<RoleManager<IdentityRole>>();

      services
        .AddScoped<IDeityService, DeityService>();

      services
        .AddScoped<IBeliefSystemService, BeliefSystemService>();

      services
        .AddScoped<ICategoryService, CategoryService>();

      services
        .AddScoped<IEmailService, EmailService>();

      services
        .AddScoped<ISearchService, SearchService>();

      services
        .AddScoped<IGlossaryService, GlossaryService>();

      services
        .AddMvc()
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

      services
        .AddCloudscribePagination();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseStaticFiles();
      app.UseCookiePolicy();

      app.UseForwardedHeaders(new ForwardedHeadersOptions
      {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
      });

      app.UseAuthentication();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
          name: "areaRoute",
          template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

        routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
