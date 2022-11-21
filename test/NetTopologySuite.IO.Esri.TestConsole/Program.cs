using NetTopologySuite.IO.Esri.TestConsole.Tests;
using System;

namespace NetTopologySuite.IO.Esri.TestConsole
{
    class Program
    {

        private static readonly Test[] TestList = {
            new DBF_ReadIterator(), //"arcmap/shp/fields_utf8.dbf"
            new DBF_ReadCore(),
            new SHP_ReadCore(),

            new Shapefile_ReadPointIterator(),
            new Shapefile_ReadPointFeatures(),
            new Shapefile_ReadPointCore(),

            new Shapefile_WriteLineIterator(),
            new Shapefile_WriteLineFeatures(),
            new Shapefile_WriteLineCore(),

            new Shapefile_WritePointCore(),

            new ReadArcMapFilesTest(),
            new CopyAndCompareArcMapShapefilesTest(),
        };


        static void Main(string[] args)
        {
            Console.WriteLine("NetTopologySuite.IO.Esri.TestConsole");

            if (string.IsNullOrEmpty(Test.TestDataDir))
            {
                Console.WriteLine("ERROR: TestData folder not found.");
                Console.WriteLine("Pres any key to exit.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("TestData directory: " + Test.TestDataDir);
            Console.WriteLine();

            WriteTestList();

            var testNumber = Console.ReadLine();
            while (!string.IsNullOrEmpty(testNumber))
            {
                RunTest(testNumber);
                WriteTestList();
                testNumber = Console.ReadLine();
            }
        }

        private static void WriteTestList()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("TEST LIST");
            Console.WriteLine("---------");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            for (int i = 0; i < TestList.Length; i++)
            {
                var test = TestList[i];
                Console.WriteLine((i + 1).ToString() + ": " + test.ToString());
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Write test number or pres ENTER to exit: ");
        }

        private static void WriteError(Exception? ex, bool exitApp = false)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: " + ex?.Message);
            ex = ex?.InnerException;
            while (ex != null)
            {
                Console.WriteLine("- " + ex.Message);
                ex = ex.InnerException;
            }
            if (exitApp)
            {
                Console.WriteLine("Pres any key to exit.");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        private static void RunTest(string testNumber)
        {
            Console.Clear();
            Console.ResetColor();
            if (int.TryParse(testNumber, out int number) && number > 0 && number <= TestList.Length)
            {
                var test = TestList[number - 1];
                var testName = test.ToString(); ;
                Console.WriteLine(testName);
                Console.WriteLine(new string('=', testName.Length));
                Console.WriteLine();

                try
                {
                    test.Run();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Test {testNumber} ({testName}) failed.");
                    WriteError(ex);
                }
                finally
                {
                    testName = test.Title + " finished.";
                    Console.WriteLine(testName);
                    Console.WriteLine(new string('=', testName.Length));
                    Console.WriteLine();
                }

            }
            else
            {
                Console.WriteLine("Invalid test number.");
            }
            Console.WriteLine();
        }
    }
}
