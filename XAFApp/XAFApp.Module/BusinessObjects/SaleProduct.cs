using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.Validation;

namespace XAFApp.Module.BusinessObjects {
  [DefaultClassOptions]
  public class SaleProduct : BaseObject {
    public SaleProduct() {
    }

    [RuleUniqueValue]
    [RuleRequiredField]
    public virtual string Name { get; set; }

    [RuleRequiredField]
    [RuleValueComparison(ValueComparisonType.GreaterThan, 0)]
    public virtual decimal? Price { get; set; }
  }
}