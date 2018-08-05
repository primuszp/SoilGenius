using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using Primusz.SoilGenius.Core.Extensions;
using Primusz.SoilGenius.Core.IO;
using Primusz.SoilGenius.Core.Model;

namespace Primusz.SoilGenius.Wpf.ViewModels
{
    public class ShellViewModel : Screen, IShell
    {
        #region Members

        private object activeViewModel;
        private readonly IEventAggregator aggregator;
        private IList<CbrTestData> list = new List<CbrTestData>();

        #endregion

        #region Properties

        public object ActiveViewModel
        {
            get { return activeViewModel; }
            set
            {
                if (activeViewModel != value)
                {
                    activeViewModel = value;
                    NotifyOfPropertyChange(() => ActiveViewModel);
                }
            }
        }

        #endregion

        public ShellViewModel(IEventAggregator aggregator)
        {
            ActiveViewModel = new CbrTestPlotViewModel(aggregator);

            if (aggregator != null)
            {
                this.aggregator = aggregator;
                this.aggregator.Subscribe(this);
            }
        }

        public void LoadTestData()
        {
            //using (FileStream stream = File.Open(@"DataSet/K1MESZ.dts", FileMode.Open))
            //{
            //    using (MultiensayoReader reader = new MultiensayoReader(stream))
            //    {
            //        TestData test = reader.Read();

            //        var list = new System.Collections.Generic.List<double>();
            //        foreach (var p in test.Points)
            //        {
            //            list.Add(p.Force);
            //        }

            //        var peaks = Core.Numerics.FindPeak.FindPeaks(list, 25);

            //        TestData td2 = new TestData();

            //        double max = test.Points.Max(p => p.Force);
            //        double treshold = 0.10 * max;

            //        foreach (var index in peaks)
            //        {
            //            var tp = test.Points[index];

            //            if (tp.Force >= (max - treshold) && (tp.Force <= max + treshold))
            //            {
            //                td2.Points.Add(test.Points[index]);
            //            }
            //        }

            //        var vm = ActiveViewModel as CbrTestViewModel;
            //        vm?.Tests.Add(new CbrTestDataViewModel(vm, td2));
            //    }
            //}

            var project = new XmlProject();
            list = project.Read(File.Open(@"DataSet/cbr_project.xml", FileMode.Open));

            var vm = ActiveViewModel as CbrTestPlotViewModel;

            foreach (var test in list)
            {
                var path = Path.Combine("DataSet", test.File);
                test.LoadTestFile(File.Open(path, FileMode.Open));

                vm?.Tests.Add(new CbrTestDataViewModel(test, aggregator));
            }
        }

        public void SaveTestData()
        {
            if (ActiveViewModel is CbrTestPlotViewModel vm)
            {
                var project = new XmlProject();
                var stream = File.Create(@"DataSet/test_project.xml");

                project.Write(stream, list);
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            DisplayName = $"SoilGenius v{version}";
        }
    }
}