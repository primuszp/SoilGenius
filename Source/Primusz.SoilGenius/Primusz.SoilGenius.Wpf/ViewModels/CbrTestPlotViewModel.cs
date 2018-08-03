using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Annotations;
using Caliburn.Micro;
using Primusz.SoilGenius.Core.Model;
using Primusz.SoilGenius.Wpf.Abstractions;

namespace Primusz.SoilGenius.Wpf.ViewModels
{
    public class CbrTestPlotViewModel : PropertyChangedBase, ICbrTestPlot
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
            get => selectedTest;
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

        public CbrTestPlotViewModel(IList<CbrTestData> dataset = null)
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
                switch (propertyName)
                {
                    case "SelectedTest":
                        SelectedTest.RenderSpline();
                        SelectedTest.RenderDataPoints();
                        SelectedTest.RenderLineAnnotation();
                        break;
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
                Background = OxyColor.FromArgb(50, 255, 255, 255),
                LegendTitle = "Legend",
                LegendPosition = LegendPosition.RightBottom
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

            LineAnnotation.MouseDown += (s, e) =>
            {
                if (e.ChangedButton == OxyMouseButton.Left)
                {
                    LineAnnotation.StrokeThickness *= 1.2;
                    PlotModel.InvalidatePlot(false);
                    e.Handled = true;
                }
            };

            LineAnnotation.MouseMove += (s, e) =>
            {
                var currPoint = LineAnnotation.InverseTransform(e.Position);
                LineAnnotation.Intercept = currPoint.Y - currPoint.X * LineAnnotation.Slope;

                SelectedTest.Slope = LineAnnotation.Slope;
                SelectedTest.Intercept = LineAnnotation.Intercept;

                PlotModel.InvalidatePlot(false);
                e.Handled = true;
            };

            LineAnnotation.MouseUp += (s, e) =>
            {
                LineAnnotation.StrokeThickness /= 1.2;
                PlotModel.InvalidatePlot(false);
                e.Handled = true;
            };

            PlotModel.Axes.Add(axisX);
            PlotModel.Axes.Add(axisY);
            PlotModel.Series.Add(LineSeries);
            PlotModel.Series.Add(ScatterSeries);
        }

        public void InvalidatePlot(bool updateData = false)
        {
            PlotModel.InvalidatePlot(updateData);
        }

        public void SetLineVisibility(bool isVisible = false)
        {
            if (isVisible)
            {
                if (PlotModel.Annotations.Contains(LineAnnotation) == false)
                {
                    PlotModel.Annotations.Add(LineAnnotation);
                }
            }
            else
            {
                PlotModel.Annotations.Remove(LineAnnotation);
            }
        }

        public void SetCbrValue(double cbr25, double cbr50)
        {
            var cbr25Text = new TextAnnotation
            {
                Text = $"CBR25 = {cbr25}",
                Background = OxyColor.FromArgb(50, 255, 255, 255),
                TextPosition = new DataPoint(2.5, cbr25)
            };

            var cbr50Text = new TextAnnotation
            {
                Text = $"CBR50 = {cbr50}",
                Background = OxyColor.FromArgb(50, 255, 255, 255),
                TextPosition = new DataPoint(5.0, cbr50)
            };

            PlotModel.Annotations.Add(cbr25Text);
            PlotModel.Annotations.Add(cbr50Text);
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