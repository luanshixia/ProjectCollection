using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BookKeeper
{
    public class BookEngine
    {
        public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.81 Safari/537.36";

        public const string Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";

        public const string AcceptEncoding = "gzip, deflate, br";

        public const string AcceptLanguage = "en-US,en;q=0.9,zh-CN;q=0.8,zh;q=0.7";

        public const string UriTemplate = "https://www.amazon.com/s/ref=nb_sb_noss_2?url=search-alias%3Dstripbooks&field-keywords={0}";

        public static async Task Search(string kwd)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(BookEngine.UserAgent);
                httpClient.DefaultRequestHeaders.Accept.ParseAdd(BookEngine.Accept);
                //httpClient.DefaultRequestHeaders.AcceptEncoding.ParseAdd(BookEngine.AcceptEncoding);
                httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(BookEngine.AcceptLanguage);

                var requestUri = string.Format(BookEngine.UriTemplate, kwd);
                var response = await httpClient.GetStringAsync(requestUri);

                var xd = XDocument.Parse(response);
            }
        }
    }
}
