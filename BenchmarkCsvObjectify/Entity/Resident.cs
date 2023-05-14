using CsvObjectify.Column;

namespace BenchmarkCsvObjectify.Entity
{
    class Resident : IEquatable<Resident?>
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool Married { get; set; }
        public int Children { get; set; }
        public string Address { get; set; }
        public string Employment { get; set; }
        public bool Disability { get; set; }

        public static ColumnMetadata[] GetMetadata =>
            new ColumnMetadata[]
            {
                new ColumnDefinition<string>(0, s => s.Trim(), "FirstName"),
                new ColumnDefinition<string>(1, s => s.Trim(), "MiddleName"),
                new ColumnDefinition<string>(2, s => s.Trim(), "LastName"),
                new ColumnDefinition<string>(3, s => s.Trim(), "Sex"),
                new ColumnDefinition<DateTime>(4, s => DateTime.Parse(s.Trim()), "DateOfBirth"),
                new ColumnDefinition<bool>(5, s => bool.Parse(s.Trim()), "Married"),
                new ColumnDefinition<int>(6, s => int.Parse(s.Trim()), "Children"),
                new ColumnDefinition<string>(7, s => s.Trim(), "Address"),
                new ColumnDefinition<string>(8, s => s.Trim(), "Employment"),
                new ColumnDefinition<bool>(9, s => bool.Parse(s.Trim()), "Disability")                
            };

        public override bool Equals(object? obj)
        {
            return Equals(obj as Resident);
        }       

        public bool Equals(Resident? other)
        {
            return other is not null &&
                   FirstName == other.FirstName &&
                   MiddleName == other.MiddleName &&
                   LastName == other.LastName &&
                   Sex == other.Sex &&
                   DateOfBirth == other.DateOfBirth &&
                   Married == other.Married &&
                   Children == other.Children &&
                   Address == other.Address &&
                   Employment == other.Employment &&
                   Disability == other.Disability;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(FirstName);
            hash.Add(MiddleName);
            hash.Add(LastName);
            hash.Add(Sex);
            hash.Add(DateOfBirth);
            hash.Add(Married);
            hash.Add(Children);
            hash.Add(Address);
            hash.Add(Employment);
            hash.Add(Disability);
            return hash.ToHashCode();
        }

        public static bool operator ==(Resident? left, Resident? right)
        {
            return EqualityComparer<Resident>.Default.Equals(left, right);
        }

        public static bool operator !=(Resident? left, Resident? right)
        {
            return !(left == right);
        }
    }
}
