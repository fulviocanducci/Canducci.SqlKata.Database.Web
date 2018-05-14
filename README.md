# Canducci.SqlKata.Database.Web

[![Version](https://img.shields.io/nuget/v/Canducci.SqlKata.Dapper.svg?style=plastic&label=version)](https://www.nuget.org/packages/Canducci.SqlKata.Dapper/)
[![NuGet](https://img.shields.io/nuget/dt/Canducci.SqlKata.Dapper.svg)](https://www.nuget.org/packages/Canducci.SqlKata.Dapper/)
[![Build Status](https://travis-ci.org/fulviocanducci/Canducci.SqlKata.Dapper.svg?branch=master)](https://travis-ci.org/fulviocanducci/Canducci.SqlKata.Dapper)

## Example

### Packages:

```
- Canducci.Pagination.Mvc
- Canducci.SqlKata.Dapper.Postgres
- Npgsql
```

### Model

```csharp
public class Credit
{
    public Credit() { }

    public Credit(string description, DateTime? created = null)
    {
        Description = description;
        Created = created;
    }

    public Credit(int id, string description, DateTime? created = null)
    {
        Id = id;
        Description = description;
        Created = created;
    }

    public int Id { get; set; }

    [Required(ErrorMessage = "Digite a descrição")]
    [MinLength(3, ErrorMessage = "Digite com no minimo 3 letras")]
    public string Description { get; set; }

    [DataType(DataType.Date, ErrorMessage = "Data inválida")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
    public DateTime? Created { get; set; }
}
```

### Configuration Startup

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddScoped<IDbConnection>(_ => 
		new NpgsqlConnection(Configuration.GetConnectionString("PostgresDatabaseConnectionString")));            
    services.AddMvc();
}
```

### Controller

```csharp
using System;
using System.Data;
using PostgresWeb.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Canducci.SqlKata.Dapper.Postgres;
using Canducci.SqlKata.Dapper.Extensions.SoftBuilder;
using System.Threading.Tasks;
using Canducci.Pagination;
using Canducci.SqlKata.Dapper;
namespace PostgresWeb.Controllers
{
    public class CreditsController : Controller
    {
        private IDbConnection connection;

        public CreditsController(IDbConnection connection)
        {
            this.connection = connection;
        }
        
        public async Task<ActionResult> Index(int? current)
        {
            current = current ?? 1;
            int items = 5;

            QueryBuilderMultiple queries = connection.SoftBuild().QueryBuilderMultipleCollection();

            var results = queries
                .AddQuery(x => x.From("credit").AsCount())
                .AddQuery(x => x.From("credit").OrderBy("description").ForPage(current.Value, items))
                .Results();

            int count = results.ReadFirst<int>();
            IEnumerable<Credit> model = await results.ReadAsync<Credit>();
            StaticPaginated<Credit> result = new StaticPaginated<Credit>(model, current.Value, items, count);

            return View(result);
        }
                
        public async Task<ActionResult> Details(int id)
        {
            var model = await connection
                .SoftBuild()
                .From("credit")
                .Where("id", id)
                .FindOneAsync<Credit>();

            return View(model);
        }
                
        public ActionResult Create()
        {
            return View();
        }
                
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Credit credit)
        {
            try
            {
                //Dictionary<string, object> values = credit;

                var id = await connection.SoftBuild()
                    .From("credit")
                    .AsInsert(new Dictionary<string, object>
                    {
                        ["description"] = credit.Description,
                        ["created"] = credit.Created
                    })                    
                    .SaveInsertAsync<int>();

                return RedirectToAction(nameof(Edit), new { id = id });
            }
            catch (Exception ex)
            {
                return View();
            }
        }
                
        public async Task<ActionResult> Edit(int id)
        {
            var model = await connection
                .SoftBuild()
                .From("credit")
                .Where("id", id)
                .FindOneAsync<Credit>();

            return View(model);
        }
                
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Credit credit)
        {
            try
            {
                await connection.SoftBuild()
                    .From("credit")
                    .Where("id", credit.Id)
                    .AsUpdate(new Dictionary<string, object>
                    {
                        ["description"] = credit.Description,
                        ["created"] = credit.Created
                    })
                    .SaveUpdateAsync();

                return RedirectToAction(nameof(Edit), new { id = credit.Id });
            }
            catch
            {
                return View();
            }
        }
                
        public async Task<ActionResult> Delete(int id)
        {
            var model = await connection
                .SoftBuild()
                .From("credit")
                .Where("id", id)
                .FindOneAsync<Credit>();

            return View(model);
        }
                
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Credit credit)
        {
            try
            {                
                await connection.SoftBuild()
                    .From("credit")
                    .Where("id", id)
                    .AsDelete()
                    .SaveUpdateAsync();                

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
```
