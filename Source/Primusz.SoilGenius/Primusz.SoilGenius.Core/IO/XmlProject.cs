using System;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;
using Primusz.SoilGenius.Core.Extensions;
using Primusz.SoilGenius.Core.Model;

namespace Primusz.SoilGenius.Core.IO
{
    public class XmlProject
    {
        public void Write(Stream stream, IList<CbrTestData> list)
        {
            var xdoc = new XDocument();
            var root = new XElement("Project");

            xdoc.Add(root);

            var maps = new Dictionary<string, XElement>();

            foreach (var test in list)
            {
                var xdir = new XElement("Directory");
                var directoryName = Path.GetDirectoryName(test.File);

                if (maps.ContainsKey(directoryName))
                {
                    xdir = maps[directoryName];
                }
                else
                {
                    xdir.SetAttributeValue("Name", directoryName);
                    maps.Add(directoryName, xdir);
                    root.Add(xdir);
                }

                var xtest = new XElement("TestData");
                xtest.SetAttributeValue("Name", test.Name);
                xtest.SetAttributeValue("File", Path.GetFileName(test.File));
                xtest.SetAttributeValue("Operator", test.Operator);
                xtest.SetAttributeValue("DateTime", test.DateTime.ToString("yyyy-MM-dd"));
                xtest.SetAttributeValue("Standard", test.Standard);

                var xsetting = new XElement("TestSetting");
                xsetting.SetAttributeValue("ControlSpeed", test.TestSetting.ControlSpeed);
                xsetting.SetAttributeValue("ControlStyle", test.TestSetting.ControlStyle);

                var xmixture = new XElement("TestMixture");
                xmixture.SetAttributeValue("SoilName", test.TestMixture.SoilName);
                xmixture.SetAttributeValue("BinderName", test.TestMixture.BinderName);
                xmixture.SetAttributeValue("BinderContent", $"{test.TestMixture.BinderContent}%");
                xmixture.SetAttributeValue("MoistureContent", $"{test.TestMixture.MoistureContent}%");
                xmixture.SetAttributeValue("Age", $"{test.TestMixture.Age}h");

                var xresults = new XElement("TestResults");
                xresults.SetAttributeValue("Force1", $"{test.TestResults.Force1:0.00}kN");
                xresults.SetAttributeValue("Force2", $"{test.TestResults.Force2:0.00}kN");

                xresults.SetAttributeValue("Slope", $"{test.TestResults.Slope:0.00}");
                xresults.SetAttributeValue("Intercept", $"{test.TestResults.Intercept:0.00}");

                xresults.SetAttributeValue("Rho", $"{test.TestResults.SplineRho:0.00}");
                xresults.SetAttributeValue("Nodes", $"{test.TestResults.SplineNodes:0}");

                xtest.Add(xsetting);
                xtest.Add(xmixture);
                xtest.Add(xresults);
                xdir.Add(xtest);
            }

            xdoc.Save(stream);
        }

        public IList<CbrTestData> Read(Stream stream)
        {
            var xdoc = XDocument.Load(stream);
            var list = new List<CbrTestData>();

            if (xdoc.Root != null)
            {
                var xdirs = xdoc.Root.Elements("Directory");

                foreach (var xdir in xdirs)
                {
                    var directory = xdir.Attribute("Name")?.Value;

                    foreach (var xtd in xdir.Elements("TestData"))
                    {
                        var test = new CbrTestData();

                        var name = xtd.Attribute("Name")?.Value;
                        var file = xtd.Attribute("File")?.Value;
                        var date = xtd.Attribute("DateTime")?.Value;
                        var standard = xtd.Attribute("Standard")?.Value;
                        var operatos = xtd.Attribute("Operator")?.Value;

                        foreach (var xitem in xtd.Elements())
                        {
                            if (xitem.Name == "TestSetting")
                            {
                                var controlspeed = xitem.Attribute("ControlSpeed")?.Value;
                                var controlstyle = xitem.Attribute("ControlStyle")?.Value;

                                test.TestSetting.ControlSpeed = controlspeed.ToDouble();
                                test.TestSetting.ControlStyle = (ControlStyle)Enum.Parse(typeof(ControlStyle), controlstyle);
                            }

                            if (xitem.Name == "TestMixture")
                            {
                                var soilName = xitem.Attribute("SoilName")?.Value;
                                var binderName = xitem.Attribute("BinderName")?.Value;
                                var binderContent = xitem.Attribute("BinderContent")?.Value;
                                var moistureContent = xitem.Attribute("MoistureContent")?.Value;
                                var age = xitem.Attribute("Age")?.Value;

                                test.TestMixture.SoilName = soilName;
                                test.TestMixture.BinderName = binderName;
                                test.TestMixture.BinderContent = binderContent.Replace("%", string.Empty).ToDouble();
                                test.TestMixture.MoistureContent = moistureContent.Replace("%", string.Empty).ToDouble();
                                test.TestMixture.Age = age.Replace("h", string.Empty).ToDouble();
                            }

                            if (xitem.Name == "TestResults")
                            {
                                var force1 = xitem.Attribute("Force1")?.Value;
                                var force2 = xitem.Attribute("Force2")?.Value;
                                var slope = xitem.Attribute("Slope")?.Value;
                                var intercept = xitem.Attribute("Intercept")?.Value;
                                var splineRho = xitem.Attribute("Rho")?.Value;
                                var splineNodes = xitem.Attribute("Nodes")?.Value;

                                if (force1 != null)
                                    test.TestResults.Force1 = force1.Replace("kN", string.Empty).ToDouble();

                                if (force2 != null)
                                    test.TestResults.Force2 = force2.Replace("kN", string.Empty).ToDouble();

                                if (slope != null)
                                    test.TestResults.Slope = slope.ToDouble();

                                if (intercept != null)
                                    test.TestResults.Intercept = intercept.ToDouble();

                                if (splineRho != null)
                                    test.TestResults.SplineRho = splineRho.ToDouble();

                                if (splineNodes != null)
                                    test.TestResults.SplineNodes = splineNodes.ToDouble();
                            }
                        }

                        test.Name = name;
                        test.File = Path.Combine(directory, file);
                        test.Operator = operatos;
                        test.Standard = standard;
                        test.DateTime = DateTime.Parse(date);

                        list.Add(test);
                    }
                }
            }
            return list;
        }
    }
}