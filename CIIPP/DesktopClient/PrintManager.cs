using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Printing;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Windows;

namespace DesktopClient
{
    public struct PrintPage
    {
        public double ExtentWidth { get; private set; }
        public double ExtentHeight { get; private set; }
        public double MediaWidth { get; private set; }
        public double MediaHeight { get; private set; }
        public double Bleed { get; private set; }

        private static PrintPage _a4_portrait;
        private static PrintPage _a4_landscape;
        private static PrintPage _a3_portrait;
        private static PrintPage _a3_landscape;

        static PrintPage()
        {
            _a3_portrait = new PrintPage { ExtentWidth = 96 * 11.7, ExtentHeight = 96 * 15.0, MediaWidth = 96 * 11.7, MediaHeight = 96 * 16.5, Bleed = 96 * 0.75 };
            _a3_landscape = new PrintPage { ExtentWidth = 96 * 15.0, ExtentHeight = 96 * 10.2, MediaWidth = 96 * 16.5, MediaHeight = 96 * 11.7, Bleed = 96 * 0.75 };
            _a4_portrait = new PrintPage { ExtentWidth = 96 * 8.27, ExtentHeight = 96 * 10.7, MediaWidth = 96 * 8.27, MediaHeight = 96 * 11.7, Bleed = 96 * 0.5 };
            _a4_landscape = new PrintPage { ExtentWidth = 96 * 10.7, ExtentHeight = 96 * 7.27, MediaWidth = 96 * 11.7, MediaHeight = 96 * 8.27, Bleed = 96 * 0.5 };
        }

        public static PrintPage A4_Portrait { get { return _a4_portrait; } }
        public static PrintPage A4_Landscape { get { return _a4_landscape; } }
        public static PrintPage A3_Portrait { get { return _a3_portrait; } }
        public static PrintPage A3_Landscape { get { return _a3_landscape; } }
    }

    public static class PrintManager
    {
        public static PrintPage Page { get; set; }

        static PrintManager()
        {
            Page = PrintPage.A3_Portrait;
        }

        public static FlowDocument GetDocumentFrom(IEnumerable<UIElement> elements)
        {
            //double dpi = 150;
            //double mag = dpi / 96;
            //var images = elements.Select(element =>
            //{
            //    RenderTargetBitmap bmp = new RenderTargetBitmap(Convert.ToInt32(mag * element.ActualWidth), Convert.ToInt32(mag * element.ActualHeight), dpi, dpi, PixelFormats.Pbgra32);
            //    bmp.Render(element);
            //    return bmp;
            //});

            FlowDocument doc = new FlowDocument();
            doc.ColumnWidth = 96 * 16;
            elements.ToList().ForEach(x =>
            {
                doc.Blocks.Add(new BlockUIContainer(x));
            });
            return doc;
        }

        public static FixedDocument FlowToFixed(FlowDocument flowDoc)
        {
            DocumentPaginator paginator = (flowDoc as IDocumentPaginatorSource).DocumentPaginator;
            paginator.PageSize = new Size(Page.ExtentWidth, Page.ExtentHeight);     // A3
            if (!paginator.IsPageCountValid)
            {
                paginator.ComputePageCount();
            }

            FixedDocument fixedDoc = new FixedDocument();
            for (int i = 0; i < paginator.PageCount; i++)
            {
                Canvas pageCanvas = new Canvas();
                pageCanvas.Margin = new Thickness(Page.Bleed);
                DocumentPageView dpv = new DocumentPageView { DocumentPaginator = paginator, PageNumber = i };
                pageCanvas.Children.Add(dpv);

                FixedPage fp = new FixedPage { Width = Page.MediaWidth, Height = Page.MediaHeight };
                fp.Children.Add(pageCanvas);
                PageContent pc = new PageContent();
                (pc as IAddChild).AddChild(fp);
                fixedDoc.Pages.Add(pc);
            }
            return fixedDoc;
        }

        public static void PrintFlowDoc(FlowDocument doc, string fileName)
        {
            DocumentPaginator paginator = (doc as IDocumentPaginatorSource).DocumentPaginator;
            //paginator.PageSize = new Size(96 * 8.27, 96 * 11.7);   // A4
            paginator.PageSize = new Size(96 * 11.7, 96 * 16.5);     // A3
            using (XpsDocument xps = new XpsDocument(fileName, System.IO.FileAccess.Write))
            {
                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xps);
                writer.Write(paginator);
            }
        }

        public static void PrintFixedDoc(FixedDocument doc, string fileName)
        {
            using (XpsDocument xps = new XpsDocument(fileName, System.IO.FileAccess.Write))
            {
                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xps);
                writer.Write(doc);
            }
        }
    }
}
