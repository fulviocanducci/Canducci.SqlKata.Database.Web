﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SqlServerWeb.Models;
using Canducci.SqlKata.Dapper.SqlServer;
using Canducci.SqlKata.Dapper.Extensions.SoftBuilder;
using System;
using System.Collections.Generic;
using X.PagedList;

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
        public ActionResult Index(int? page)
        {
            page = page ?? 1;

            int total = 5;

            int count = connection
                .SoftBuild()
                .From("Part")
                .AsCount()
                .FindOne<int>();

            IEnumerable<Part> model = connection
                .SoftBuild()
                .From("Part")
                .OrderBy("Description")
                .ForPage(page.Value, total)
                .List<Part>();

            StaticPagedList<Part> result =
                new StaticPagedList<Part>(model, page.Value, total, count);

            return View(result);
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
                    .AsInsert(new Dictionary<string, object>
                    {
                        ["description"] = part.Description
                    })
                    .SaveInsert<Guid>();

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
                    .AsUpdate(new Dictionary<string, object>
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