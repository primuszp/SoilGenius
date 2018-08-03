using System;
using OxyPlot;
using OxyPlot.Series;
using Caliburn.Micro;
using Primusz.SoilGenius.Core.Model;
using Primusz.SoilGenius.Core.Numerics;
using Primusz.SoilGenius.Wpf.Abstractions;

namespace Primusz.SoilGenius.Wpf.ViewModels
{
    public class CbrTestDataViewModel : PropertyChangedBase
    {
        #region Members

        private bool adjustZeroPoint;
        private double slope, intercept;

        private readonly Spline spline = new Spline { Nodes = 25, Rho = 2 };
        private readonly ICbrTestPlot plot;
        private readonly CbrTestData data;

        #endregion

        #region Properties

        public double Slope
        {
            get => slope;
            set
            {
                slope = value;
                plot.LineAnnotation.Slope = value;
                NotifyOfPropertyChange(() => Slope);
                NotifyOfPropertyChange(() => ZeroPoint);
            }
        }

        public double MaxSlope { get; private set; }

        public double MinSlope { get; private set; }

        public double Intercept
        {
            get => intercept;
            set
            {
                intercept = value;
                plot.LineAnnotation.Intercept = value;
                NotifyOfPropertyChange(() => Intercept);
                NotifyOfPropertyChange(() => ZeroPoint);
            }
        }

        public bool AdjustZeroPoint
        {
            get => adjustZeroPoint;
            set
            {
                if (value != adjustZeroPoint)
                {
                    adjustZeroPoint = value;
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

        public double SplineRho
        {
            get => spline.Rho;
            set
            {
                spline.Rho = value;
                NotifyOfPropertyChange(() => SplineRho);
            }
        }

        public int SplineNodes
        {
            get => spline.Nodes;
            set
            {
                spline.Nodes = value;
                NotifyOfPropertyChange(() => SplineNodes);
            }
        }

        public string DisplayName => data?.File.Replace(".cbr", string.Empty);

        #endregion

        #region Constructors

        public CbrTestDataViewModel(ICbrTestPlot plot, CbrTestData data)
        {
            this.data = data;
            this.plot = plot;

            FillSlopeRangeFromDataPoints();
        }

        #endregion

        public void RenderDataPoints()
        {
            plot.ScatterSeries.Points.Clear();

            foreach (var point in data.Points)
            {
                plot.ScatterSeries.Points.Add(new ScatterPoint(point.Stroke, point.Force));
            }

            plot.InvalidatePlot(true);
        }

        public void RenderSpline(double step = 0.005d)
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

            spline.X = x;
            spline.Y = y;

            if (data.Points.Count > 0)
            {
                if (spline.Fit())
                {
                    plot.LineSeries.Points.Clear();
                    plot.LineSeries.Color = OxyColors.Green;

                    for (double i = 0; i <= maximum; i += step)
                    {
                        plot.LineSeries.Points.Add(new DataPoint(i, spline.Calculation(i)));
                    }

                    plot.InvalidatePlot(true);
                }
            }
        }

        public void RenderLineAnnotation(bool updateData = false)
        {
            if (adjustZeroPoint)
            {
                plot.LineAnnotation.Slope = Slope;
                plot.LineAnnotation.Intercept = Intercept;

                plot.SetLineVisibility(true);
                plot.InvalidatePlot(updateData);
            }
            else
            {
                plot.SetLineVisibility();
                plot.InvalidatePlot(updateData);
            }
        }

        public override void NotifyOfPropertyChange(string propertyName = null)
        {
            base.NotifyOfPropertyChange(propertyName);
            {
                switch (propertyName)
                {
                    case "Slope":
                    case "Intercept":
                        plot.InvalidatePlot();
                        break;
                    case "SplineRho":
                    case "SplineNodes":
                        RenderSpline();
                        break;
                    case "AdjustZeroPoint":
                        RenderLineAnnotation();
                        break;
                }

                //var cbr25 = Math.Round(spline.Calculation(ZeroPoint + 2.5), 2);
                //var cbr50 = Math.Round(spline.Calculation(ZeroPoint + 5.0), 2);

                //plot.SetCbrValue(cbr25, cbr50);
            }
        }

        private void FillSlopeRangeFromDataPoints()
        {
            var min = double.MaxValue;
            var max = double.MinValue;
            var sum = 0.0;

            for (var i = 0; i < data.Points.Count - 1; i++)
            {
                var p1 = data.Points[i + 0];
                var p2 = data.Points[i + 1];

                var result = Math.Abs(p1.Force - p2.Force) / Math.Abs(p1.Stroke - p2.Stroke);

                if (result > max)
                    max = result;

                if (result < min)
                    min = result;

                sum += result;
            }

            Slope = sum / (data.Points.Count - 1);

            MinSlope = min;
            MaxSlope = max;
        }
    }
}