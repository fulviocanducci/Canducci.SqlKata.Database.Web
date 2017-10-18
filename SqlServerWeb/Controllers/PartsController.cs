using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SqlServerWeb.Models;
using Canducci.SqlKata.Dapper.SqlServer;
using Canducci.SqlKata.Dapper.Extensions.SoftBuilder;
using System;
using System.Collections.Generic;

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
        public ActionResult Index()
        {
            return View(connection.SoftBuild().From("part").List<Part>());
        }

        // GET: Parts/Details/5
        public ActionResult Details(string id)
        {
            return View(connection.SoftBuild().From("part").Where("id", Guid.Parse(id)).FindOne<Part>());
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
                    .From("part")
                    .Insert(new Dictionary<string, object>
                    {
                        ["description"] = part.Description
                    })
                    .SaveInsertGetByIdInserted<Guid>();

                return RedirectToAction(nameof(Edit), new { id = id });
            }
            catch
            {
                return View();
            }
        }

        // GET: Parts/Edit/5
        public ActionResult Edit(string id)
        {
            return View(connection.SoftBuild().From("part").Where("id", Guid.Parse(id)).FindOne<Part>());
        }

        // POST: Parts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, Part part)
        {
            try
            {
                connection.SoftBuild()
                    .From("part")
                    .Where("id", part.Id)
                    .Update(new Dictionary<string, object>
                    {
                        ["description"] = part.Description
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
            return View(connection.SoftBuild().From("part").Where("id", Guid.Parse(id)).FindOne<Part>());
        }

        // POST: Parts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, IFormCollection collection)
        {
            try
            {
                connection.SoftBuild()
                    .From("part")
                    .Where("id", Guid.Parse(id))
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