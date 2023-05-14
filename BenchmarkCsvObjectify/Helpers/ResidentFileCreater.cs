using BenchmarkCsvObjectify.Entity;
using Bogus;
using Faker;

namespace BenchmarkCsvObjectify.Helpers
{
    internal static class ResidentFileCreater
    {
        private static Resident[] GenerateResidents(int count)
        {
            Randomizer randomizer = new Randomizer();
            var bogusResident = new Faker<Resident>("en")
                .RuleFor(x => x.FirstName, x => x.Person.FirstName)
                .RuleFor(x => x.MiddleName, x => x.Person.FirstName)
                .RuleFor(x => x.LastName, x => x.Person.LastName)
                .RuleFor(x => x.Sex, x => x.Person.Gender.ToString())
                .RuleFor(x => x.DateOfBirth, x => DateTime.Parse(x.Person.DateOfBirth.ToString("dd-MMMM-yyyy")))
                .RuleFor(x => x.Married, x => x.PickRandom<bool>(new bool[] { true, false }))
                .RuleFor(x => x.Children, x => x.PickRandom<int>(Enumerable.Range(0, 6)))
                .RuleFor(x => x.Address, x => "\"" + x.Person.Address.Street + " "+ x.Person.Address.City + " " + x.Person.Address.ZipCode + "\"")
                .RuleFor(x => x.Employment, x => "\"" + x.Company.CompanyName() + "\"")
                .RuleFor(x => x.Disability, x => x.PickRandom<bool>(new bool[] { true, false }));

            Resident[] residents = new Resident[count];

            for (int i = 0; i < count; i++)
            {
                residents[i] = bogusResident.Generate();
            }

            return residents;
        }

        internal static void CreateCsvFile(int count, string filePath)
        {
            var residents = GenerateResidents(count);
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("First Name,Middle Name,Last Name,Sex,Date of Birth,Married,Children,Address,Employment,Disability");

                foreach (var resident in residents)
                {
                    writer.WriteLine($"{resident.FirstName},{resident.MiddleName},{resident.LastName}," +
                        $"{resident.Sex},{resident.DateOfBirth:yyyy-MM-dd},{resident.Married},{resident.Children}," +
                        $"{resident.Address},{resident.Employment},{resident.Disability}");
                }
            }
        }
    }
}
