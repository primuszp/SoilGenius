using System;
using System.IO;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Annotations;
//using netDxf;
//using netDxf.Entities;
using Caliburn.Micro;
using Primusz.SoilGenius.Core.Model;
using Primusz.SoilGenius.Core.Numerics;

namespace Primusz.SoilGenius.Wpf.ViewModels
{
    public class CbrTestSampleViewModel : PropertyChangedBase
    {
        #region Members

        private LineSeries spline;
        private LineAnnotation la;
        private CbrTestData model;

        private int nodes = 50;
        private double rho = 2;
        private bool adjustZeroPoint;

        #endregion

        #region Properties

        public double Slope
        {
            get { return la.Slope; }
            set
            {
                la.Slope = value;
                PlotModel.InvalidatePlot(false);
                NotifyOfPropertyChange(() => Slope);
                NotifyOfPropertyChange(() => ZeroPoint);
            }
        }

        public double Intercept
        {
            get { return la.Intercept; }
            set
            {
                la.Intercept = value;
                PlotModel.InvalidatePlot(false);
                NotifyOfPropertyChange(() => Intercept);
                NotifyOfPropertyChange(() => ZeroPoint);
            }
        }

        public double ZeroPoint
        {
            get
            {
                if (AdjustZeroPoint)
                {
                    return -Intercept / Slope;
                }
                return 0.0d;
            }
        }

        public double Rho
        {
            get { return rho; }
            set
            {
                if (Math.Abs(value - rho) > double.Epsilon)
                {
                    rho = value;
                    SplineCurve();
                    NotifyOfPropertyChange(() => Rho);
                }
            }
        }

        public int Nodes
        {
            get { return nodes; }
            set
            {
                if (value != nodes)
                {
                    nodes = value;
                    SplineCurve();
                    NotifyOfPropertyChange(() => Nodes);
                }
            }
        }

        public bool AdjustZeroPoint
        {
            get { return adjustZeroPoint; }
            set
            {
                if (value != adjustZeroPoint)
                {
                    adjustZeroPoint = value;

                    if (adjustZeroPoint)
                        PlotModel.Annotations.Add(la);
                    else
                        PlotModel.Annotations.Remove(la);

                    PlotModel.InvalidatePlot(false);
                    NotifyOfPropertyChange(() => ZeroPoint);
                    NotifyOfPropertyChange(() => AdjustZeroPoint);
                }
            }
        }

        public PlotModel PlotModel { get; private set; }

        #endregion

        public CbrTestSampleViewModel(CbrTestData sample)
        {
            SetupPlotModel(sample);
        }

        private void SetupPlotModel(CbrTestData sample)
        {
            model = sample;

            OxyColor color = OxyColor.FromArgb(100, 255, 255, 255);

            PlotModel = new PlotModel
            {
                Title = "CBR%",
                TextColor = OxyColors.White,
                TitleFontSize = 14,
                PlotAreaBackground = color,
                Background = OxyColor.FromArgb(50, 255, 255, 255)
            };

            LinearAxis left = new LinearAxis
            {
                Title = "Erő [kN]",
                TicklineColor = color,
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                TickStyle = TickStyle.Outside
            };

            LinearAxis bottom = new LinearAxis
            {
                Title = "Elmozdulás [mm]",
                TicklineColor = color,
                Position = AxisPosition.Bottom,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                TickStyle = TickStyle.Outside
            };

            ScatterSeries series = new ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerStrokeThickness = 1,
                MarkerFill = OxyColors.Red,
                MarkerStroke = OxyColors.Black,
                MarkerSize = 4
            };

            foreach (CbrTestPoint point in model.TestPoints)
            {
                series.Points.Add(new ScatterPoint(point.Penetration, point.Force));
            }

            SplineCurve();

            PlotModel.Axes.Add(left);
            PlotModel.Axes.Add(bottom);
            PlotModel.Series.Add(series);
            PlotModel.Series.Add(spline);

            la = new LineAnnotation
            {
                Type = LineAnnotationType.LinearEquation,
                StrokeThickness = 2,
                Slope = 2.0,
                Intercept = 2.0
            };

            la.MouseDown += (s, e) =>
            {
                if (e.ChangedButton == OxyMouseButton.Left)
                {
                    la.StrokeThickness *= 2;
                    PlotModel.InvalidatePlot(false);
                    e.Handled = true;
                }
            };

            la.MouseMove += (s, e) =>
            {
                var currPoint = la.InverseTransform(e.Position);
                Intercept = currPoint.Y - currPoint.X * la.Slope;

                PlotModel.InvalidatePlot(false);
                e.Handled = true;
            };

            la.MouseUp += (s, e) =>
            {
                la.StrokeThickness /= 2;
                PlotModel.InvalidatePlot(false);
                e.Handled = true;
            };
        }

        public void Save(string fileName)
        {
            //LineSeries line = SplineCurve(model.TestPoints, 0.1);
            //ExportToDxf(line, fileName);
        }


        private void SplineCurve(double step = 0.0025d)
        {
            if (spline == null)
                spline = new LineSeries { Smooth = false };

            double maximum = 0;

            double[] x = new double[model.TestPoints.Count];
            double[] y = new double[model.TestPoints.Count];

            for (var i = 0; i < model.TestPoints.Count; i++)
            {
                x[i] = model.TestPoints[i].Penetration;
                y[i] = model.TestPoints[i].Force;

                if (x[i] >= maximum) maximum = x[i];
            }

            var curve = new Spline { X = x, Y = y, Rho = Rho, Nodes = Nodes };

            if (model.TestPoints.Count > 0)
            {
                if (curve.Fit())
                {
                    spline.Points.Clear();
                    spline.Color = OxyColors.Green;

                    for (double i = 0; i <= maximum; i += step)
                    {
                        spline.Points.Add(new DataPoint(i, curve.Calculation(i)));
                    }

                    PlotModel.InvalidatePlot(true);
                }
            }
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