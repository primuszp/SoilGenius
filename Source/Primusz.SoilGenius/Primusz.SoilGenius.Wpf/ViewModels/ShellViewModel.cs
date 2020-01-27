using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Caliburn.Micro;
using Primusz.SoilGenius.Core.Extensions;
using Primusz.SoilGenius.Core.IO;
using Primusz.SoilGenius.Core.Model;
using Screen = Caliburn.Micro.Screen;

namespace Primusz.SoilGenius.Wpf.ViewModels
{
    public class ShellViewModel : Screen, IShell
    {
        #region Members

        private string soilName = "untreated_corrected";
        private object activeViewModel;
        private readonly IEventAggregator aggregator;
        private List<CbrTestData> list = new List<CbrTestData>();

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
            list = project.Read(File.Open($@"C:\DataSet\{soilName}.xml", FileMode.Open));

            var vm = ActiveViewModel as CbrTestPlotViewModel;

            foreach (var test in list)
            {
                var path = Path.Combine(@"C:\DataSet", test.File);
                test.LoadTestFile(File.Open(path, FileMode.Open));

                vm?.Tests.Add(new CbrTestDataViewModel(test, aggregator));
            }
        }

        public void SaveTestData()
        {
            if (ActiveViewModel is CbrTestPlotViewModel vm)
            {
                var project = new XmlProject();
                var stream = File.Create($@"C:\DataSet\{soilName}.xml");

                project.Write(stream, list);
            }
        }

        public void ImportTestData()
        {
            var testDateTime = new DateTime(2008, 1, 1);
            var testOperator = "SOE";
            var testBinderName = "Lime";
            var path = @"C:\DataSet";

            using (var fbd = new FolderBrowserDialog())
            {
                var result = fbd.ShowDialog();

                if (result == DialogResult.OK &&
                    !string.IsNullOrEmpty(fbd.SelectedPath))
                {
                    var files = Directory.GetFiles(fbd.SelectedPath);

                    list.Clear();

                    foreach (var file in files)
                    {
                        var fileInfo = new FileInfo(file);

                        var fileName = fileInfo.Name;
                        var directoryName = fileInfo.Directory?.Name;

                        var testData = new CbrTestData();

                        if (directoryName != null)
                        {
                            testData.File = Path.Combine(directoryName, fileName);
                        }

                        testData.Name = Path.GetFileNameWithoutExtension(file);
                        testData.Operator = testOperator;
                        testData.DateTime = testDateTime;
                        testData.TestMixture.SoilName = directoryName;
                        testData.TestMixture.BinderName = testBinderName;

                        var buffer = testData.Name.Split('-');
                        testData.TestMixture.BinderContent = buffer[1].ToDouble();
                        testData.TestMixture.MoistureContent = buffer[2].ToDouble();
                        testData.TestMixture.Age = buffer[3].ToDouble();

                        list.Add(testData);
                    }

                    var dir = Path.GetFileName(fbd.SelectedPath);
                    path = Path.Combine(path, dir.ToLower() + "_project.xml");

                    var project = new XmlProject();
                    var stream = File.Create(path);

                    //list = list.OrderBy(x => x.TestMixture.BinderContent)
                    //    .ThenBy(x => x.TestMixture.Age)
                    //    .ThenBy(x => x.TestMixture.MoistureContent)
                    //    .ToList();

                    project.Write(stream, list);
                }
            }
        }

        public void ExportTestData()
        {
            var age = string.Empty;
            var lines = new List<string>();

            foreach (var item in list)
            {
                var b = item.TestMixture.BinderContent.ToString("0").Replace(',', '.');
                var m = item.TestMixture.MoistureContent.ToString("0.0").Replace(',', '.');
                var a = item.TestMixture.Age.ToString("0");

                var f1 = item.TestResults.Force1.ToString("0.00").Replace(',', '.');
                var f2 = item.TestResults.Force2.ToString("0.00").Replace(',', '.');

                if (age != a)
                {
                    age = a;
                    lines.Add(Environment.NewLine);
                    lines.Add($"# age={age}");
                }

                var line = b + "\t" + m + "\t" + f1 + "\t" + f2;

                lines.Add(line);
            }

            File.WriteAllLines($@"C:\DataSet\{soilName}_gp.txt", lines);
        }

        public void ExportTestData2()
        {
            var lines = new List<string>();

            foreach (var item in list)
            {
                var c = item.Name;
                var b = item.TestMixture.BinderContent.ToString("0");
                var m = item.TestMixture.MoistureContent.ToString("0.0");
                var a = item.TestMixture.Age.ToString("0");

                var f1 = item.TestResults.Force1.ToString("0.00");
                var f2 = item.TestResults.Force2.ToString("0.00");

                var line = c + "\t" + b + "\t" + m + "\t" + a + "\t" + f1 + "\t" + f2 + "\t";

                lines.Add(line);
            }

            File.WriteAllLines($@"C:\DataSet\{soilName}.txt", lines);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            DisplayName = $"SoilGenius v{version}";
        }
    }
}