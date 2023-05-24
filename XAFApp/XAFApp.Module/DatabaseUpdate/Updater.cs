using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.EF;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using DevExpress.ExpressApp.SystemModule;
using XAFApp.Module.BusinessObjects;

namespace XAFApp.Module.DatabaseUpdate;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Updating.ModuleUpdater
public class Updater : ModuleUpdater {
  public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
      base(objectSpace, currentDBVersion) {
    Console.WriteLine("Updater ctor, currentDBVersion: " + currentDBVersion);
  }
  public override void UpdateDatabaseAfterUpdateSchema() {
    base.UpdateDatabaseAfterUpdateSchema();

    var defaultRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(role => role.Name == "Default");
    if (defaultRole == null) {
      defaultRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
      defaultRole.Name = "Default";

      defaultRole.AddObjectPermissionFromLambda<ApplicationUser>(SecurityOperations.Read, cm => cm.ID == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
      defaultRole.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
      defaultRole.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", cm => cm.ID == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
      defaultRole.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "StoredPassword", cm => cm.ID == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
      defaultRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Deny);
      defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
      defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
      defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
      defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);

      defaultRole.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/SaleProduct_ListView", SecurityPermissionState.Allow);
      defaultRole.AddTypePermissionsRecursively<SaleProduct>(SecurityOperations.FullAccess, SecurityPermissionState.Allow);
    }

    var user = ObjectSpace.FirstOrDefault<ApplicationUser>(u => u.UserName == "user");
    if (user == null) {
      user = ObjectSpace.CreateObject<ApplicationUser>();
      user.UserName = "user";
      user.SetPassword("user");

      ObjectSpace.CommitChanges();
      ((ISecurityUserWithLoginInfo)user).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(user));
      user.Roles.Add(defaultRole);
    }

    var adminRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(r => r.Name == "Administrators");
    if (adminRole == null) {
      adminRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
      adminRole.Name = "Administrators";
      adminRole.IsAdministrative = true;
    }

    var admin = ObjectSpace.FirstOrDefault<ApplicationUser>(u => u.UserName == "admin");
    if (admin == null) {
      admin = ObjectSpace.CreateObject<ApplicationUser>();
      admin.UserName = "admin";
      admin.SetPassword("admin");

      ObjectSpace.CommitChanges();
      ((ISecurityUserWithLoginInfo)admin).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(admin));
      admin.Roles.Add(adminRole);
    }

    ObjectSpace.CommitChanges();


    var rubberChicken = ObjectSpace.FirstOrDefault<SaleProduct>(p => p.Name == "Rubber Chicken");
    if (rubberChicken == null) {
      // we assume that the demo data doesn't exist yet
      rubberChicken = ObjectSpace.CreateObject<SaleProduct>();
      rubberChicken.Name = "Rubber Chicken";
      rubberChicken.Price = 13.99m;
      var pulley = ObjectSpace.CreateObject<SaleProduct>();
      pulley.Name = "Pulley";
      pulley.Price = 3.99m;
      var enterprise = ObjectSpace.CreateObject<SaleProduct>();
      enterprise.Name = "Starship Enterprise";
      enterprise.Price = 149999999.99m;
      var lostArk = ObjectSpace.CreateObject<SaleProduct>();
      lostArk.Name = "The Lost Ark";
      lostArk.Price = 1000000000000m;
    }

    ObjectSpace.CommitChanges();
  }
  public override void UpdateDatabaseBeforeUpdateSchema() {
    base.UpdateDatabaseBeforeUpdateSchema();
  }
}
