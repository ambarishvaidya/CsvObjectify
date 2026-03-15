# CsvObjectify

This is a simple c# .NET 10 library to convert given csv file to an enumerable of user-defined type (UDT). 
Each cell value in csv is parsed to be saved to a property in UDT. 
Simple parsing like int, string are baked into the library.

## Version 2.0.0.0

This version introduces significant performance improvements:
- **Span-based parsing**: The parser now uses `ReadOnlySpan<char>` for zero-allocation string processing
- **Improved performance**: The new implementation delivers better throughput compared to the previous version
- **.NET 10 support**: Updated to target .NET 10 for the latest framework features and optimizations

The previous version used `ParseWithoutSpan` which relied on the `TextFieldParser` library. Version 2.0.0.0 now uses `ParseWithSpan` as the default parsing method, providing enhanced performance through modern memory-efficient techniques.

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
If all the column definitions are defined by index, then IsFirsRowHeader can be false.

The CsvParser<Resident> for the UDT is then created passing the CsvProfile. This returns ICsvParser with single method to get the UDT.
