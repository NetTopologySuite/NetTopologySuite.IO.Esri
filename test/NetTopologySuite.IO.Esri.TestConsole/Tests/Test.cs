using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Dbf;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shapefiles.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NetTopologySuite.IO.Esri.TestConsole.Tests
{
    public abstract class Test
    {
        public string Title { get; protected set; }

        public Test()
        {
            Title = GetType().Name.Replace("Test", "").Replace("_", " - ");
        }

        public abstract void Run();

        public static readonly string FieldSpace = " ".PadRight(12);

        public static string TestDataDir = GetTestDataDir(Assembly.GetExecutingAssembly().Location);

        private static string GetTestDataDir(string dir)
        {
            dir = Path.GetDirectoryName(dir);
            var testDataDir = Path.Combine(dir, "TestData");

            if (Directory.Exists(testDataDir))
                return testDataDir;

            if (dir.Length < 4) // "C:\"
            {
                return "";
            }

            return GetTestDataDir(dir);
        }

        public static string GetTestFilePath(string filePath)
        {
            if (File.Exists(filePath))
                return filePath;

            var path = Path.Combine(TestDataDir, filePath);
            path = Path.GetFullPath(path);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Test file not found: " + filePath);
            }
            return path;
        }

        public static string GetTempFilePath(string fileName)
        {
            var tempFilePath = Path.Combine(TestDataDir, "temp", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(tempFilePath));
            return tempFilePath;
        }

        public static string CreateFileCopyDir(string sourceFilePath)
        {
            var filePath = GetTestFilePath(sourceFilePath);                     // arcmap/shp/point.shp
            var copyDirPath = Path.GetDirectoryName(filePath) + "-copy";        // arcmap/shp           => arcmap/shp-copy
            Directory.CreateDirectory(copyDirPath);

            return Path.Combine(copyDirPath, Path.GetFileName(sourceFilePath)); // arcmap/shp-copy/point.shp
        }

        public override string ToString()
        {
            return Title;
        }

        protected void PrintFieldNames(IReadOnlyList<DbfField> fields)
        {
            Console.WriteLine("FIELD LIST");
            Console.WriteLine("----------");
            foreach (var field in fields)
            {
                Console.WriteLine("  " + field.Name.PadRight(10) + " [" + field.FieldType.ToString().PadRight(9) + field.Length.ToString().PadLeft(4) + field.NumericScale.ToString().PadLeft(3) + "]");
            }
            Console.WriteLine();
        }

        protected void PrintFieldValues(IReadOnlyDictionary<string, object> values)
        {
            Console.WriteLine();
            foreach (var nameVal in values)
            {
                PrintFieldValue(nameVal.Key, nameVal.Value);
            }
        }

        protected void PrintFieldValue(string name, object value)
        {
            name += ": ";
            Console.WriteLine(name.PadRight(12) + ToText(value));
        }

        public void PrintFields(DbfReader dbf)
        {
            PrintFieldNames(dbf.Fields);
            foreach (var values in dbf)
            {
                PrintFieldValues(values);

            }
        }

        protected void PrintGeometry(Geometry geometry)
        {
            if (geometry.IsEmpty)
            {
                PrintFieldValue("SHAPE", "NullShape");
                return;
            }

            PrintFieldValue("SHAPE", geometry.GeometryType);
            if (geometry is GeometryCollection geomColl)
            {
                for (int i = 0; i < geometry.NumGeometries; i++)
                {
                    Console.WriteLine(FieldSpace + "Part " + (i + 1) + ":");
                    var part = geomColl[i];
                    PrintCoordinates(part.Coordinates);
                }
                Console.WriteLine(FieldSpace + "Parts end.");
            }
            else
            {
                PrintCoordinates(geometry.Coordinates);
            }
        }

        private void PrintCoordinates(Coordinate[] coordinates)
        {
            foreach (var c in coordinates)
            {
                Console.WriteLine(FieldSpace + "  " + c);
            }

        }

        public void PrintFeature(Feature feature)
        {
            Console.WriteLine();
            foreach (var name in feature.Attributes.GetNames())
            {
                PrintFieldValue(name, feature.Attributes[name]);
            }
            PrintGeometry(feature.Geometry);
        }

        public void PrintFeatures(ShapefileReader shp)
        {
            PrintFieldNames(shp.Fields);

            PrintRecordListHeader();
            foreach (var feature in shp)
            {
                PrintFeature(feature);
                Console.WriteLine();
            }
        }


        protected string ToText(object value)
        {
            if (value == null)
                return "<null>";

            if (value is string s)
                return "'" + s + "'";

            return value.ToString();
        }
        protected void PrintRecordListHeader()
        {
            Console.WriteLine("RECORD LIST");
            Console.WriteLine("-----------");
            Console.WriteLine();
        }

        public void PrintSectionTitle(string title)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine(new string('_', 80));
            Console.WriteLine(title);
            Console.WriteLine(new string('-', 80));
            Console.WriteLine();
            Console.ResetColor();
        }

        public void PrintValidationResult(bool isValid, string message)
        {
            Console.ForegroundColor = isValid ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public CoordinateSequence CreateCoordinateSequence(int size, Ordinates ordinates = Ordinates.XY)
        {
            return NtsGeometryServices.Instance.DefaultCoordinateSequenceFactory.Create(size, ordinates);
        }
    }



}
