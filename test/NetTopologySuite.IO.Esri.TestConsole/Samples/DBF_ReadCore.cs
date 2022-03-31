using NetTopologySuite.IO.Esri.Dbf;
using System;

namespace NetTopologySuite.IO.Esri.TestConsole.Tests
{
    public class DBF_ReadCore : Test
    {
        public override void Run()
        {
            var dbfPath = GetTestFilePath("arcmap/shp/pt_utf8.dbf");

            using (var dbf = new DbfReader(dbfPath))
            {
                foreach (var field in dbf.Fields)
                {
                    Console.WriteLine(field);
                }
                Console.WriteLine();

                while (dbf.Read(out var deleted))
                {
                    if (deleted)
                        continue;

                    Console.WriteLine("Record ID: " + dbf.Fields["Id"].Value);
                    foreach (var field in dbf.Fields)
                    {
                        PrintFieldValue(field.Name, field.Value);
                    }
                    Console.WriteLine();
                }
            }
        }



    }
}
