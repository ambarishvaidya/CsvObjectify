# CsvObjectify

This is a simple c# .net 6 library to convert given csv file to an enumerable of user-defined type (UDT). 
Each cell value in csv is parsed to be saved to a property in UDT. 
Simple parsing like int, string are baked into the library.

# Working
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
