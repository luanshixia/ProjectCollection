using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PersonalSite.Pages
{
    public class ArticleModel : PageModel
    {
        private IHostingEnvironment _env;

        public string Name { get; set; }

        public string Markdown { get; set; }

        public string Html { get; set; }

        public string Title { get; set; }

        public Dictionary<string, string> Metadata { get; set; }

        public ArticleModel(IHostingEnvironment env)
        {
            _env = env;
        }

        public async Task OnGetAsync()
        {
            Name = RouteData.Values["name"].ToString();
            var fileName = System.IO.Path.Combine(_env.ContentRootPath, "Articles", $"{Name}.md");
            Markdown = await System.IO.File.ReadAllTextAsync(fileName);
            Html = Markdig.Markdown.ToHtml(Markdown);
            Metadata = (await System.IO.File.ReadAllLinesAsync(fileName))
                .Take(100)
                .Select(line => Regex.Match(line, @"^\s*\[(?<key>[a-zA-Z0-9_]+)\]:\s*#\s*\((?<value>.*)\)\s*$"))
                .Where(match => match.Success)
                .ToDictionary(
                    match => match.Groups["key"].Value,
                    match => match.Groups["value"].Value);
            Title = Metadata.ContainsKey("Title") ? Metadata["Title"] : Name;
        }
    }
}