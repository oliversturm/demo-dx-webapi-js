using DevExpress.EntityFrameworkCore.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.EFCore;
using DevExpress.ExpressApp.Security;
using Microsoft.EntityFrameworkCore;

namespace XAFApp.WebApi.Core;

public sealed class ObjectSpaceProviderFactory : IObjectSpaceProviderFactory {
  readonly ISecurityStrategyBase security;
  readonly ITypesInfo typesInfo;
  readonly IDbContextFactory<XAFApp.Module.BusinessObjects.XAFAppEFCoreDbContext> dbFactory;

  public ObjectSpaceProviderFactory(ISecurityStrategyBase security, ITypesInfo typesInfo, IDbContextFactory<XAFApp.Module.BusinessObjects.XAFAppEFCoreDbContext> dbFactory) {
    this.security = security;
    this.typesInfo = typesInfo;
    this.dbFactory = dbFactory;
  }

  IEnumerable<IObjectSpaceProvider> IObjectSpaceProviderFactory.CreateObjectSpaceProviders() {
    yield return new SecuredEFCoreObjectSpaceProvider<XAFApp.Module.BusinessObjects.XAFAppEFCoreDbContext>((ISelectDataSecurityProvider)security, dbFactory, typesInfo);
    yield return new NonPersistentObjectSpaceProvider(typesInfo, null);
  }
}
