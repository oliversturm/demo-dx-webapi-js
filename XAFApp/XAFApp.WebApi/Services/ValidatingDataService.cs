using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.WebApi.Services;
using DevExpress.Persistent.Validation;
using System.ComponentModel;

namespace XAFApp.WebApi.Core;

public class ValidatingDataService : DataService {
  private readonly IValidator validator;

  public ValidatingDataService(IObjectSpaceFactory objectSpaceFactory,
    ITypesInfo typesInfo, IValidator validator)
    : base(objectSpaceFactory, typesInfo) {
    this.validator = validator;
  }

  protected override IObjectSpace CreateObjectSpace(Type objectType) {
    IObjectSpace objectSpace = base.CreateObjectSpace(objectType);
    objectSpace.Committing += ObjectSpace_Committing;
    return objectSpace;
  }

  private void ObjectSpace_Committing(object? sender,
    CancelEventArgs e) {
    IObjectSpace os = (IObjectSpace)sender!;
    RuleSetValidationResult validationResult = validator.RuleSet.ValidateAllTargets(
      os, os.ModifiedObjects, DefaultContexts.Save
    );
    if (validationResult.ValidationOutcome == ValidationOutcome.Error) {
      throw new ValidationException(validationResult);
    }
  }
}