using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(YeGods.Web.Areas.Identity.IdentityHostingStartup))]
namespace YeGods.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}
