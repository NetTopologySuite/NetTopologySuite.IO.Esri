from osgeo import ogr
import os

def print_valid_geom_count(path: str):
    print("Path:", path)
    data_source = ogr.Open(path)
    layer = data_source.GetLayer()
    geom_names = set()
    valid_count = 0
    invalid_count = 0
    for feature in layer:
        geom: ogr.Geometry = feature.GetGeometryRef()
        geom_names.add(geom.GetGeometryName())
        if geom.IsValid():
            valid_count += 1
        else:
            invalid_count += 1
    print("Geometry types", ", ".join(geom_names))
    print("-", "Valid geometry count:  ", valid_count)
    print("-", "Invalid geometry count:", invalid_count)
        
print_valid_geom_count("UKCS_Licences_WGS84.shp")
print_valid_geom_count("UKCS_Licensed_Blocks_WGS84.shp")
print_valid_geom_count("UKCS_SubAreas_WGS84.shp")