using NetTopologySuite.IO.Esri.Dbf;
using System;

namespace NetTopologySuite.IO.Esri.TestConsole.Tests
{
    public class DBF_ReadIterator : Test
    {
        public override void Run()
        {
            var dbfPath = GetTestFilePath("arcmap/shp/pt_utf8.dbf");
            using var dbf = new DbfReader(dbfPath);
            foreach (var fields in dbf)
            {
                Console.WriteLine("Record ID: " + fields["Id"]);
                foreach (var fieldName in fields.GetNames())
                {
                    Console.WriteLine($"{fieldName, 10} {fields[fieldName]}");
                }
                Console.WriteLine();
            }
        }





    }
}
