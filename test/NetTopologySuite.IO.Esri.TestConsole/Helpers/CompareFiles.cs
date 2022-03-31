using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetTopologySuite.IO.Esri.TestConsole
{
    public static class CompareFiles
    {
        public static void PrintResults(string file1, string file2)
        {
            if (!File.Exists(file1))
                return;

            Console.Write(Path.GetFileName(file2).PadRight(20) + "  ");
            Console.ForegroundColor = ConsoleColor.Red;

            if (!File.Exists(file2))
            {
                Console.WriteLine("file does not exists.");
                Console.ResetColor();
                return;
            }

            var ext = Path.GetExtension(file1).ToLowerInvariant();
            var bytes1 = File.ReadAllBytes(file1);
            var bytes2 = File.ReadAllBytes(file2);

            if (bytes1.Length != bytes2.Length)
            {
                Console.WriteLine($"files have different size: {bytes1.Length} | {bytes2.Length}");
                Console.ResetColor();
                return;
            }

            if (ext == ".shp" || ext == ".shx")
                ValidateShpHeader(bytes1, bytes2);

            if (ext == ".dbf")
                ValidateDbfHeader(bytes1, bytes2);

            if (ValidateAllBytes(bytes1, bytes2, ext))
                return;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("OK");
            Console.ResetColor();
        }

        private static bool IsFileHeaderEnd(int pos, string ext)
        {
            if ((ext == ".shp" || ext == ".shx") && pos == 100)
                return true;

            if (ext == ".dbf" && pos == 32)
                return true;

            return false;
        }

        private static bool PrintDifferentBytes(int index, byte b1, byte b2, bool newLine)
        {
            if (b1 == b2)
                return false;


            var sb = new StringBuilder();
            sb.Append("- byte[" + index + "]: ");

            sb.Append(b1.ToString().PadLeft(3));
            sb.Append(" | ");
            sb.Append(b2.ToString().PadLeft(3));

            sb.Append("   '" + (char)b1);
            sb.Append("' | '");
            sb.Append((char)b2 + "'");
            sb.Replace(char.MinValue, '▬');

            if (newLine)
                Console.WriteLine();
            Console.WriteLine(sb.ToString());

            return true;
        }

        private static bool ValidateAllBytes(byte[] bytes1, byte[] bytes2, string ext)
        {
            bool hasErrors = false;
            Console.ForegroundColor = ConsoleColor.Red;
            for (int i = 0; i < bytes1.Length; i++)
            {
                if (hasErrors && IsFileHeaderEnd(i, ext))
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("--- File header end ---");
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                if (i > 0 && i < 4 && ext == ".dbf")
                    continue; // DBF file date

                if (PrintDifferentBytes(i, bytes1[i], bytes2[i], !hasErrors))
                    hasErrors = true;
            }
            Console.ResetColor();
            return hasErrors;
        }

        private static bool ValidateShpHeader(byte[] b1, byte[] b2)
        {
            List<string> results = new List<string>();

            ValidateBytesSpan(00, 00, b1, b2, results, "File code does not match");
            ValidateBytesSpan(24, 27, b1, b2, results, "File length does not match");
            ValidateBytesSpan(28, 31, b1, b2, results, "File version does not match");
            ValidateBytesSpan(32, 35, b1, b2, results, "Shape type does not match");

            ValidateBytesSpan(36, 36 + 7, b1, b2, results, "Xmin does not match");
            ValidateBytesSpan(44, 44 + 7, b1, b2, results, "Ymin does not match");
            ValidateBytesSpan(52, 52 + 7, b1, b2, results, "Xmax does not match");
            ValidateBytesSpan(60, 60 + 7, b1, b2, results, "Ymax does not match");
            ValidateBytesSpan(68, 68 + 7, b1, b2, results, "Zmin does not match");
            ValidateBytesSpan(76, 76 + 7, b1, b2, results, "Zmax does not match");
            ValidateBytesSpan(84, 84 + 7, b1, b2, results, "Mmin does not match");
            ValidateBytesSpan(92, 92 + 7, b1, b2, results, "Mmax does not match");

            return PrintValidationResults(results);
        }

        private static bool ValidateDbfHeader(byte[] b1, byte[] b2)
        {
            List<string> results = new List<string>();

            ValidateBytesSpan(00, 00, b1, b2, results, "dBASE version does not match");
            //ValidateBytesSpan(01, 03, b1, b2, results, "File update date does not match");
            ValidateBytesSpan(04, 07, b1, b2, results, "Record number does not match");
            ValidateBytesSpan(08, 09, b1, b2, results, "Header size does not match");
            ValidateBytesSpan(10, 11, b1, b2, results, "Record size does not match");
            ValidateBytesSpan(12, 28, b1, b2, results, "Reserved bytes do not match");
            ValidateBytesSpan(29, 29, b1, b2, results, "Encoding does not match");
            ValidateBytesSpan(30, 31, b1, b2, results, "Reserved bytes not match");

            return PrintValidationResults(results);
        }

        private static void ValidateBytesSpan(int startIndex, int endIndex, byte[] bytes1, byte[] bytes2, List<string> results, string errMsg)
        {
            if (startIndex < 0 || endIndex > bytes1.Length || endIndex > bytes2.Length)
            {
                results.Add("Index out of range of file bytes.");
                return;
            }
            for (int i = startIndex; i <= endIndex; i++)
            {
                if (bytes1[i] != bytes2[i])
                {
                    results.Add(errMsg.PadRight(20) + $"   [{startIndex}..{endIndex}]");
                    return;
                }
            }
        }

        private static bool PrintValidationResults(List<string> results)
        {
            if (results.Count < 1)
                return false;

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var res in results)
            {
                Console.WriteLine("- " + res);
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"--- file header start ---"); // Without new line
            Console.ResetColor();
            return true;
        }

    }
}
