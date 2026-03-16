# CsvObjectify

This is a simple c# .NET 10 library to convert given csv file to an enumerable of user-defined type (UDT). 
Each cell value in csv is parsed to be saved to a property in UDT. 
Simple parsing like int, string are baked into the library.

## Version 2.0.2

This version exposes three parsing methods: `Parse()`, `ParseWithSpan()`, and `ParseWithoutSpan()`. The default `Parse()` method uses `ParseWithSpan()` for optimal performance.

**Key improvements:**
- **Span-based parsing**: Uses `ReadOnlySpan<char>` for zero-allocation string processing
- **Performance**: Up to 2x faster with 85% less memory allocation compared to `ParseWithoutSpan`
- **.NET 10 support**: Updated to target .NET 10 for latest framework features and optimizations

The legacy `ParseWithoutSpan()` method (using `TextFieldParser`) remains available for compatibility or comparison purposes. See benchmark results below for detailed performance metrics.

# Usage

## Define Column Mappings
Create Columns defn specifying type for each column of interest in the csv file.

	new ColumnDefinition<string>("Middle Name", s => s.Trim(), "MiddleName"),
  	new ColumnDefinition<DateTime>(4, s => DateTime.Parse(s), "DateOfBirth"),
	
This matches the provided csv column name or csv index to a property name of specified type in Model (UDT).

	internal class Resident
	{
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public DateTime DateOfBirth { get; set; }
	...
		
With the required properties in the Model are mapped to csv columns, CsvProfile is created passing the column definition array.		

	CsvProfile.Build(new ColumnMetadata[]
	{
		ColumnDefinitionHelper.CreateStringColumn("First Name", "FirstName"),
		new ColumnDefinition<string>("Middle Name", s => s.Trim(), "MiddleName"),
		new ColumnDefinition<string>(2, s => s.Trim(), "LastName"),
		new ColumnDefinition<DateTime>(4, s => DateTime.Parse(s), "DateOfBirth"),
		new ColumnDefinition<char>("Disability", s => ParseDisablity(s), "Disability")
	},
	new FileDetails()
	{
		FilePath = @"YourCsvFilepath.csv",
		IsFirstRowHeader = true
	})
	
CsvProfile will check if the column definitions and the csv file passed can work together.
If all the column definitions are defined by index, then IsFirstRowHeader can be false.

The CsvParser<Resident> for the UDT is then created passing the CsvProfile. This returns ICsvParser with single method to get the UDT.

# Benchmark Results

The count before each benchmarking indicates the number of rows in the csv file used for benchmarking.

BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.26200.8037)
Intel Core Ultra 9 185H, 1 CPU, 22 logical and 16 physical cores
.NET SDK=10.0.200

1000000
|           Method |    Mean |    Error |   StdDev |         Gen0 |        Gen1 |      Gen2 | Allocated |
|----------------- |--------:|---------:|---------:|-------------:|------------:|----------:|----------:|
| ParseWithoutSpan | 9.764 s | 0.1929 s | 0.2640 s | 3497000.0000 | 877000.0000 | 9000.0000 |  40.32 GB |
|    ParseWithSpan | 4.746 s | 0.0515 s | 0.0482 s |  484000.0000 | 126000.0000 | 8000.0000 |    5.6 GB |

100000
|           Method |       Mean |    Error |   StdDev |        Gen0 |        Gen1 |      Gen2 | Allocated |
|----------------- |-----------:|---------:|---------:|------------:|------------:|----------:|----------:|
| ParseWithoutSpan | 1,068.1 ms | 12.17 ms | 10.79 ms | 401000.0000 | 103000.0000 | 6000.0000 | 4645.2 MB |
|    ParseWithSpan |   484.2 ms |  9.20 ms |  9.84 ms |  50000.0000 |  15000.0000 | 4000.0000 | 573.47 MB |


10000
|           Method |      Mean |    Error |   StdDev |       Gen0 |      Gen1 |      Gen2 | Allocated |
|----------------- |----------:|---------:|---------:|-----------:|----------:|----------:|----------:|
| ParseWithoutSpan | 116.45 ms | 2.255 ms | 2.109 ms | 39333.3333 | 4000.0000 | 1666.6667 | 455.22 MB |
|    ParseWithSpan |  43.34 ms | 0.643 ms | 0.601 ms |  4750.0000 | 1500.0000 |  916.6667 |  57.43 MB |


1000
|           Method |      Mean |     Error |    StdDev |      Gen0 |     Gen1 | Allocated |
|----------------- |----------:|----------:|----------:|----------:|---------:|----------:|
| ParseWithoutSpan | 10.703 ms | 0.1922 ms | 0.1704 ms | 3859.3750 |  31.2500 |  45.28 MB |
|    ParseWithSpan |  3.772 ms | 0.0374 ms | 0.0350 ms |  476.5625 | 304.6875 |   5.74 MB |


100
|           Method |       Mean |    Error |   StdDev |     Gen0 |    Gen1 |  Allocated |
|----------------- |-----------:|---------:|---------:|---------:|--------:|-----------:|
| ParseWithoutSpan | 1,706.1 us | 33.83 us | 72.83 us | 351.5625 | 78.1250 | 4280.45 KB |
|    ParseWithSpan |   484.5 us |  9.51 us | 11.33 us |  47.8516 |  7.8125 |  596.04 KB |


10
|           Method |     Mean |   Error |   StdDev |    Gen0 |   Gen1 | Allocated |
|----------------- |---------:|--------:|---------:|--------:|-------:|----------:|
| ParseWithoutSpan | 321.7 us | 6.40 us | 13.78 us | 36.1328 | 3.9063 | 442.41 KB |
|    ParseWithSpan | 144.0 us | 2.75 us |  2.57 us |  5.3711 | 0.2441 |  66.47 KB |