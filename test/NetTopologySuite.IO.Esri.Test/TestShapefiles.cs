using System;
using System.IO;

internal sealed class TestShapefiles
{
    public static readonly string Directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestShapefiles");

    public static string PathTo(string shpName)
    {
        return Path.Combine(Directory, shpName);
    }
}
