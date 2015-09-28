using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Teller.Charts.UserControls
{
    /// <summary>
    /// En custom dings som tegner grafer via Microsoft sin Chart control for web (eller WinForms om nødvendig, men web foreløpig)
    /// </summary>
    public class StautChart : FrameworkElement
    {
        private readonly ChartMaterializer _materializer = new ChartMaterializer();

        public static DependencyProperty DataProperty;

        public ObservableCollection<StautSeries> Data
        {
            get { return (ObservableCollection<StautSeries>) GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public FontFamily FontFamily
        {
            get { return (FontFamily) GetValue(TextElement.FontFamilyProperty); }
            set { SetValue(TextElement.FontFamilyProperty, value); }
        }

        public double FontSize
        {
            get { return (double) GetValue(TextElement.FontSizeProperty); }
            set { SetValue(TextElement.FontSizeProperty, value); }
        }

        static StautChart()
        {
            var metadata = new FrameworkPropertyMetadata {AffectsRender = true};
            DataProperty = DependencyProperty.Register("Data", typeof (ObservableCollection<StautSeries>),
                typeof (StautChart), metadata);

            metadata = new FrameworkPropertyMetadata {Inherits = true, AffectsMeasure = true, AffectsArrange = true, AffectsRender = true};
            TextElement.FontFamilyProperty.AddOwner(typeof (StautChart), metadata);
            metadata = new FrameworkPropertyMetadata { Inherits = true, AffectsMeasure = true, AffectsArrange = true, AffectsRender = true };
            TextElement.FontSizeProperty.AddOwner(typeof (StautChart), metadata);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            _materializer.FontFamily = FontFamily.Source;
            _materializer.FontSize = FontSize;

            var chartImage = _materializer.RenderChart(Data, ActualWidth, ActualHeight);

            drawingContext.PushOpacity(1.0);
            drawingContext.DrawImage(chartImage, new Rect(new Size(ActualWidth, ActualHeight)));
        }

    }
}
