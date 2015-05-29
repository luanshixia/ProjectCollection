using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.Net.Http.Headers;

namespace UploadFile.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }

        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Import([ImportModelBinder] ImportSource source)
        {
            if (source != null && source.FileName != null)
            {
                ViewBag.Message = $"{source.FileName} uploaded, sized {source.FileStream.Length}.";
            }
            return View();
        }
    }

    public class ImportSource
    {
        public string FileName { get; set; }
        public System.IO.Stream FileStream { get; set; }
    }

    public class ImportSourceModelBinder : IModelBinder
    {
        public async Task<ModelBindingResult> BindModelAsync(ModelBindingContext bindingContext)
        {
            var request = bindingContext.OperationBindingContext.HttpContext.Request;
            var form = await request.ReadFormAsync();
            var result = new ImportSource();
            if (form.Files.Count > 0)
            {
                var file = form.Files[0];
                result.FileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
                result.FileStream = file.OpenReadStream();
            }
            return new ModelBindingResult(result, "importSource", result.FileName != null);
        }
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class ImportModelBinderAttribute : ModelBinderAttribute
    {
        public ImportModelBinderAttribute()
            : base()
        {
            BinderType = typeof(ImportSourceModelBinder);
        }
    }
}
