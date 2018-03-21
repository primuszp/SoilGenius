using System.IO;
using System.Reflection;
using Microsoft.Win32;
using Caliburn.Micro;
using Primusz.SoilGenius.Core.IO;
using Primusz.SoilGenius.Core.Model;

namespace Primusz.SoilGenius.Wpf.ViewModels
{
    public class ShellViewModel :  Screen, IShell
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
            ActiveViewModel = new CbrTestSampleViewModel(new CbrTestSample());

            if (aggregator != null)
            {
                this.aggregator = aggregator;
                this.aggregator.Subscribe(this);
            }
        }

        public void LoadTestData()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                using (FileStream stream = File.Open(dialog.FileName, FileMode.Open))
                {
                    using (MultiensayoReader reader = new MultiensayoReader(stream))
                    {
                        CbrTestSample sample = reader.Read();
                        ActiveViewModel = new CbrTestSampleViewModel(sample);
                    }
                }
            }
        }

        public void SaveTestData()
        {
            CbrTestSampleViewModel vm = ActiveViewModel as CbrTestSampleViewModel;
            vm?.Save(@"X:\CBR\cbr1.dxf");
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            DisplayName = $"SoilGenius v{version}";
        }
    }
}