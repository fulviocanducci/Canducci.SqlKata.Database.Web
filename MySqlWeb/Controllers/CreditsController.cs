using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Canducci.SqlKata.Dapper.MySql;
using Canducci.SqlKata.Dapper.Extensions.SoftBuilder;
using X.PagedList;
using MySqlWeb.Models;
using System.Threading.Tasks;

namespace MySqlWeb.Controllers
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

            int count = await connection
                .SoftBuild()
                .From("credit")
                .Count()
                .UniqueResultToIntAsync();

            IEnumerable<Credit> result = await connection
                .SoftBuild()
                .From("credit")
                .OrderBy("description")
                .ForPage(page.Value, items)
                .ListAsync<Credit>();

            StaticPagedList<Credit> model = 
                new StaticPagedList<Credit>(result, page.Value, items, count);

            return View(model);
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
                var id = await connection
                    .SoftBuild()
                    .From("credit")
                    .Insert(new Dictionary<string, object>
                    {
                        ["description"] = credit.Description,
                        ["created"] = credit.Created
                    })
                    .SaveInsertAsync<int>();

                return RedirectToAction(nameof(Edit), new { id = id });
            }
            catch
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
                await connection
                    .SoftBuild()
                    .From("credit")
                    .Where("id", credit.Id)
                    .Update(new Dictionary<string, object>
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
               await connection
                    .SoftBuild()
                    .From("credit")
                    .Where("id", id)
                    .Delete()
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