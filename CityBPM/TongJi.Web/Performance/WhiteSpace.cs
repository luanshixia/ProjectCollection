using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace TongJi.Web.Performance
{
    /// <summary>
    /// CompressWhitespaceFilter
    /// </summary>
    public class CompressWhitespaceFilter : Stream
    {
        private GZipStream _contentGZipStream;
        private DeflateStream _content_DeflateStream;
        private Stream _contentStream;
        private CompressOptions _compressOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompressWhitespaceFilter"/> class.
        /// </summary>
        /// <param name="contentStream">The content stream.</param>
        /// <param name="compressOptions">The compress options.</param>
        public CompressWhitespaceFilter(Stream contentStream, CompressOptions compressOptions)
        {
            if (compressOptions == CompressOptions.GZip)
            {
                this._contentGZipStream = new GZipStream(contentStream, CompressionMode.Compress);
                this._contentStream = this._contentGZipStream;
            }
            else if (compressOptions == CompressOptions.Deflate)
            {
                this._content_DeflateStream = new DeflateStream(contentStream, CompressionMode.Compress);
                this._contentStream = this._content_DeflateStream;
            }
            else
            {
                this._contentStream = contentStream;
            }
            this._compressOptions = compressOptions;
        }

        public override bool CanRead
        {
            get { return this._contentStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return this._contentStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return this._contentStream.CanWrite; }
        }

        public override void Flush()
        {
            this._contentStream.Flush();
        }

        public override long Length
        {
            get { return this._contentStream.Length; }
        }

        public override long Position
        {
            get
            {
                return this._contentStream.Position;
            }
            set
            {
                this._contentStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this._contentStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this._contentStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this._contentStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            byte[] data = new byte[count + 1];
            Buffer.BlockCopy(buffer, offset, data, 0, count);

            string strtext = System.Text.Encoding.UTF8.GetString(data);
            strtext = Regex.Replace(strtext, "^\\s*", string.Empty, RegexOptions.Compiled | RegexOptions.Multiline);
            strtext = Regex.Replace(strtext, "\\r\\n", string.Empty, RegexOptions.Compiled | RegexOptions.Multiline);
            strtext = Regex.Replace(strtext, "<!--*.*?-->", string.Empty, RegexOptions.Compiled | RegexOptions.Multiline);

            byte[] outdata = System.Text.Encoding.UTF8.GetBytes(strtext);
            this._contentStream.Write(outdata, 0, outdata.GetLength(0));
        }
    }

    /// <summary>
    /// CompressOptions
    /// </summary>
    /// <seealso cref="http://en.wikipedia.org/wiki/Zcat#gunzip_and_zcat"/>
    /// <seealso cref="http://en.wikipedia.org/wiki/DEFLATE"/>
    public enum CompressOptions
    {
        GZip,
        Deflate,
        None
    }

    /// <summary>
    /// CompressWhitespaceModule
    /// </summary>
    public class CompressWhitespaceModule : IHttpModule
    {
        #region IHttpModule Members

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose()
        {
            // Nothing to dispose; 
        }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication"/> that provides access to the methods, properties, and events common to all application objects within an ASP.NET ion</param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
        }

        /// <summary>
        /// Handles the BeginRequest event of the context control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void context_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            HttpContext context = app.Context;
            HttpRequest request = context.Request;
            if (FileTypeNeedToCompress(request.RawUrl))
            {
                string acceptEncoding = request.Headers["Accept-Encoding"];
                HttpResponse response = context.Response;
                if (response.ContentType == "text/html")
                {
                    if (!string.IsNullOrEmpty(acceptEncoding))
                    {
                        acceptEncoding = acceptEncoding.ToUpperInvariant();
                        if (acceptEncoding.Contains("GZIP"))
                        {
                            response.Filter = new CompressWhitespaceFilter(context.Response.Filter, CompressOptions.GZip);
                            response.AppendHeader("Content-encoding", "gzip");
                        }
                        else if (acceptEncoding.Contains("DEFLATE"))
                        {
                            response.Filter = new CompressWhitespaceFilter(context.Response.Filter, CompressOptions.Deflate);
                            response.AppendHeader("Content-encoding", "deflate");
                        }
                    }
                    response.Cache.VaryByHeaders["Accept-Encoding"] = true;
                }
            }
        }

        bool FileTypeNeedToCompress(string url)
        {
            string[] extensions = { ".cshtml", ".vbhtml", ".html", ".aspx" };
            return extensions.Any(x => url.EndsWith(x));
        }

        #endregion
    }
}
