using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;

namespace Primusz.SoilGenius.Wpf.Abstractions
{
    public interface ITestPlot
    {
        PlotModel PlotModel { get; }

        LineSeries LineSeries { get; }

        ScatterSeries ScatterSeries { get; }

        LineAnnotation LineAnnotation { get; }
    }
}