using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Canducci.SqlKata.Dapper.Postgres;
using PostgresWeb.Models;
using Canducci.SqlKata.Dapper.Extensions.SoftBuilder;
using System.Threading.Tasks;
using System;
using Canducci.SqlKata.Dapper;
using Canducci.Pagination;
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