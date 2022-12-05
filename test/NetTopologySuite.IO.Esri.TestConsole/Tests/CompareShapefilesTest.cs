
using System;
using System.IO;
using System.Linq;
using NetTopologySuite.IO.Esri.Shapefiles.Writers;

namespace NetTopologySuite.IO.Esri.TestConsole.Tests
{
    public abstract class CompareShapefilesTest : Test
    {
        protected void CompareShapefiles(string srcFile, string destFile)
        {
            var fileName = "arcmap/shp/" + Path.GetFileName(srcFile);
            Console.WriteLine(fileName);

            Console.WriteLine("===");

            CompareFilesCore(srcFile, destFile, ".shp");
            CompareFilesCore(srcFile, destFile, ".shx");
            CompareFilesCore(srcFile, destFile, ".dbf");
            CompareFilesCore(srcFile, destFile, ".cpg");
            CompareFilesCore(srcFile, destFile, ".prj");

            Console.WriteLine();
        }

        private void CompareFilesCore(string file1, string file2, string ext)
        {
            ext = ext.ToLowerInvariant();
            file1 = Path.ChangeExtension(file1, ext);
            file2 = Path.ChangeExtension(file2, ext);
            CompareFiles.PrintResults(file1, file2);
        }

        protected string CopyShapefile(string srcPath)
        {
            var destPath = CreateFileCopyDir(srcPath);
            //srcPath = GetTestFilePath(srcPath);

            using (var src = Shapefile.OpenRead(srcPath))
            using (var copy = Shapefile.OpenWrite(destPath, new ShapefileWriterOptions(src)))
            {
                var srcFeatures = src.ToArray();
                copy.Write(srcFeatures);
            }
            return destPath;
        }
    }
}
