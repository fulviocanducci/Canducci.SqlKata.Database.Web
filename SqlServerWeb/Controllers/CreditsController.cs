using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SqlServerWeb.Models;
using Canducci.SqlKata.Dapper.SqlServer;
using Canducci.SqlKata.Dapper.Extensions.SoftBuilder;
using Microsoft.AspNetCore.Http;
using Canducci.Pagination;
using System;
using Canducci.SqlKata.Dapper;
using System.Threading.Tasks;

namespace SqlServerWeb.Controllers
{
    public class CreditsController : Controller
    {
        private IDbConnection connection;
        public CreditsController(IDbConnection connection)
        {
            this.connection = connection;
        }
        // GET: Credits
        public async Task<ActionResult> Index(int? page)
        {
            page = page ?? 1;
            int items = 5;

            QueryBuilderMultiple queries = connection.SoftBuild().QueryBuilderMultipleCollection();

            var results = queries
                .AddQuery(x => x.From("credit").AsCount())
                .AddQuery(x => x.From("credit").OrderBy("description").ForPage(page.Value, items))
                .Results();

            int count = results.ReadFirst<int>();
            IEnumerable<Credit> model = await results.ReadAsync<Credit>();
            StaticPaginated<Credit> result = new StaticPaginated<Credit>(model, page.Value, items, count);

            return View(result);
        }

        // GET: Credits/Details/5
        public ActionResult Details(int id)
        {
            return View(connection.SoftBuild().From("Credit").Where("Id", id).FindOne<Credit>());
        }

        // GET: Credits/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Credits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Credit credit)
        {
            try
            {
                var id = connection.SoftBuild()
                    .From("Credit")
                    .AsInsert(new Dictionary<string, object>
                    {
                        ["Description"] = credit.Description,
                        ["Created"] = credit.Created
                    })
                    .SaveInsert<int>();

                return RedirectToAction(nameof(Edit), new { id = id });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: Credits/Edit/5
        public ActionResult Edit(int id)
        {
            return View(connection.SoftBuild().From("Credit").Where("Id", id).FindOne<Credit>());
        }

        // POST: Credits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Credit credit)
        {
            try
            {
                connection.SoftBuild()
                    .From("Credit")
                    .Where("Id", credit.Id)
                    .AsUpdate(new Dictionary<string, object>
                    {
                        ["Description"] = credit.Description,
                        ["Created"] = credit.Created
                    })
                    .SaveUpdate();

                return RedirectToAction(nameof(Edit), new { id = credit.Id });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: Credits/Delete/5
        public ActionResult Delete(int id)
        {
            return View(connection.SoftBuild().From("Credit").Where("Id", id).FindOne<Credit>());
        }

        // POST: Credits/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                connection.SoftBuild()
                    .From("Credit")
                    .Where("id", id)
                    .AsDelete()
                    .SaveUpdate();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}