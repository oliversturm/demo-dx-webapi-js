using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Objects;
using DevExpress.ExpressApp.Office;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using XAFApp.Module.BusinessObjects;
using Updater = XAFApp.Module.DatabaseUpdate.Updater;

namespace XAFApp.Module;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
public sealed class XAFAppModule : ModuleBase {
  public XAFAppModule() {
    // 
    // XAFAppModule
    // 

    AdditionalExportedTypes.Add(typeof(ApplicationUser));
    AdditionalExportedTypes.Add(typeof(PermissionPolicyRole));
    AdditionalExportedTypes.Add(typeof(ModelDifference));
    AdditionalExportedTypes.Add(typeof(ModelDifferenceAspect));

    RequiredModuleTypes.Add(typeof(SystemModule));
    RequiredModuleTypes.Add(typeof(SecurityModule));
    RequiredModuleTypes.Add(typeof(BusinessClassLibraryCustomizationModule));
    RequiredModuleTypes.Add(typeof(ValidationModule));
    RequiredModuleTypes.Add(typeof(ReportsModuleV2));
    RequiredModuleTypes.Add(typeof(OfficeModule));

    SecurityModule.UsedExportedTypes = UsedExportedTypes.Custom;
  }

  public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
    ModuleUpdater updater = new Updater(objectSpace, versionFromDB);
    return new[] { updater };
  }

  public override void Setup(XafApplication application) {
    base.Setup(application);
    // Manage various aspects of the application UI and behavior at the module level.
  }
}