# NetTopologySuite.IO.Esri

This library provides forward-only readers and writers for [Esri shapefiles](https://support.esri.com/en/white-paper/279).

## DBF

Shapefile feature attributes are held in a dBASE format file (.dbf extension). Each attribute record
has a one-to-one relationship with the associated shape record. Classes whose name starts
with `Dbf` (eg. `DbfReader`) provide direct access to dBASE files.

```c#
using (var dbf = new DbfReader(dbfPath))
{
    foreach (var record in dbf)
    {
        var fieldNames = record.Keys;
        foreach (var fieldName in fieldNames)
        {
            Console.WriteLine($"{fieldName,10} {record[fieldName]}");
        }
        Console.WriteLine();
    }
}
```

## SHP

The main file (.shp extension) is a variable-record-length file in which each record describes
a shape with a list of its vertices. Classes whose name starts with `Shp` (eg. `ShpPointReader`)
provide direct access to main file.

```c#
using (var shpStream = File.OpenRead(shpPath))
using (var shp = new ShpPointReader(shpStream))
{
    while (shp.Read())
    {
        Console.WriteLine(shp.Geometry);
    }
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

```c#
var features = new List<Feature>();
for (int i = 1; i < 5; i++)
{
    var lineCoords = new List<CoordinateZ>();
    lineCoords.Add(new CoordinateZ(i, i + 1, i));
    lineCoords.Add(new CoordinateZ(i, i, i));
    lineCoords.Add(new CoordinateZ(i + 1, i, i));
    var line = new LineString(lineCoords.ToArray());
    var mline = new MultiLineString(new LineString[] { line });

    var attributes = new AttributesTable();
    attributes.Add("date", new DateTime(2000, 1, i + 1));
    attributes.Add("float", i * 0.1);
    attributes.Add("int", i);
    attributes.Add("logical", i % 2 == 0);
    attributes.Add("text", i.ToString("0.00"));

    var feature = new Feature(mline, attributes);
    features.Add(feature);
}

Shapefile.WriteAllFeatures(features, shpPath);
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

## Validation

For performance reasons this library does not provide any kind of validation
during reading or writing shapefiles. If you write new shapefile it is your
responsibility to write properly formated data.

## Tests

This library was tested with shapefiles created by ArcMap 10.6.
TestConsole application read those files to memory and write it back to file system.
Then output files are checked byte by byte for differences.

At the moment inconsistency spoted is related to Date fields.
ArcMap 10.6 can create different null date representation in one .shp file!
Test file pt_utf8.shp have field named 'date' with such binary data:
```
=== record 0     Stream.Position: 673
...
date    Record.Position: 191
ReadString: '▬▬▬▬▬▬▬▬'                  // '▬' == char.MinValue == (char)0
=== record 1     Stream.Position: 1145
...
date    Record.Position: 191
ReadString: '        '
```
According to [Esri documentation](https://desktop.arcgis.com/en/arcmap/latest/manage-data/shapefiles/geoprocessing-considerations-for-shapefile-output.htm)
Null value substitution for Date field is *'Stored as zero'*. So this library saves null dates as zero (null) bytes which is also consistent with Numeric and Float fields.

Another inconsistency is related to polygons. When reading polygons containing
multiple parts [additional pre-processing](https://gis.stackexchange.com/a/147971/26684) is needed.
This can change internal rings order. This in turn may lead to different output files than
original ArcMap files.
