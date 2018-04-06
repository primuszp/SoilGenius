using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Annotations;
using Caliburn.Micro;
using Primusz.SoilGenius.Core.Model;
using Primusz.SoilGenius.Wpf.Abstractions;

namespace Primusz.SoilGenius.Wpf.ViewModels
{
    public class CbrTestViewModel : PropertyChangedBase, ITestPlot
    {
        #region Members

        private CbrTestDataViewModel selectedTest;

        #endregion

        #region Properties

        public PlotModel PlotModel { get; private set; }

        public LineSeries LineSeries { get; private set; }

        public ScatterSeries ScatterSeries { get; private set; }

        public LineAnnotation LineAnnotation { get; private set; }

        public CbrTestDataViewModel SelectedTest
        {
            get { return selectedTest; }
            set
            {
                if (value != selectedTest)
                {
                    selectedTest = value;
                    NotifyOfPropertyChange(() => SelectedTest);
                }
            }
        }

        public ObservableCollection<CbrTestDataViewModel> Tests { get; set; }

        #endregion

        public CbrTestViewModel(IList<CbrTestData> dataset = null)
        {
            Tests = new ObservableCollection<CbrTestDataViewModel>();

            if (dataset != null)
            {
                foreach (var item in dataset)
                {
                    Tests.Add(new CbrTestDataViewModel(this, item));
                }
            }

            SetupPlotModel();
        }

        public override void NotifyOfPropertyChange(string propertyName = null)
        {
            base.NotifyOfPropertyChange(propertyName);
            {
                if (propertyName == "SelectedTest")
                {
                    SelectedTest.RenderSpline();
                    SelectedTest.RenderDataPoints();
                    SelectedTest.RenderLineAnnotations();
                }
            }
        }

        private void SetupPlotModel()
        {
            var color = OxyColor.FromArgb(100, 255, 255, 255);

            PlotModel = new PlotModel
            {
                Title = "CBR%",
                TextColor = OxyColors.White,
                TitleFontSize = 14,
                PlotAreaBackground = color,
                Background = OxyColor.FromArgb(50, 255, 255, 255)
            };

            var axisY = new LinearAxis
            {
                Title = "Erő [kN]",
                TicklineColor = color,
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                TickStyle = TickStyle.Outside
            };

            var axisX = new LinearAxis
            {
                Title = "Elmozdulás [mm]",
                TicklineColor = color,
                Position = AxisPosition.Bottom,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                TickStyle = TickStyle.Outside
            };

            ScatterSeries = new ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerStrokeThickness = 1,
                MarkerFill = OxyColors.Red,
                MarkerStroke = OxyColors.Black,
                MarkerSize = 4
            };

            LineSeries = new LineSeries { Smooth = false };


            LineAnnotation = new LineAnnotation
            {
                Type = LineAnnotationType.LinearEquation,
                StrokeThickness = 2.0,
                Slope = 2.0,
                Intercept = 2.0
            };

            PlotModel.Axes.Add(axisX);
            PlotModel.Axes.Add(axisY);
            PlotModel.Series.Add(LineSeries);
            PlotModel.Series.Add(ScatterSeries);
        }

        //private void ExportToDxf(LineSeries series, string fileName)
        //{
        //    DxfDocument dxf = new DxfDocument();
        //    LwPolyline polyline = new LwPolyline();

        //    foreach (var p in series.Points)
        //    {
        //        polyline.Vertexes.Add(new LwPolylineVertex(10 * p.X, 10 * p.Y));
        //    }

        //    dxf.AddEntity(polyline);
        //    dxf.Save(fileName);
        //}

        private void ExportSeries(LineSeries series, string fileName)
        {
            using (FileStream stream = File.Create(fileName))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    foreach (var p in series.Points)
                    {
                        writer.WriteLine(p.X.ToString("0.00") + "\t" + p.Y.ToString("0.0000"));
                    }
                }
            }
        }
    }
}