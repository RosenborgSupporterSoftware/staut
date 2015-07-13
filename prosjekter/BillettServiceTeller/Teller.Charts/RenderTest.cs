using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Teller.Charts.Templates;
using Teller.Charts.Viewmodels;

namespace Teller.Charts
{
    public class RenderTest
    {
        private SingleMatchViewmodel _vm;

        public void Render()
        {
            var ctrl = new SingleMatch {DataContext = _vm = new SingleMatchViewmodel()};

            RenderAndSave(ctrl, @"d:\temp\staut\wpf-test.png", 460, 600);
        }

        void RenderAndSave(UIElement target, string filename, int width, int height)
        {
            var mainContainer = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            mainContainer.Children.Add(target);

            target.Measure(new Size(width, height));
            target.Arrange(new Rect(0, 0, width, height));
            target.UpdateLayout();

            var encoder = new PngBitmapEncoder();
            var render = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);

            render.Render(target);
            encoder.Frames.Add(BitmapFrame.Create(render));
            using (var s = File.Open(filename, FileMode.Create))
            {
                encoder.Save(s);
            }
        }
    }
}
