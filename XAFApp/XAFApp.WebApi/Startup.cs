﻿using DevExpress.Persistent.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using DevExpress.ExpressApp.WebApi.Services;
using Microsoft.AspNetCore.OData;
using DevExpress.ExpressApp.Core;
using XAFApp.WebApi.Core;
using DevExpress.ExpressApp.AspNetCore.WebApi;
using Antlr4.StringTemplate;
using DevExpress.ExpressApp.Security.Authentication.ClientServer;
using XAFApp.WebApi.Security;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using DevExpress.ExpressApp.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using XAFApp.Module.BusinessObjects;

namespace XAFApp.WebApi;

public class Startup {
  public Startup(IConfiguration configuration) {
    Configuration = configuration;
  }

  public IConfiguration Configuration { get; }

  // This method gets called by the runtime. Use this method to add services to the container.
  // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
  public void ConfigureServices(IServiceCollection services) {
    services
        .AddScoped<IObjectSpaceProviderFactory, ObjectSpaceProviderFactory>()
        .AddSingleton<IWebApiApplicationSetup, WebApiApplicationSetup>();

    services.AddXafAspNetCoreSecurity(Configuration, options => {
      options.RoleType = typeof(PermissionPolicyRole);
      options.UserType = typeof(ApplicationUser);
      options.UserLoginInfoType = typeof(ApplicationUserLoginInfo);
      // in XPO applications, uncomment the following line
      // options.Events.OnSecurityStrategyCreated = securityStrategy => ((SecurityStrategy)securityStrategy).RegisterXPOAdapterProviders();
      options.SupportNavigationPermissionsForTypes = false;
    })
    .AddAuthenticationStandard(options => {
      options.IsSupportChangePassword = true;
    });
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
          ValidIssuer = Configuration["Authentication:Jwt:Issuer"],
          ValidAudience = Configuration["Authentication:Jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authentication:Jwt:IssuerSigningKey"]))
        };
      })
      .AddCookie(options => {
        // options.Cookie.Name = "XAFDemoAppCookie";
        // options.Cookie.HttpOnly = true;
        // options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
        // options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
        // options.Cookie.IsEssential = true;
        // options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        // options.LoginPath = "/Account/Login";
        // options.LogoutPath = "/Account/Logout";
        // options.AccessDeniedPath = "/Account/AccessDenied";
        // options.SlidingExpiration = true;
      });

    services.AddAuthorization(options => {
      options.DefaultPolicy = new AuthorizationPolicyBuilder(
          JwtBearerDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme)
              .RequireAuthenticatedUser()
              .RequireXafAuthentication()
              .Build();
    });

    services.AddDbContextFactory<XAFApp.Module.BusinessObjects.XAFAppEFCoreDbContext>((serviceProvider, options) => {
      // Uncomment this code to use an in-memory database. This database is recreated each time the server starts. With the in-memory database, you don't need to make a migration when the data model is changed.
      // Do not use this code in production environment to avoid data loss.
      // We recommend that you refer to the following help topic before you use an in-memory database: https://docs.microsoft.com/en-us/ef/core/testing/in-memory
      //options.UseInMemoryDatabase("InMemory");
      var connectionStringTemplate = new Template(Configuration.GetConnectionString("ConnectionString"));
      connectionStringTemplate.Add("SQL_DBNAME", System.Environment.GetEnvironmentVariable("SQL_DBNAME"));
      connectionStringTemplate.Add("SQL_SA_PASSWD", System.Environment.GetEnvironmentVariable("SQL_SA_PASSWD"));
      options.UseSqlServer(connectionStringTemplate.Render());
      options.UseChangeTrackingProxies();
      options.UseObjectSpaceLinkProxies();
      options.UseLazyLoadingProxies();
      options.UseSecurity(serviceProvider);
    }, ServiceLifetime.Scoped);

    services.AddScoped<IDataService, XAFApp.WebApi.Core.ValidatingDataService>();

    services
        .AddXafWebApi(Configuration, options => {
          // Make your business objects available in the Web API and generate the GET, POST, PUT, and DELETE HTTP methods for it.
          // options.BusinessObject<YourBusinessObject>();
          options.BusinessObject<XAFApp.Module.BusinessObjects.SaleProduct>();
        });
    services
        .AddControllers()
        .AddOData((options, serviceProvider) => {
          options
                  .AddRouteComponents("api/odata", new EdmModelBuilder(serviceProvider).GetEdmModel())
                  .EnableQueryFeatures(100);
        });

    services.AddSwaggerGen(c => {
      c.EnableAnnotations();
      c.SwaggerDoc("v1", new OpenApiInfo
      {
        Title = "XAFApp API",
        Version = "v1",
        Description = @"Use AddXafWebApi(options) in the XAFApp.WebApi\Startup.cs file to make Business Objects available in the Web API."
      });
    });
  }

  // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
  public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
    if (env.IsDevelopment()) {
      app.UseDeveloperExceptionPage();
      app.UseSwagger();
      app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "XAFApp WebApi v1");
      });
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
    app.UseEndpoints(endpoints => {
      endpoints.MapControllers();
    });
  }
}
