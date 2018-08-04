using System;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;
using Primusz.SoilGenius.Core.Extensions;
using Primusz.SoilGenius.Core.Model;

namespace Primusz.SoilGenius.Core.IO
{
    public class XmlProjectReader
    {
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
                            }
                        }

                        var path = Path.Combine(directory, file);

                        var test = new CbrTestData
                        {
                            Name = name,
                            File = path,
                            Operator = operatos,
                            Standard = standard,
                            DateTime = DateTime.Parse(date)
                        };

                        list.Add(test);
                    }
                }
            }
            return list;
        }
    }
}