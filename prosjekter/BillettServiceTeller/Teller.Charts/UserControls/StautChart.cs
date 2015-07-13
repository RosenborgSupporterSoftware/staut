using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Teller.Charts.UserControls
{
    public class StautChart : FrameworkElement
    {
        public static DependencyProperty DataProperty;

        public ObservableCollection<StautSeries> Data
        {
            get { return (ObservableCollection<StautSeries>) GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        static StautChart()
        {
            var metadata = new FrameworkPropertyMetadata {AffectsRender = true};
            DataProperty = DependencyProperty.Register("Data", typeof (ObservableCollection<StautSeries>),
                typeof (StautChart), metadata);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var bounds = new Rect(0, 0, ActualWidth, ActualHeight);
            drawingContext.DrawRoundedRectangle(Brushes.CadetBlue, new Pen(Brushes.DarkSeaGreen, 3.5), bounds, 10, 10);
            drawingContext.DrawLine(new Pen(Brushes.OrangeRed, 5), new Point(0, 0),
                new Point(ActualWidth, ActualHeight));
        }
    }
}
