using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PersonalSite.Pages
{
    public class ArticleModel : PageModel
    {
        private IHostingEnvironment _env;

        public string Name { get; set; }

        public string Markdown { get; set; }

        public string Html { get; set; }

        public ArticleModel(IHostingEnvironment env)
        {
            _env = env;
        }

        public async Task OnGetAsync()
        {
            Name = RouteData.Values["name"].ToString();
            Markdown = await System.IO.File.ReadAllTextAsync(System.IO.Path.Combine(_env.ContentRootPath, "Articles", $"{Name}.md"));
            Html = Markdig.Markdown.ToHtml(Markdown);
        }
    }
}