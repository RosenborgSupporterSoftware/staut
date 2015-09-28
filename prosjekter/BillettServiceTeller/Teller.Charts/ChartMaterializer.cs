using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using System.Windows.Media.Imaging;

namespace Teller.Charts
{
    /// <summary>
    /// En klasse som lager selve Chart-objektet og tilpasser det STAut-behov
    /// </summary>
    public class ChartMaterializer
    {

        #region Fields

        private readonly Color _gridlineColor = Color.FromArgb(50, Color.Black);
        private Font _font;

        #endregion

        public string FontFamily { get; set; }
        public double FontSize { get; set; }

        public BitmapImage RenderChart(IEnumerable<StautSeries> data, double width, double height)
        {
            if (data == null)
                return null;

            var chart = CreateChart(width, height);

            _font = new Font(FontFamily, (float)FontSize, FontStyle.Regular);

            var singleSeries = data.FirstOrDefault();

            var seriesName = singleSeries.Title;
            var series = new Series(seriesName);
            series.ChartType = SeriesChartType.SplineArea;
            
            chart.Series.Add(series);
            var measurements = singleSeries.Points.ToList();
            var xValues = measurements.Select(m => m.XValue).ToList();
            var yValues = measurements.Select(m => m.YValue).ToList();
            series.Points.DataBindXY(xValues, yValues);
            series.Color = Color.FromArgb(100, Color.Gray);
            series.BackGradientStyle = GradientStyle.None;
            //series.BackSecondaryColor = Color.FromArgb(100, Color.Red);
            series.LabelFormat = "";
            series.SmartLabelStyle.Enabled = true;
            series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
            series.CustomProperties = "DrawingStyle = Cylinder, PixelPointWidth = 3";

            var chartArea = new ChartArea();
            var xAxis = CreateXAxis(chartArea);
            var yAxis = CreateYAxis(chartArea);

            yAxis.Minimum = CalculateMinimumValue(measurements);

            chartArea.AxisX = xAxis;
            chartArea.AxisY = yAxis;

            chart.ChartAreas.Add(chartArea);

            return CreateBitmapImage(chart);
        }

        private int CalculateMinimumValue(IList<StautPoint> measurements)
        {
            var minValue = measurements.OrderBy(m => m.YValue).FirstOrDefault();
            if (minValue == null)
                return 0;

            var k = (int) (minValue.YValue/1000);

            return k*1000;
        }

        private static BitmapImage CreateBitmapImage(Chart chart)
        {
            var imageStream = new MemoryStream();
            chart.SaveImage(imageStream);

            var res = new BitmapImage();
            res.BeginInit();
            res.StreamSource = imageStream;
            res.EndInit();

            return res;
        }

        private Axis CreateYAxis(ChartArea chartArea)
        {
            var yAxis = new Axis(chartArea, AxisName.Y)
            {
                //Minimum = 8000,   // TODO: Regnes ut til nærmeste hele tusen under faktisk minimum
                Maximum = 22000,
                LabelStyle = { Interval = 2000 },
                MajorGrid = { Interval = 2000 },
                IntervalOffsetType = DateTimeIntervalType.Number,
                IsMarginVisible = false
            };
            yAxis.LabelAutoFitStyle = LabelAutoFitStyles.None;
            yAxis.LabelStyle.Font = _font;
            yAxis.MajorTickMark.Enabled = false;
            yAxis.LabelStyle.Format = "#,k";
            yAxis.IsStartedFromZero = false;
            yAxis.MajorGrid.LineColor = _gridlineColor;
            return yAxis;
        }

        private Axis CreateXAxis(ChartArea chartArea)
        {
            var xAxis = new Axis(chartArea, AxisName.X)
            {
                IsReversed = true,
                Minimum = 0,
                Maximum = 30,
                IntervalOffsetType = DateTimeIntervalType.Days,
                //Title = "Dager før kamp",
                IsMarginVisible = false,
                MajorGrid = {LineColor = _gridlineColor, Interval = 5},
                LabelStyle = { Interval = 5 }
            };
            xAxis.MajorTickMark.Enabled = false;
            xAxis.LabelAutoFitStyle = LabelAutoFitStyles.None;
            xAxis.LabelStyle.Font = _font;
            return xAxis;
        }

        private Chart CreateChart(double width, double height)
        {
            var chart = new Chart
            {
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High,
                Width = new Unit(width, UnitType.Pixel),
                Height = new Unit(height, UnitType.Pixel),
                BackImageTransparentColor = Color.White
            };

            return chart;
        }
    }
}
