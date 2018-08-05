using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Annotations;
using Caliburn.Micro;
using Primusz.SoilGenius.Core.Model;
using Primusz.SoilGenius.Core.Numerics;
using Primusz.SoilGenius.Wpf.Messages;

namespace Primusz.SoilGenius.Wpf.ViewModels
{
    public class CbrTestPlotViewModel : PropertyChangedBase, IHandle<string>
    {
        #region Members

        private CbrTestDataViewModel selectedTest;
        private readonly Spline spline = new Spline { Nodes = 25, Rho = 2 };

        #endregion

        #region Properties

        public PlotModel PlotModel { get; set; }

        private LineSeries LineSeries { get; set; }

        private ScatterSeries ScatterSeries { get; set; }

        private LineAnnotation LineAnnotation { get; set; }

        private TextAnnotation TextAnnotation1 { get; set; }

        private TextAnnotation TextAnnotation2 { get; set; }

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

        #region Constructors

        public CbrTestPlotViewModel(IEventAggregator eventAggregator, IList<CbrTestData> dataset = null)
        {
            eventAggregator.Subscribe(this);

            Tests = new ObservableCollection<CbrTestDataViewModel>();

            if (dataset != null)
            {
                foreach (var item in dataset)
                {
                    Tests.Add(new CbrTestDataViewModel(item, eventAggregator));
                }
            }

            SetupPlotModel();
        }

        #endregion

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

            LineSeries = new LineSeries { Smooth = false, StrokeThickness = 3.0 };

            TextAnnotation1 = new TextAnnotation
            {
                FontSize = 12d,
                FontWeight = FontWeights.Bold,
                TextColor = OxyColors.Black,
                Background = OxyColors.Silver,
                Offset = new ScreenVector(0, -50)
            };

            TextAnnotation2 = new TextAnnotation
            {
                FontSize = 12d,
                FontWeight = FontWeights.Bold,
                TextColor = OxyColors.Black,
                Background = OxyColors.Silver,
                Offset = new ScreenVector(0, -50)
            };

            LineAnnotation = new LineAnnotation
            {
                Type = LineAnnotationType.LinearEquation,
                StrokeThickness = 3.0,
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

                SelectedTest.Slope = LineAnnotation.Slope;
                SelectedTest.Intercept = currPoint.Y - currPoint.X * LineAnnotation.Slope;

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

        private void SetAnnotationVisibility(Annotation annotation, bool isVisible = false)
        {
            if (isVisible)
            {
                if (PlotModel.Annotations.Contains(annotation) == false)
                {
                    PlotModel.Annotations.Add(annotation);
                }
            }
            else
            {
                PlotModel.Annotations.Remove(annotation);
            }
        }

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

        private void RenderSpline(bool invalidatePlot = false)
        {
            double maximum = 0;

            double[] x = new double[SelectedTest.TestDataPoints.Count];
            double[] y = new double[SelectedTest.TestDataPoints.Count];

            for (var i = 0; i < SelectedTest.TestDataPoints.Count; i++)
            {
                x[i] = SelectedTest.TestDataPoints[i].Stroke;
                y[i] = SelectedTest.TestDataPoints[i].Force;

                if (x[i] >= maximum) maximum = x[i];
            }

            spline.X = x;
            spline.Y = y;
            spline.Rho = SelectedTest.SplineRho;
            spline.Nodes = (int)SelectedTest.SplineNodes;

            if (SelectedTest.TestDataPoints.Count > 0)
            {
                if (spline.Fit())
                {
                    LineSeries.Points.Clear();
                    LineSeries.Color = OxyColors.Green;

                    for (double i = 0; i <= maximum; i += 0.05d)
                    {
                        LineSeries.Points.Add(new DataPoint(i, spline.Calculation(i)));
                    }

                    SelectedTest.Force1 = Math.Round(spline.Calculation(SelectedTest.ZeroPoint + 2.5), 2);
                    SelectedTest.Force2 = Math.Round(spline.Calculation(SelectedTest.ZeroPoint + 5.0), 2);

                    if (invalidatePlot)
                    {
                        PlotModel.InvalidatePlot(true);
                    }
                }
            }
        }

        private void RenderDataPoints(bool invalidatePlot = false)
        {
            ScatterSeries.Points.Clear();

            foreach (var point in SelectedTest.TestDataPoints)
            {
                ScatterSeries.Points.Add(new ScatterPoint(point.Stroke, point.Force));
            }

            if (invalidatePlot)
            {
                PlotModel.InvalidatePlot(true);
            }
        }

        private void RenderTextAnnotation(bool invalidatePlot = false)
        {
            TextAnnotation1.TextPosition = new DataPoint(2.5 + SelectedTest.ZeroPoint, SelectedTest.Force1);
            TextAnnotation1.Text = $"{selectedTest.Force1:0.00} kN";

            TextAnnotation2.TextPosition = new DataPoint(5.0 + SelectedTest.ZeroPoint, SelectedTest.Force2);
            TextAnnotation2.Text = $"{selectedTest.Force2:0.00} kN";

            SetAnnotationVisibility(TextAnnotation1, true);
            SetAnnotationVisibility(TextAnnotation2, true);

            if (invalidatePlot)
            {
                PlotModel.InvalidatePlot(true);
            }
        }

        private void RenderLineAnnotation(bool invalidatePlot = false)
        {
            LineAnnotation.Slope = SelectedTest.Slope;
            LineAnnotation.Intercept = SelectedTest.Intercept;

            SetAnnotationVisibility(LineAnnotation, SelectedTest.AdjustZeroPoint);

            if (invalidatePlot)
            {
                PlotModel.InvalidatePlot(true);
            }
        }

        #region Overridden Methods

        public override void NotifyOfPropertyChange(string propertyName = null)
        {
            base.NotifyOfPropertyChange(propertyName);
            {
                switch (propertyName)
                {
                    case "SelectedTest":
                        {
                            RenderSpline(true);
                            RenderDataPoints(true);
                            RenderTextAnnotation(true);
                            RenderLineAnnotation(true);
                        }
                        break;
                }
            }
        }

        #endregion

        #region From IHandle Interface

        public void Handle(string message)
        {
            if (SelectedTest != null && PlotMessages.InvalidatePlot == message)
            {
                RenderSpline();
                RenderDataPoints();
                RenderTextAnnotation();
                RenderLineAnnotation(true);
            }
        }

        #endregion
    }
}