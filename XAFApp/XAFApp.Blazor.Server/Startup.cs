using Antlr4.StringTemplate;
using DevExpress.ExpressApp.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using XAFApp.Blazor.Server.Services;
using XAFApp.Module;
using XAFApp.Module.BusinessObjects;

namespace XAFApp.Blazor.Server;

public class Startup {
  public Startup(IConfiguration configuration) {
    Configuration = configuration;
  }

  public IConfiguration Configuration { get; }

  // This method gets called by the runtime. Use this method to add services to the container.
  // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
  public void ConfigureServices(IServiceCollection services) {
    services.AddSingleton(typeof(HubConnectionHandler<>), typeof(ProxyHubConnectionHandler<>));

    services.AddRazorPages();
    services.AddServerSideBlazor();
    services.AddHttpContextAccessor();
    services.AddScoped<CircuitHandler, CircuitHandlerProxy>();
    services.AddXaf(Configuration, builder => {
      builder.UseApplication<XAFAppBlazorApplication>();
      builder.Modules
        .AddReports(options => {
          options.EnableInplaceReports = true;
          options.ReportDataType = typeof(ReportDataV2);
          options.ReportStoreMode = ReportStoreModes.XML;
        })
        .Add<XAFAppModule>()
        .Add<XAFAppBlazorModule>();
      builder.ObjectSpaceProviders
        .AddSecuredEFCore(options => options.PreFetchReferenceProperties())
        .WithDbContext<XAFAppEFCoreDbContext>((serviceProvider, options) => {
          // Uncomment this code to use an in-memory database. This database is recreated each time the server starts. With the in-memory database, you don't need to make a migration when the data model is changed.
          // Do not use this code in production environment to avoid data loss.
          // We recommend that you refer to the following help topic before you use an in-memory database: https://docs.microsoft.com/en-us/ef/core/testing/in-memory
          //options.UseInMemoryDatabase("InMemory");
          Template connectionStringTemplate = new(Configuration.GetConnectionString("ConnectionString"));
          connectionStringTemplate.Add("SQL_DBNAME", Environment.GetEnvironmentVariable("SQL_DBNAME"));
          connectionStringTemplate.Add("SQL_SA_PASSWD", Environment.GetEnvironmentVariable("SQL_SA_PASSWD"));
          options.UseSqlServer(connectionStringTemplate.Render());
          Console.WriteLine("Used SQL Server with connection string: " + connectionStringTemplate.Render());
          options.UseChangeTrackingProxies();
          options.UseObjectSpaceLinkProxies();
          options.UseLazyLoadingProxies();
        })
        .AddNonPersistent();
      builder.Security
        .UseIntegratedMode(options => {
          options.RoleType = typeof(PermissionPolicyRole);
          // ApplicationUser descends from PermissionPolicyUser and supports the OAuth authentication. For more information, refer to the following topic: https://docs.devexpress.com/eXpressAppFramework/402197
          // If your application uses PermissionPolicyUser or a custom user type, set the UserType property as follows:
          options.UserType = typeof(ApplicationUser);
          // ApplicationUserLoginInfo is only necessary for applications that use the ApplicationUser user type.
          // If you use PermissionPolicyUser or a custom user type, comment out the following line:
          options.UserLoginInfoType = typeof(ApplicationUserLoginInfo);
        })
        .AddPasswordAuthentication(options => {
          options.IsSupportChangePassword = true;
        });
    });

    AuthenticationBuilder authentication =
      services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);
    authentication
      .AddCookie(options => {
        options.LoginPath = "/LoginPage";
      });
  }

  // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
  public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
    if (env.IsDevelopment()) {
      app.UseDeveloperExceptionPage();
    }
    else {
      app.UseExceptionHandler("/Error");
      // The default HSTS value is 30 days. To change this for production scenarios, see: https://aka.ms/aspnetcore-hsts.
      app.UseHsts();
    }

    //app.UseHttpsRedirection();
    app.UseRequestLocalization();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseXaf();
    app.UseEndpoints(endpoints => {
      endpoints.MapXafEndpoints();
      endpoints.MapBlazorHub();
      endpoints.MapFallbackToPage("/_Host");
      endpoints.MapControllers();
    });
  }
}