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
                        var standard = xtd.Attribute("Standard")?.Value;
                        var operatos = xtd.Attribute("Operator")?.Value;
                        var controlspeed = xtd.Attribute("ControlSpeed")?.Value;

                        var xpoints = xtd.Elements("TestPoints");

                        foreach (var xpoint in xpoints)
                        {
                            var file = xpoint.Attribute("File")?.Value;
                            var code = xpoint.Attribute("Code")?.Value;
                            var date = xpoint.Attribute("DateTime")?.Value;

                            var path = Path.Combine(directory, file);

                            var test = new CbrTestData
                            {
                                Code = code,
                                Name = name,
                                File = path,
                                Operator = operatos,
                                Standard = standard,
                                DateTime = DateTime.Parse(date),
                                ControlSpeed = controlspeed.ToDouble()
                            };

                            list.Add(test);
                        }
                    }
                }
            }
            return list;
        }
    }
}