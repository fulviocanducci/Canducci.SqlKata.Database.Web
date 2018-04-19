using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SqlServerWeb.Models;
using Canducci.SqlKata.Dapper.SqlServer;
using Canducci.SqlKata.Dapper.Extensions.SoftBuilder;
using Canducci.Pagination;
using Canducci.SqlKata.Dapper;
using System.Threading.Tasks;

namespace SqlServerWeb.Controllers
{
    public class PeoplesController : Controller
    {
        private IDbConnection connection;
        public PeoplesController(IDbConnection connection)
        {
            this.connection = connection;
        }
        // GET: Peoples
        public async Task<ActionResult> Index(int? current)
        {
            current = current ?? 1;
            int items = 5;

            QueryBuilderMultiple queries = connection.SoftBuild().QueryBuilderMultipleCollection();

            var results = queries
                .AddQuery(x => x.From("People").AsCount())
                .AddQuery(x => x.From("People").OrderBy("Name").ForPage(current.Value, items))
                .Results();

            int count = results.ReadFirst<int>();
            IEnumerable<People> model = await results.ReadAsync<People>();
            StaticPaginated<People> result = new StaticPaginated<People>(model, current.Value, items, count);

            return View(result);
        }

        // GET: Peoples/Details/5
        public ActionResult Details(int id)
        {
            return View(connection.SoftBuild().From("People").Where("Id", id).FindOne<People>());
        }

        // GET: Peoples/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Peoples/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(People people)
        {
            try
            {
                var data = new Dictionary<string, object>
                {
                    ["Name"] = people.Name,
                    ["Created"] = people.Created,
                    ["Active"] = people.Active
                };
                var ins = connection.SoftBuild()
                    .From("People")
                    .AsInsert(data);
               var id = ins.SaveInsert<int>();

                return RedirectToAction(nameof(Edit), new { id = id });
            }
            catch (Exception ex)
            {
                throw ex;                
            }
        }

        // GET: Peoples/Edit/5
        public ActionResult Edit(int id)
        {
            return View(connection.SoftBuild().From("People").Where("Id", id).FindOne<People>());
        }

        // POST: Peoples/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(People people)
        {
            try
            {
                connection.SoftBuild()
                    .From("People")
                    .Where("Id", people.Id)
                    .AsUpdate(new Dictionary<string, object>
                    {
                        ["Name"] = people.Name,
                        ["Created"] = people.Created,
                        ["Active"] = people.Active
                    })
                    .SaveUpdate();

                return RedirectToAction(nameof(Edit), new { Id = people.Id });
            }
            catch
            {
                return View();
            }
        }

        // GET: Peoples/Delete/5
        public ActionResult Delete(int id)
        {
            return View(connection.SoftBuild().From("People").Where("Id", id).FindOne<People>());
        }

        // POST: Peoples/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                connection.SoftBuild()
                    .From("People")
                    .Where("Id", id)
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