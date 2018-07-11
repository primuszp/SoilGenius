using System;
using Caliburn.Micro;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Annotations;
using Primusz.SoilGenius.Core.Model;
using Primusz.SoilGenius.Core.Numerics;
using Primusz.SoilGenius.Wpf.Abstractions;

namespace Primusz.SoilGenius.Wpf.ViewModels
{
    public class CbrTestDataViewModel : PropertyChangedBase
    {
        #region Members

        private int nodes = 50;
        private double rho = 2;
        private double slope = 2, intercept = 2;
        private bool adjustZeroPoint;
        private readonly ITestPlot plot;
        private readonly TestData data;
        private readonly LineAnnotation line;

        #endregion

        #region Properties

        public double Slope
        {
            get { return slope; }
            set
            {
                slope = value;
                line.Slope = slope;
                plot.PlotModel.InvalidatePlot(false);
                NotifyOfPropertyChange(() => Slope);
                NotifyOfPropertyChange(() => ZeroPoint);
            }
        }

        public double Intercept
        {
            get { return intercept; }
            set
            {
                intercept = value;
                line.Intercept = intercept;
                plot.PlotModel.InvalidatePlot(false);
                NotifyOfPropertyChange(() => Intercept);
                NotifyOfPropertyChange(() => ZeroPoint);
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
                    RenderLineAnnotations();
                    NotifyOfPropertyChange(() => ZeroPoint);
                    NotifyOfPropertyChange(() => AdjustZeroPoint);
                }
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
                    RenderSpline();
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
                    RenderSpline();
                    NotifyOfPropertyChange(() => Nodes);
                }
            }
        }

        public string DisplayName { get; }

        #endregion

        #region Constructors

        public CbrTestDataViewModel(ITestPlot plot, TestData data)
        {
            this.data = data;
            this.plot = plot;
            this.line = plot.LineAnnotation;

            //DisplayName = this.data.File.Replace(".cbr", string.Empty);

            DisplayName = "Alma";

            line.MouseDown += (s, e) =>
            {
                if (e.ChangedButton == OxyMouseButton.Left)
                {
                    line.StrokeThickness *= 1.2;
                    plot.PlotModel.InvalidatePlot(false);
                    e.Handled = true;
                }
            };

            line.MouseMove += (s, e) =>
            {
                var currPoint = line.InverseTransform(e.Position);
                line.Intercept = currPoint.Y - currPoint.X * line.Slope;

                plot.PlotModel.InvalidatePlot(false);
                e.Handled = true;
            };

            line.MouseUp += (s, e) =>
            {
                line.StrokeThickness /= 1.2;
                plot.PlotModel.InvalidatePlot(false);
                e.Handled = true;
            };
        }

        #endregion

        public void RenderDataPoints()
        {
            plot.ScatterSeries.Points.Clear();

            foreach (TestPoint point in data.Points)
            {
                plot.ScatterSeries.Points.Add(new ScatterPoint(point.Stroke, point.Force));
            }

            plot.PlotModel.InvalidatePlot(true);
        }

        public void RenderSpline(double step = 0.0025d)
        {
            double maximum = 0;

            double[] x = new double[data.Points.Count];
            double[] y = new double[data.Points.Count];

            for (var i = 0; i < data.Points.Count; i++)
            {
                x[i] = data.Points[i].Stroke;
                y[i] = data.Points[i].Force;

                if (x[i] >= maximum) maximum = x[i];
            }

            var curve = new Spline { X = x, Y = y, Rho = rho, Nodes = nodes };

            if (data.Points.Count > 0)
            {
                if (curve.Fit())
                {
                    plot.LineSeries.Points.Clear();
                    plot.LineSeries.Color = OxyColors.Green;

                    for (double i = 0; i <= maximum; i += step)
                    {
                        plot.LineSeries.Points.Add(new DataPoint(i, curve.Calculation(i)));
                    }

                    plot.PlotModel.InvalidatePlot(true);
                }
            }
        }

        public void RenderLineAnnotations(bool updateData = false)
        {
            if (adjustZeroPoint)
                plot.PlotModel.Annotations.Add(line);
            else
                plot.PlotModel.Annotations.Remove(line);

            plot.PlotModel.InvalidatePlot(updateData);
        }
    }
}