using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace XAFApp.Module.BusinessObjects;

[DefaultProperty(nameof(UserName))]
public class ApplicationUser : PermissionPolicyUser, ISecurityUserWithLoginInfo {
  public ApplicationUser() {
    UserLogins = new ObservableCollection<ApplicationUserLoginInfo>();
  }

  [Browsable(false)]
  [Aggregated]
  public virtual IList<ApplicationUserLoginInfo> UserLogins { get; set; }

  IEnumerable<ISecurityUserLoginInfo> IOAuthSecurityUser.UserLogins => UserLogins.OfType<ISecurityUserLoginInfo>();

  ISecurityUserLoginInfo ISecurityUserWithLoginInfo.CreateUserLoginInfo(string loginProviderName,
    string providerUserKey) {
    ApplicationUserLoginInfo result = ((IObjectSpaceLink)this).ObjectSpace.CreateObject<ApplicationUserLoginInfo>();
    result.LoginProviderName = loginProviderName;
    result.ProviderUserKey = providerUserKey;
    result.User = this;
    return result;
  }
}