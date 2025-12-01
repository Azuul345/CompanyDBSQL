
using CompanyDBSQL.Data;
using CompanyDBSQL.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //added connection to appdbcontext
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConection")));

            builder.Services.AddScoped<DbService>();

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            //end points
            //get all companies
            app.MapGet("/api/companies", async (DbService dbservice) =>
            {
                var companies = await dbservice.GetCompanies();
                return Results.Ok(companies);
            });

            app.MapGet("/api/companies/{id}", async (int id, DbService dbservice) =>
            {
                var company = await dbservice.GetCompany(id);
                if (company == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(company);
            });

            //create
            app.MapPost("/api/companies", async (Company company, DbService db) =>
            {
                var newCompany = await db.CreateCompany(company);
                return Results.Ok(newCompany);
            });

            //update
            app.MapPut("api/companies/{id}", async (int id, Company company, DbService db) =>
            {
                if (id != company.ID)
                {
                    return Results.BadRequest();
                }
                var updatedCompany = await db.UpdateCompany(company);
                return Results.Ok(updatedCompany);
            });

            //delete
            app.MapDelete("/api/companies/{id}", async (int id, DbService db) =>
            {
                var deltedCompanie = await db.DeleteCompany(id);
                return deltedCompanie ? Results.NoContent() : Results.NotFound();
            });

            app.Run();
        }
    }
}
