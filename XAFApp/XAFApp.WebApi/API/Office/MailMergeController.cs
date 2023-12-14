using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.WebApi.Services;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace XAFApp.WebApi.API.Office;

[Authorize]
[Route("api/[controller]")]
public class MailMergeController : ControllerBase {
  private readonly IDataService dataService;

  public MailMergeController(IDataService dataService) {
    this.dataService = dataService;
  }

  [HttpGet("MergeDocument({mailMergeId})/{objectIds?}")]
  public async Task<object> MergeDocument(
    [FromRoute] string mailMergeId,
    [FromRoute] string? objectIds) {
    // Fetch the mail merge data by the given ID
    IObjectSpace objectSpace = dataService.GetObjectSpace<RichTextMailMergeData>();
    RichTextMailMergeData mailMergeData =
      objectSpace.GetObjectByKey<RichTextMailMergeData>(new Guid(mailMergeId));

    // Fetch the list of objects by their IDs
    List<Guid> ids = objectIds?.Split(',').Select(s => new Guid(s)).ToList();
    IList dataObjects = ids != null
      ? objectSpace.GetObjects(mailMergeData.DataType, new InOperator("ID", ids))
      : objectSpace.GetObjects(mailMergeData.DataType);

    using RichEditDocumentServer server = new();
    server.Options.MailMerge.DataSource = dataObjects;
    server.Options.MailMerge.ViewMergedData = true;
    server.OpenXmlBytes = mailMergeData.Template;

    MailMergeOptions mergeOptions = server.Document.CreateMailMergeOptions();
    mergeOptions.MergeMode = MergeMode.NewSection;

    MemoryStream temp = new();
    server.Document.MailMerge(mergeOptions, temp, DocumentFormat.Doc);
    temp.Seek(0, SeekOrigin.Begin);

    using RichEditDocumentServer exporter = new();
    MemoryStream output = new();
    exporter.LoadDocument(temp);
    exporter.ExportToPdf(output);

    // Shorter attempt that does not work right at the moment.
    // MailMergeOptions mergeOptions = server.Document.CreateMailMergeOptions();
    // mergeOptions.MergeMode = MergeMode.NewSection;
    //
    // server.Document.MailMerge(mergeOptions, server.Document);
    // server.Document.MailMerge(server.Document);
    // MemoryStream output = new();
    // server.ExportToPdf(output);

    output.Seek(0, SeekOrigin.Begin);
    return File(output, "application/pdf", "MailMerge.pdf");
  }
}