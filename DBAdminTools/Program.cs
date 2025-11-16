using ClosedXML.Excel;
using CompanyDBSQL.Data;
using CompanyDBSQL.Models;

namespace DBAdminTools
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var context = new AppDbContext();
            var dbservice = new DbService(context);

            while (true)
            {
                Console.Clear();

                Console.WriteLine("*** Company Database admin tools *** ");
                Console.WriteLine("1. Update CSV");
                Console.WriteLine("2. Create a Company");
                Console.WriteLine("3. View all companies");
                Console.WriteLine("4. Search Company");
                Console.WriteLine("5 Delete Company");
                Console.WriteLine("6.Export to CSV");
                Console.WriteLine("7. Database statistics");
                Console.WriteLine("0 Exit");
                Console.WriteLine("Select option");


                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        //Console.WriteLine("update");
                        await UploadCSV(dbservice);
                        break;
                    case "2":
                        //Console.WriteLine();
                        await Createcompany(dbservice);
                        break;
                    case "3":
                        //Console.WriteLine();
                        await ViewAllcompanies(dbservice);
                        break;
                    case "4":
                        //Console.WriteLine("");
                        await SearchCompany(dbservice);
                        break;
                    case "5":
                        //Console.WriteLine("");
                        await DeleteCompany(dbservice);
                        break;
                    case "6":
                        Console.WriteLine();
                        break;
                    case "7":
                        Console.WriteLine("");
                        break;
                    case "0":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Choose correct");
                        break;

                }
                Console.WriteLine("press any key to continue");
                Console.ReadKey();
            }

            Console.ReadKey();
        }

        static async Task ShowStats(DbService dbservice)
        {
            var company = await dbservice.GetCompanies();
            Console.WriteLine($"Total companies: {company.Count}");
            Console.WriteLine($"cities: {company.Select(c => c.City.Distinct().Count())}");
        }

        static async Task DeleteCompany(DbService dbservice)
        {
            Console.WriteLine("Enter company ID to remove: ");

            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var sucess = await dbservice.DeleteCompany(id);
                Console.WriteLine(sucess? "Deleted company": "Not found");
            }
        }

        static async Task SearchCompany(DbService dbservice)
        {
            Console.Write("Enter company ID ");

            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var company = await dbservice.GetCompany(id);

                if (company != null)
                {
                    Console.WriteLine($"{company.Name} \n {company.OrgNr}");
                }
                else
                {
                    Console.WriteLine("Company not found");
                }
            }

        }

        static async Task ViewAllcompanies(DbService dbService)
        {
            var companies = await dbService.GetCompanies();

            foreach (var c in companies)
            {
                Console.WriteLine($"{c.ID} | {c.Name} | {c.OrgNr}");
            }
        }

        static async Task Createcompany(DbService dbservice)
        {
            Console.WriteLine("Create a Company by filling in information");
            Console.Write("Name of company: ");
            var name = Console.ReadLine();

            Console.Write("Enter org number: ");
            var number = Console.ReadLine();

            var company = new Company
            {
                Name = name,
                OrgNr = number

            };

            await dbservice.CreateCompany(company);
        }

        static async Task UploadCSV(DbService dbservice)
        {
            Console.WriteLine("enter filePath: ");
            var path = Console.ReadLine();

            if (!File.Exists(path))
            {
                Console.WriteLine("file not found");
                return;
            }

            var workBook = new XLWorkbook(path);
            var workSheet = workBook.Worksheet(1);
            var rows = workSheet.RangeUsed().RowsUsed().Skip(1);

            int count = 0;
            foreach (var row in rows)
            {
                var orgnr = row.Cell(5).GetString();

                var company = new Company
                {
                    Name = row.Cell(1).GetString(),
                    City = row.Cell(2).GetString(),
                    Industry = row.Cell(3).GetString(),
                    OrgNr = orgnr,
                    Website = row.Cell(8).GetString(),
                };

                await dbservice.CreateCompany(company);
                count++;

                if (count % 100 == 0) Console.WriteLine($"Processes {count} companies");

            }
        }

    }
}
