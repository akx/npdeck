using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NpDeck
{
	internal static class ImageRenderer
	{
		public static void Render(string formattedResult, Config config)
		{
			var rtb = new RenderTargetBitmap(config.ImageWidth, config.ImageHeight, 96, 96, PixelFormats.Pbgra32);
			var dw = new DrawingVisual();
			var dc = dw.RenderOpen();
			var tf = new Typeface(config.ImageFontName);
			var br = new SolidColorBrush((Color) ColorConverter.ConvertFromString(config.ImageTextColor));
			var ft = new FormattedText(formattedResult, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, tf, config.ImageFontSize, br);
			dc.DrawText(ft, new Point(0, 0));
			dc.Close();
			rtb.Render(dw);
			BitmapEncoder encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(rtb));
			using (var fileStream = new FileStream(config.ImageFilename, FileMode.Create))
			{
				encoder.Save(fileStream);
			}
		}
	}
}