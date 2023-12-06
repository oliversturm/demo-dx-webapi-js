using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.EFCore.DesignTime;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace XAFApp.Module.BusinessObjects;

// This code allows our Model Editor to get relevant EF Core metadata at design time.
// For details, please refer to https://supportcenter.devexpress.com/ticket/details/t933891.
public class XAFAppContextInitializer : DbContextTypesInfoInitializerBase {
  protected override DbContext CreateDbContext() {
    DbContextOptionsBuilder<XAFAppEFCoreDbContext> optionsBuilder = new DbContextOptionsBuilder<XAFAppEFCoreDbContext>()
      .UseSqlServer(";")
      .UseChangeTrackingProxies()
      .UseObjectSpaceLinkProxies();
    return new XAFAppEFCoreDbContext(optionsBuilder.Options);
  }
}

//This factory creates DbContext for design-time services. For example, it is required for database migration.
public class XAFAppDesignTimeDbContextFactory : IDesignTimeDbContextFactory<XAFAppEFCoreDbContext> {
  public XAFAppEFCoreDbContext CreateDbContext(string[] args) {
    throw new InvalidOperationException(
      "Make sure that the database connection string and connection provider are correct. After that, uncomment the code below and remove this exception.");
    //var optionsBuilder = new DbContextOptionsBuilder<XAFAppEFCoreDbContext>();
    //optionsBuilder.UseSqlServer("Integrated Security=SSPI;Pooling=false;Data Source=(localdb)\\mssqllocaldb;Initial Catalog=XAFApp");
    //optionsBuilder.UseChangeTrackingProxies();
    //optionsBuilder.UseObjectSpaceLinkProxies();
    //return new XAFAppEFCoreDbContext(optionsBuilder.Options);
  }
}

[TypesInfoInitializer(typeof(XAFAppContextInitializer))]
public class XAFAppEFCoreDbContext : DbContext {
  public XAFAppEFCoreDbContext(DbContextOptions<XAFAppEFCoreDbContext> options) : base(options) {
  }
  //public DbSet<ModuleInfo> ModulesInfo { get; set; }

  public DbSet<SaleProduct> SaleProducts { get; set; }
  public DbSet<ReportDataV2> ReportDataV2 { get; set; }
  public DbSet<RichTextMailMergeData> RichTextMailMergeData { get; set; }

  public DbSet<ModelDifference> ModelDifferences { get; set; }
  public DbSet<ModelDifferenceAspect> ModelDifferenceAspects { get; set; }
  public DbSet<PermissionPolicyRole> Roles { get; set; }
  public DbSet<ApplicationUser> Users { get; set; }
  public DbSet<ApplicationUserLoginInfo> UserLoginInfos { get; set; }


  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    base.OnModelCreating(modelBuilder);
    modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotificationsWithOriginalValues);
    modelBuilder.Entity<ApplicationUserLoginInfo>(b => {
      b.HasIndex(nameof(ISecurityUserLoginInfo.LoginProviderName), nameof(ISecurityUserLoginInfo.ProviderUserKey))
        .IsUnique();
    });
    modelBuilder.Entity<ModelDifference>()
      .HasMany(t => t.Aspects)
      .WithOne(t => t.Owner)
      .OnDelete(DeleteBehavior.Cascade);
  }
}