using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SqlServerWeb.Models;
using Canducci.SqlKata.Dapper.SqlServer;
using Canducci.SqlKata.Dapper.Extensions.SoftBuilder;
using SqlKata;
using SqlKata.Compilers;

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
        public ActionResult Index()
        {
            return View(connection.SoftBuild().From("People").OrderBy("Name").List<People>());
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
                //Query q = new Query("People")
                //    .Insert(new Dictionary<string, object>
                //    {
                //        ["Name"] = people.Name,
                //        ["Created"] = null,
                //        ["Active"] = people.Active
                //    });

                //SqlServerCompiler s = new SqlServerCompiler();
                //var res = s.Compile(q);

                var id = connection.SoftBuild()
                    .From("People")
                    .Insert(new Dictionary<string, object>
                    {
                        ["Name"] = people.Name, 
                        ["Created"] = people.Created ?? default(DateTime?),
                        ["Active"] = people.Active
                    })
                    .SaveInsertGetByIdInserted<int>();

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
                    .Update(new Dictionary<string, object>
                    {
                        ["Name"] = people.Name,
                        ["Created"] = people.Created,
                        ["Active"] = people.Active
                    })
                    .SaveUpdate();

                return RedirectToAction(nameof(Index));
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