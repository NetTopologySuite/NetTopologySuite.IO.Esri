using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shp.Readers;
using System;
using System.IO;

namespace NetTopologySuite.IO.Esri.TestConsole.Tests
{
    public class SHP_ReadCore : Test
    {
        public override void Run()
        {
            var shpPath = GetTestFilePath("arcmap/shp/pt_utf8.shp");

            using (var shpStream = File.OpenRead(shpPath))
            using (var shp = new ShpPointReader(shpStream))
            {
                while (shp.Read())
                {
                    Console.WriteLine(shp.Shape);
                }
            }
        }
    }
}
