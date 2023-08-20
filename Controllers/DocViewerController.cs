using GroupDocs.Viewer;
using GroupDocs.Viewer.Options;
using GroupDocs.Viewer.Results;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace DocumentViewer.Application.Controllers;

public class DocViewerController : Controller
{
    private readonly IHostingEnvironment _hostingEnvironment;
    private string projectRootPath;
    private string outputPath;
    private string storagePath;
    List<string> lstFiles;

    public DocViewerController(IHostingEnvironment hostingEnvironment)
    {
        _hostingEnvironment = hostingEnvironment;
        projectRootPath = _hostingEnvironment.ContentRootPath;
        outputPath = Path.Combine(projectRootPath, "wwwroot/Content");
        storagePath = Path.Combine(projectRootPath, "storage");
        lstFiles = new List<string>();
    }

    public IActionResult Index()
    {
        var files = Directory.GetFiles(storagePath);
        foreach (var file in files)
        {
            lstFiles.Add(Path.GetFileName(file));
        }
        ViewBag.lstFiles = lstFiles;
        return View();
    }
    [HttpPost]
    public IActionResult OnPost(string FileName)
    {
        int pageCount = 0;
        string imageFilesFolder = Path.Combine(outputPath, Path.GetFileName(FileName).Replace(".", "_"));
        if (!Directory.Exists(imageFilesFolder))
        {
            Directory.CreateDirectory(imageFilesFolder);
        }
        string imageFilesPath = Path.Combine(imageFilesFolder, "page-{0}.png");
        using (Viewer viewer = new Viewer(Path.Combine(storagePath, FileName)))
        {
            //Get document info
            ViewInfo info = viewer.GetViewInfo(ViewInfoOptions.ForPngView(false));
            pageCount = info.Pages.Count;
            //Set options and render document
            PngViewOptions options = new PngViewOptions(imageFilesPath);
            viewer.View(options);
        }
        return new JsonResult(pageCount);
    }



}
