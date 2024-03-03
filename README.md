# NetTopologySuite.IO.Esri

This library provides forward-only readers and writers for [Esri shapefiles](https://support.esri.com/en/white-paper/279).

## DBF

Shapefile feature attributes are held in a [dBASE format file](dBASE.md) (.dbf extension). Each attribute record
has a one-to-one relationship with the associated shape record. Classes whose name starts
with `Dbf` (eg. `DbfReader`) provide direct access to dBASE files.

```c#
using var dbf = new DbfReader(dbfPath);
foreach (var record in dbf)
{
    foreach (var fieldName in record.GetNames())
    {
        Console.WriteLine($"{fieldName,10} {record[fieldName]}");
    }
    Console.WriteLine();
}
```

## SHP

The main file (.shp extension) is a variable-record-length file in which each record describes
a shape with a list of its vertices. Classes whose name starts with `Shp` (eg. `ShpPointReader`)
provide direct access to main file.

```c#
foreach (var geometry in Shapefile.ReadAllGeometries(shpPath))
{
    Console.WriteLine(geometry);
}
```

## SHX

The index file (.shx extension) stores the offset and content length for each record in SHP file.
As there is no additional value, this file is ignored during reading shapefiles.
Writing SHX data is handled directly by `ShpWriter` classes.

## Shapefile

All three files described above form a shapefile. Unified access to shapefile triplet
is provided through classes whose name starts with `Shapefile` (eg. `ShapefilePointReader`).
Under the hood they are decorators wrapping `Dbf` and `Shp` classes.

### Reading shapefiles using c# code

```c#
foreach (var feature in Shapefile.ReadAllFeatures(shpPath))
{
    foreach (var attrName in feature.Attributes.GetNames())
    {
        Console.WriteLine($"{attrName,10}: {feature.Attributes[attrName]}");
    }
    Console.WriteLine($"     SHAPE: {feature.Geometry}");
    Console.WriteLine();
}
```

### Writing shapefiles using c# code

The most common variant of writing shapefiles is to use `Shapefile.WriteAllFeatures` method.

```c#
var features = new List<Feature>();
for (int i = 1; i < 5; i++)
{
    var lineCoords = new List<CoordinateZ>
    {
        new CoordinateZ(i, i + 1, i),
        new CoordinateZ(i, i, i),
        new CoordinateZ(i + 1, i, i)
    };
    var line = new LineString(lineCoords.ToArray());
    var mline = new MultiLineString(new LineString[] { line });

    var attributes = new AttributesTable
    {
        { "date", new DateTime(2000, 1, i + 1) },
        { "float", i * 0.1 },
        { "int", i },
        { "logical", i % 2 == 0 },
        { "text", i.ToString("0.00") }
    };

    var feature = new Feature(mline, attributes);
    features.Add(feature);
}
Shapefile.WriteAllFeatures(features, shpPath);
```

The most efficient way to write large shapefiles is to use `ShapefileWriter` class.
This variant should also be used when you need to write a shapefile with a attributes containing `null` values.

```c#
var fields = new List<DbfField>();
var dateField = fields.AddDateField("date");
var floatField = fields.AddFloatField("float");
var intField = fields.AddNumericInt32Field("int");
var logicalField = fields.AddLogicalField("logical");
var textField = fields.AddCharacterField("text");

var options = new ShapefileWriterOptions(ShapeType.PolyLine, fields.ToArray());
using (var shpWriter = Shapefile.OpenWrite(shpPath, options))
{
    for (var i = 1; i < 5; i++)
    {
        var lineCoords = new List<Coordinate>
        {
            new(i, i + 1),
            new(i, i),
            new(i + 1, i)
        };
        var line = new LineString(lineCoords.ToArray());
        var mline = new MultiLineString(new LineString[] { line });

        int? nullIntValue = null;

        shpWriter.Geometry = mline;
        dateField.DateValue = DateTime.Now;
        floatField.NumericValue = i * 0.1;
        intField.NumericValue = nullIntValue;
        logicalField.LogicalValue = i % 2 == 0;
        textField.StringValue = i.ToString("0.00");
        shpWriter.Write();
    }
}
```

## Encoding

The .NET Framework supports a large number of character encodings and code pages.
On the other hand, .NET Core only supports
[limited list](https://docs.microsoft.com/en-us/dotnet/api/system.text.codepagesencodingprovider.instance#remarks) of encodings.
To retrieve an encoding that is present in the .NET Framework on the Windows
desktop but not in .NET Core, you need to do the following:

1. Add to your project reference to to the [System.Text.Encoding.CodePages.dll](https://www.nuget.org/packages/System.Text.Encoding.CodePages/).
2. Put the following  line somewhere in your code:
   `Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);`

## Install using NuGet package manager

Stable releases are hosted on the default NuGet feed. You can install them using the following command on the package manager command line

```console
PM> NuGet\Install-Package NetTopologySuite.IO.Esri.Shapefile
```

