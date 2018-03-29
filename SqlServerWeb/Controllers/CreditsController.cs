using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SqlServerWeb.Models;
using Canducci.SqlKata.Dapper.SqlServer;
using Canducci.SqlKata.Dapper.Extensions.SoftBuilder;
using Microsoft.AspNetCore.Http;
using X.PagedList;
using System;

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
        public ActionResult Index(int? page)
        {
            page = page ?? 1;

            int total = 5;

            int count = connection
                .SoftBuild()
                .From("Credit")
                .AsCount()
                .FindOne<int>();

            IEnumerable<Credit> model = connection
                .SoftBuild()
                .From("Credit")
                .OrderBy("Description")
                .ForPage(page.Value, total)
                .List<Credit>();
            
            StaticPagedList<Credit> result =
                new StaticPagedList<Credit>(model, page.Value, total, count);

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