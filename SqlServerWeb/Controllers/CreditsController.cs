﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SqlServerWeb.Models;
using Canducci.SqlKata.Dapper.SqlServer;
using Canducci.SqlKata.Dapper.Extensions.SoftBuilder;
using Microsoft.AspNetCore.Http;
using X.PagedList;
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
                .Count()
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
            return View(connection.SoftBuild().From("credit").Where("id", id).FindOne<Credit>());
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
                    .From("credit")
                    .Insert(new Dictionary<string, object>
                    {
                        ["description"] = credit.Description
                    })
                    .SaveInsertGetByIdInserted<int>();

                return RedirectToAction(nameof(Edit), new { id = id });
            }
            catch
            {
                return View();
            }
        }

        // GET: Credits/Edit/5
        public ActionResult Edit(int id)
        {
            return View(connection.SoftBuild().From("credit").Where("id", id).FindOne<Credit>());
        }

        // POST: Credits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Credit credit)
        {
            try
            {
                connection.SoftBuild()
                    .From("credit")
                    .Where("id", credit.Id)
                    .Update(new Dictionary<string, object>
                    {
                        ["description"] = credit.Description
                    })
                    .SaveUpdate();

                return RedirectToAction(nameof(Edit), new { id = credit.Id });
            }
            catch
            {
                return View();
            }
        }

        // GET: Credits/Delete/5
        public ActionResult Delete(int id)
        {
            return View(connection.SoftBuild().From("credit").Where("id", id).FindOne<Credit>());
        }

        // POST: Credits/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                connection.SoftBuild()
                    .From("credit")
                    .Where("id", id)
                    .Delete()
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