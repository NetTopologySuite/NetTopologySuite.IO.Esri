# Esri Flavored dBASE format specification



## File Header

|         Byte         |     Data type      | Description                                |
| :------------------: | :----------------: | :----------------------------------------- |
|   1&nbsp;‑&nbsp;32   | byte&lsqb;32&rsqb; | Table descriptor (see below).              |
|  32&nbsp;‑&nbsp;64   | byte&lsqb;32&rsqb; | Field Descriptor (see below).              |
|  65&nbsp;‑&nbsp;96   | byte&lsqb;32&rsqb; | Field Descriptor.                          |
|         ...          |        ...         | Field Descriptors.                         |
| (n‑32)&nbsp;-&nbsp;n | byte&lsqb;32&rsqb; | Field Descriptor. Maximum [255 fields][limits].                          |
|         n+1          |        byte        | 0x0D stored as the file header terminator. |

<br/>

## Table descriptor

| Byte  |     Data type      |     Sample content      | Description                                                                                                                                                      |
| :---: | :----------------: | :---------------------: | :--------------------------------------------------------------------------------------------------------------------------------------------------------------- |
|   1   |        byte        |          0x03           | dBASE file version.                                                                                                                                              |
|  2-4  | byte&lsqb;3&rsqb;  | 121&verbar;01&verbar;16 | Date of last DBF file update in YYMMDD format.  Each byte contains the number as a binary.  Year starts from 1900, therefore possible year values are 1900-2155. |
|  5-8  |        uint        |           123           | Number of records in the table. Records limit in a table:  [1 Billion][limits]. Unsigned 32 bit integer (little-endian).                                         |
| 9-10  |       ushort       |           161           | Number of bytes in the file header (table descriptor header + field descriptor headers + header terminator char 0x0d). Unsigned 16 bit integer (little-endian).  |
| 11-12 |       ushort       |           123           | Number of bytes in the record. Unsigned 16 bit integer  (little-endian).                                                                                         |
| 13-29 | byte&lsqb;17&rsqb; |           ---           | Reserved. Filled with zeros.                                                                                                                                     |
|  30   |        byte        |          0x23           | Encoding flag based on Language Driver ID (LDID).                                                                                                                |
| 31-32 | byte&lsqb;2&rsqb;  |           ---           | Reserved. Filled with zeros.                                                                                                                                     |

<br/>

## Field Descriptor

| Byte  |     Data type      | Sample content | Description                                                                                                                                      |
| :---: | :----------------: | :------------: | :----------------------------------------------------------------------------------------------------------------------------------------------- |
| 1-10  | byte&lsqb;10&rsqb; |     "NAME"     | Field name may contain any letter, number or the undersocre (_) character. Max [10 characters][dBaseIV] long. Empty space [zero-filled][header]. |
|  11   |        byte        |      ---       | Reserved. Filled with zero.                                                                                                                      |
|  12   |        byte        |      'N'       | Field type in ASCII (C, D, F, N, L).                                                                                                             |
| 13-16 | byte&lsqb;4&rsqb;  |      ---       | Reserved. Filled with zeros.                                                                                                                     |
|  17   |        byte        |       19       | Field length.                                                                                                                                    |
|  18   |        byte        |       15       | Decimal places count for numeric field.                                                                                                          |
| 19-32 | byte&lsqb;14&rsqb; |      ---       | Reserved. Filled with zeros.                                                                                                                     |

<br/>

## Data Types

| Symbol | Data type |   Length    | Decimal places | Sample content | Description                                                                                   |
| :----: | :-------: | :---------: | :------------: | :------------: | :-------------------------------------------------------------------------------------------- |
|   C    | Character |    1-254    |       0        |     "NAME"     | All OEM code page characters - padded with blanks to the width of the field.                  |
|   D    |   Date    |      8      |       0        |   "20210116"   | Date stored as a string in the format YYYYMMDD (8 bytes). Any date from AD 1 to AD 9999.      |
|   N    |  Numeric  | [1-19][len] |  [1-15][len]   |   "12.3456"    | Number stored as a string, right justified, and padded with blanks to the width of the field. |
|   F    |   Float   | [1-19][len] |  [1-15][len]   |   "12.3456"    | Identical to Numeric. [Maintained for compatibility][field-types].                            |
|   L    |  Logical  |      1      |       0        |      'T'       | Initialized to 0x20 (space) otherwise 'T' or 'F'.                                             |

<br/>

## Data records

The records follow the header in the table file. Data records are preceded by one byte,
that is, a space (0x20) if the record is not deleted, an asterisk (0x2A) if the record is deleted.
Fields are packed into records without field separators or record terminators.
The end of the file is marked by a single byte, with the end-of-file marker,
an OEM code page character value of 26 (0x1A).

<br/>

## Reference

- [Data File Header Structure for the dBASE Version 7 Table File][header]
- [dBASE Field types][field-types]
- [Understanding DBF Essentials][len]
- [Converting DBF file of dBASE II/III into micro CDS/ISIS](http://web.simmons.edu/~chen/nit/NIT%2789/89-127-han.html)
- [Data types in dBase](https://www.promotic.eu/en/pmdoc/Subsystems/Db/dBase/DataTypes.htm)
- [dBASE IV Basics][dBaseIV]
- [Null values][null]
- [Null value substitution in shapefiles and dBASE (.dbf) files][shp-null]
- [What are the limits to dBASE databases?][limits]
- [Xbase Data file (*.dbf)][xbase]
- [DBF and DBT/FPT file structure][dbf-fpt-format]

[header]: http://www.dbase.com/KnowledgeBase/int/db7_file_fmt.htm
[field-types]: https://www.dbase.com/help/11_2/Design_Tables/IDH_TABLEDES_FIELD_TYPES.htm
[len]: http://www.sfu.ca/sasdoc/sashtml/accpc/z0214453.htm
[dBaseIV]: https://www.osti.gov/servlets/purl/10180088
[null]: http://www.dbase.com/help/2_80/Plus-en.htm#Language_Definition/IDH_LDEF_NULLVALUES.htm
[shp-null]: https://desktop.arcgis.com/en/arcmap/latest/manage-data/shapefiles/geoprocessing-considerations-for-shapefile-output.htm
[limits]: http://www.dbase.com/Knowledgebase/faq/dBASE_Limits_FAQ.html
[xbase]: https://www.clicketyclick.dk/databases/xbase/format/dbf.html
[dbf-fpt-format]: http://www.independent-software.com/dbase-dbf-dbt-file-format.html