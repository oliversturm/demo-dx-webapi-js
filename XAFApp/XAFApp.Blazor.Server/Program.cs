﻿using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.DesignTime;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.Utils;
using System.Reflection;

namespace XAFApp.Blazor.Server;

public class Program : IDesignTimeApplicationFactory {
  XafApplication IDesignTimeApplicationFactory.Create() {
    IHostBuilder hostBuilder = CreateHostBuilder(Array.Empty<string>());
    return DesignTimeApplicationFactoryHelper.Create(hostBuilder);
  }

  private static bool ContainsArgument(string[] args, string argument) {
    return args.Any(arg => arg.TrimStart('/').TrimStart('-').ToLower() == argument.ToLower());
  }

  public static int Main(string[] args) {
    if (ContainsArgument(args, "help") || ContainsArgument(args, "h")) {
      Console.WriteLine("Updates the database when its version does not match the application's version.");
      Console.WriteLine();
      Console.WriteLine(
        $"    {Assembly.GetExecutingAssembly().GetName().Name}.exe --updateDatabase [--forceUpdate --silent]");
      Console.WriteLine();
      Console.WriteLine(
        "--forceUpdate - Marks that the database must be updated whether its version matches the application's version or not.");
      Console.WriteLine(
        "--silent - Marks that database update proceeds automatically and does not require any interaction with the user.");
      Console.WriteLine();
      Console.WriteLine($"Exit codes: 0 - {DBUpdaterStatus.UpdateCompleted}");
      Console.WriteLine($"            1 - {DBUpdaterStatus.UpdateError}");
      Console.WriteLine($"            2 - {DBUpdaterStatus.UpdateNotNeeded}");
    }
    else {
      FrameworkSettings.DefaultSettingsCompatibilityMode = FrameworkSettingsCompatibilityMode.Latest;
      IHost host = CreateHostBuilder(args).Build();
      if (ContainsArgument(args, "updateDatabase")) {
        using (IServiceScope serviceScope = host.Services.CreateScope()) {
          return serviceScope.ServiceProvider.GetRequiredService<IDBUpdater>()
            .Update(ContainsArgument(args, "forceUpdate"), ContainsArgument(args, "silent"));
        }
      }

      host.Run();
    }

    return 0;
  }

  public static IHostBuilder CreateHostBuilder(string[] args) {
    return Host.CreateDefaultBuilder(args)
      .ConfigureWebHostDefaults(webBuilder => {
        webBuilder.UseStartup<Startup>();
      });
  }
}