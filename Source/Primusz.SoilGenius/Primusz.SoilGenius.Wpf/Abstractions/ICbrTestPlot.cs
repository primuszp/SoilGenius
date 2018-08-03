using OxyPlot.Series;
using OxyPlot.Annotations;

namespace Primusz.SoilGenius.Wpf.Abstractions
{
    public interface ICbrTestPlot
    {
        LineSeries LineSeries { get; }

        ScatterSeries ScatterSeries { get; }

        LineAnnotation LineAnnotation { get; }

        void InvalidatePlot(bool updateData = false);

        void SetLineVisibility(bool isVisible = false);

        void SetCbrValue(double cbr25, double cbr50);
    }
}