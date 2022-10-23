using System;
using System.IO;

internal sealed class TestShapefiles
{
    public static readonly string Directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestShapefiles");

    public static string PathTo(string shpName)
    {
        return Path.Combine(Directory, shpName);
    }

    public static string PathToCountriesPt(string ext = ".shp")
    {
        return PathTo(Path.Combine("eurostat", "countries_pt" + ext));
    }

    public static string PathToCountriesLn(string ext = ".shp")
    {
        return PathTo(Path.Combine("eurostat", "countries_ln" + ext));
    }

    public static string PathToCountriesPg(string ext = ".shp")
    {
        return PathTo(Path.Combine("eurostat", "countries_pg" + ext));
    }

    /// <summary>
    /// Creates a uniquely named, temporary shapefile name and returns the full path of that file.
    /// </summary>
    /// <returns>The full path of the temporary Shapefile.</returns>
    public static string GetTempShpPath()
    {
        var tempFile = Path.GetTempFileName();
        var shpFile = Path.ChangeExtension(tempFile, ".shp");
        File.Move(tempFile, shpFile);
        return shpFile;
    }

    public static void DeleteShp(string shpPath)
    {
        var shpDir = Path.GetDirectoryName(shpPath);
        var shpName = Path.GetFileNameWithoutExtension(shpPath);
        foreach (var shpFile in System.IO.Directory.GetFiles(shpDir, shpName + ".*"))
        {
            File.Delete(shpFile);
        }
    }
}
