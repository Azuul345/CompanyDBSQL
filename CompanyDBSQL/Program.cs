using CompanyDBSQL.Data;
using CompanyDBSQL.Models;

namespace CompanyDBSQL
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            var context = new AppDbContext();

            var dbService = new DbService(context);


            var company = new Company
            {
                OrgNr = "154785",
                Name = "Test"
            };

            await dbService.CreateCompany(company);

            var allcompanies = await dbService.GetCompanies();

            foreach (var c in allcompanies)
            {
                Console.WriteLine($"{c.ID} {c.Name} {c.OrgNr}");
                await dbService.DeleteCompany(c.ID);
            }

            Console.ReadKey();
        }
    }
}
