using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Teller.Charts.Templates;
using Teller.Charts.Viewmodels;
using Teller.Core.Entities;
using Teller.Core.Infrastructure;

namespace Teller.Charts
{
    public class RenderTest
    {
        private SingleMatchViewmodel _vm;

        public void Render(BillettServiceEvent bsEvent)
        {
            var ctrl = new SingleMatch {DataContext = _vm = new SingleMatchViewmodel(bsEvent)};

            var path = Path.Combine(StautConfiguration.Current.StaticImageDirectory, bsEvent.EventNumber + ".png");

            RenderAndSave(ctrl, path, 460, 650);
        }

        void RenderAndSave(UIElement target, string filename, int width, int height)
        {
            var mainContainer = new Canvas()
            {
                Width = width,
                Height = height
            };
            mainContainer.Children.Add(target);

            target.Measure(new Size(width, height));
            target.Arrange(new Rect(0, 0, width, height));
            target.UpdateLayout();

            RenderOptions.SetClearTypeHint(target, ClearTypeHint.Enabled);
            RenderOptions.SetBitmapScalingMode(target, BitmapScalingMode.Fant);

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
