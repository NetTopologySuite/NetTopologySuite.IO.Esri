// See https://aka.ms/new-console-template for more information


using NetTopologySuite.IO.Esri;
using NetTopologySuite.IO.Esri.Shapefiles.Readers;

string path = @"C:\work\shp\10m_cultural\sample\ne_10m_admin_0_countries_fra.shp";
string pathCopy= @"C:\work\shp\10m_cultural\sample\ne_10m_admin_0_countries_fra_cpy.shp";
var config = new ShapefileReaderOptions()
{
	GeometryBuilderMode = GeometryBuilderMode.IgnoreInvalidShapes
};
var features = Shapefile.ReadAllFeatures(path,config );
Shapefile.WriteAllFeatures(features, pathCopy);


