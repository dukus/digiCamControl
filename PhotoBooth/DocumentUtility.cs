using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoBooth
{
    public static class DocumentUtility
    {
        public static FixedDocument CreateFixedDocument(double pageWidth, double pageHeight, UIElement content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            if (pageWidth <= 0 || pageHeight <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            RenderTargetBitmap renderTarget = new RenderTargetBitmap(Convert.ToInt32(content.RenderSize.Width), Convert.ToInt32(content.RenderSize.Height),
                Constants.ScreenDPI, Constants.ScreenDPI, PixelFormats.Pbgra32);
            renderTarget.Render(content);

            FixedDocument doc = new FixedDocument();
            Size pageSize = new Size(pageWidth, pageHeight);
            doc.DocumentPaginator.PageSize = pageSize;
            FixedPage fixedPage = new FixedPage();
            fixedPage.Width = pageWidth;
            fixedPage.Height = pageHeight;
            Image image = new Image();
            image.Height = pageHeight;
            image.Width = pageWidth;
            image.Stretch = Stretch.Uniform;
            image.StretchDirection = StretchDirection.Both;
            image.Source = renderTarget;
            fixedPage.Children.Add(image);

            PageContent pageContent = new PageContent();
            ((IAddChild)pageContent).AddChild(fixedPage);
            doc.Pages.Add(pageContent);
            return doc;
        }
    }
}
