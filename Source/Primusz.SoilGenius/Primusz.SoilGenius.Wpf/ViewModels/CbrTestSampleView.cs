using System.IO;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Annotations;
using Caliburn.Micro;
using netDxf;
using netDxf.Entities;
using Primusz.SoilGenius.Core.Model;

namespace Primusz.SoilGenius.Wpf.ViewModels
{
    public class CbrTestSampleViewModel : PropertyChangedBase
    {
        #region Members

        private CbrTestSample model;
        private DataPoint prevPoint;
        private DataPoint currPoint;

        #endregion

        #region Properties

        public PlotModel CbrPlotModel { get; private set; }

        #endregion

        public CbrTestSampleViewModel(CbrTestSample sample)
        {
            SetupPlotModel(sample);
        }

        private void SetupPlotModel(CbrTestSample sample)
        {
            model = sample;

            OxyColor color = OxyColor.FromArgb(100, 255, 255, 255);

            CbrPlotModel = new PlotModel
            {
                Title = "CBR%",
                TextColor = OxyColors.White,
                TitleFontSize = 14,
                PlotAreaBackground = color,
                Background = OxyColor.FromArgb(50, 255, 255, 255)
            };

            LinearAxis left = new LinearAxis()
            {
                Title = "Erők (kN)",
                TicklineColor = color,
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                TickStyle = TickStyle.Outside
            };

            LinearAxis bottom = new LinearAxis()
            {
                Title = "Elmozdulás (mm)",
                TicklineColor = color,
                Position = AxisPosition.Bottom,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                TickStyle = TickStyle.Outside
            };

            ScatterSeries series = new ScatterSeries()
            {
                MarkerType = MarkerType.Circle,
                MarkerStrokeThickness = 0,
                MarkerFill = OxyColors.Red,
                MarkerSize = 2
            };

            foreach (TestPoint point in model.TestPoints)
            {
                series.Points.Add(new ScatterPoint(point.Stroke, point.Force));
            }

            LineSeries line = SplineFitting(model.TestPoints, 0.1);
            line.Color = OxyColors.Green;

            CbrPlotModel.Axes.Add(left);
            CbrPlotModel.Axes.Add(bottom);
            CbrPlotModel.Series.Add(series);
            CbrPlotModel.Series.Add(line);


            var la = new LineAnnotation { Type = LineAnnotationType.LinearEquation, Slope = 2, Intercept = 2};

            la.MouseDown += (s, e) =>
            {
                if (e.ChangedButton == OxyMouseButton.Left)
                {
                    prevPoint = la.InverseTransform(e.Position);

                    la.StrokeThickness *= 5;
                    CbrPlotModel.InvalidatePlot(false);
                    e.Handled = true;
                }
            };

            // Handle mouse movements (note: this is only called when the mousedown event was handled)
            la.MouseMove += (s, e) =>
            {
                currPoint = la.InverseTransform(e.Position);
                la.Intercept = currPoint.Y - currPoint.X * la.Slope;

                CbrPlotModel.InvalidatePlot(false);
                e.Handled = true;
            };

            la.MouseUp += (s, e) =>
            {
                la.StrokeThickness /= 5;
                CbrPlotModel.InvalidatePlot(false);
                e.Handled = true;
            };

            CbrPlotModel.Annotations.Add(la);
        }

        public void Save(string fileName)
        {
            LineSeries line = SplineFitting(model.TestPoints, 0.1);
            ExportToDxf(line, fileName);
        }

        private LineSeries SplineFitting(IList<TestPoint> points, double step)
        {
            LineSeries series = new LineSeries();

            double[] x = new double[points.Count];
            double[] y = new double[points.Count];

            double maximum = 0;

            for (int i = 0; i < points.Count; i++)
            {
                x[i] = points[i].Stroke;
                y[i] = points[i].Force;

                if (x[i] > maximum) maximum = x[i];
            }

            Math.Spline spline = new Math.Spline { X = x, Y = y, Rho = 1, Nodes = 300 };

            if (points.Count > 0)

                if (spline.DataFitting())
                {
                    for (double i = 0; i <= maximum; i += step)
                    {
                        series.Points.Add(new DataPoint(i, spline.Calculation(i)));
                    }
                }

            return series;
        }

        private void ExportToDxf(LineSeries series, string fileName)
        {
            DxfDocument dxf = new DxfDocument();
            LwPolyline polyline = new LwPolyline();

            foreach (var p in series.Points)
            {
                polyline.Vertexes.Add(new LwPolylineVertex(10*p.X, 10*p.Y));
            }

            dxf.AddEntity(polyline);
            dxf.Save(fileName);
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
    }
}