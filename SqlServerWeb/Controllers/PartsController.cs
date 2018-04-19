using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SqlServerWeb.Models;
using Canducci.SqlKata.Dapper.SqlServer;
using Canducci.SqlKata.Dapper.Extensions.SoftBuilder;
using System;
using System.Collections.Generic;
using Canducci.SqlKata.Dapper;
using System.Threading.Tasks;
using Canducci.Pagination;
namespace SqlServerWeb.Controllers
{
    public class PartsController : Controller
    {
        private IDbConnection connection;
        public PartsController(IDbConnection connection)
        {
            this.connection = connection;
        }

        // GET: Parts
        public async Task<ActionResult> Index(int? page)
        {
            page = page ?? 1;
            int items = 5;

            QueryBuilderMultiple queries = connection.SoftBuild().QueryBuilderMultipleCollection();

            var results = queries
                .AddQuery(x => x.From("Part").AsCount())
                .AddQuery(x => x.From("Part").OrderBy("Description").ForPage(page.Value, items))
                .Results();

            int count = results.ReadFirst<int>();
            IEnumerable<Part> model = await results.ReadAsync<Part>();
            StaticPaginated<Part> result = new StaticPaginated<Part>(model, page.Value, items, count);

            return View(result);
        }

        // GET: Parts/Details/5
        public ActionResult Details(string id)
        {
            return View(connection.SoftBuild().From("Part").Where("Id", Guid.Parse(id)).FindOne<Part>());
        }

        // GET: Parts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Parts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Part part)
        {
            try
            {
                var id = connection.SoftBuild()
                    .From("Part")
                    .AsInsert(new Dictionary<string, object>
                    {
                        ["Description"] = part.Description
                    })
                    .SaveInsert<Guid>();

                return RedirectToAction(nameof(Edit), new { id });
            }
            catch
            {
                return View();
            }
        }

        // GET: Parts/Edit/5
        public ActionResult Edit(string id)
        {
            return View(connection.SoftBuild().From("Part").Where("Id", Guid.Parse(id)).FindOne<Part>());
        }

        // POST: Parts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, Part part)
        {
            try
            {
                connection.SoftBuild()
                    .From("Part")
                    .Where("Id", part.Id)
                    .AsUpdate(new Dictionary<string, object>
                    {
                        ["Description"] = part.Description
                    })
                    .SaveUpdate();

                return RedirectToAction(nameof(Edit), new { id = part.Id });
            }
            catch
            {
                return View();
            }
        }

        // GET: Parts/Delete/5
        public ActionResult Delete(string id)
        {
            return View(connection.SoftBuild().From("Part").Where("Id", Guid.Parse(id)).FindOne<Part>()); 
        }

        // POST: Parts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, IFormCollection collection)
        {
            try
            {
                connection.SoftBuild()
                    .From("Part")
                    .Where("Id", Guid.Parse(id))
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