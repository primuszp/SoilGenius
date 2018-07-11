using System;
using System.IO;
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

        private readonly IEventAggregator aggregator;
        private object activeViewModel;

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
            ActiveViewModel = new CbrTestViewModel();

            if (aggregator != null)
            {
                this.aggregator = aggregator;
                this.aggregator.Subscribe(this);
            }
        }

        public void LoadTestData()
        {
            using (FileStream stream = File.Open(@"DataSet/sample.dts", FileMode.Open))
            {
                using (MultiensayoReader reader = new MultiensayoReader(stream))
                {
                    TestData test = reader.Read();
                    var vm = ActiveViewModel as CbrTestViewModel;
                    vm?.Tests.Add(new CbrTestDataViewModel(vm, test));
                }
            }

            //XmlProjectReader reader = new XmlProjectReader();
            //var list = reader.Read(File.Open(@"DataSet/cbr_project.xml", FileMode.Open));

            //var vm = ActiveViewModel as CbrTestViewModel;

            //foreach (var test in list)
            //{
            //    string path = Path.Combine("DataSet", test.File);
            //    test.LoadTestFile(File.Open(path, FileMode.Open));

            //    vm?.Tests.Add(new CbrTestDataViewModel(vm, test));
            //}
        }

        //public void SaveTestData()
        //{
        //    CbrTestViewModel vm = ActiveViewModel as CbrTestViewModel;
        //    vm?.Save(@"X:\CBR\cbr1.dxf");
        //}

        protected override void OnInitialize()
        {
            base.OnInitialize();
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            DisplayName = $"SoilGenius v{version}";
        }
    }
}