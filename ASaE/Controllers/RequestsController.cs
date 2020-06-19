using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASaE.Models;
using Microsoft.AspNet.Identity;

namespace ASaE.Controllers
{
    public class RequestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public async Task<ActionResult> Index()
        {
            return View(await db.Requests.Where(r => r.Status == RequestStatus.Waiting)
                .ToListAsync());
        }

        /// <summary>
        /// Создание запроса на предоставление услуг
        /// </summary>
        /// <param name="basketConfirm"> Данные запросы </param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> NewRequest(BasketConfirmViewModel basketConfirm)
        {
            await db.Users.SingleOrDefaultAsync(u => u.Id == basketConfirm.UserBasket.User.Id).ContinueWith(async (t) =>
            {
                var user = t.Result;
                var items = new List<RequestItem>();

                foreach (var item in user.UserBasket.Items)
                {
                    items.Add(new RequestItem
                    {
                        File = item.File,
                        Count = item.Count,
                        Service = item.Service
                    });
                }

                var request = db.Requests.Add(new ASaE.Models.Request
                {
                    Client = user,
                    Items = items,
                    Status = RequestStatus.Waiting,
                    UserComment = basketConfirm.UserComment,
                    Date = DateTime.Now
                });

                await db.SaveChangesAsync().ContinueWith(async (tt) =>
                {
                    user.UserBasket.Items.Clear();
                    await db.SaveChangesAsync();
                });
            });
            return RedirectToAction("Index", "Manage");
        }

        /// <summary>
        /// Подтвержение запроса 
        /// </summary>
        /// <param name="id"> Id запроса </param>
        /// <param name="employeeId"> Id работника который должен выполнить заказ </param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmRequest(int id, string employeeId)
        {
            ActionResult result = RedirectToAction("Index");

            if (employeeId != string.Empty)
            {
                await db.Requests.FindAsync(id).ContinueWith(async (t) =>
                {
                   var request = t.Result;

                   request.Status = RequestStatus.Ready;

                   var contract = db.Contracts.Add(new Contract
                   {
                       Client = request.Client,
                       Employee = db.Users.SingleOrDefault(e => e.Id == employeeId),
                       Status = ContractStatus.Waiting,
                       UserComment = request.UserComment,
                       Date = DateTime.Now
                   });

                   foreach (var item in request.Items)
                   {
                       contract.Items.Add(new ContractItem
                       {
                           File = item.File,
                           Count = item.Count,
                           Service = item.Service
                       });
                   }

                    await db.SaveChangesAsync();
                });
            } else
            {
                result = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return result;
        }

        /// <summary>
        /// Получение списка работников, аргументы являются фильтрами
        /// </summary>
        /// <param name="name"> Фильтр по имени работника </param>
        /// <returns></returns>
        public async Task<ActionResult> GetEmployeesList(string name = "")
        {
            var employees = await db.Employees.Where(e => e.User.FirstName.Contains(name == string.Empty ? e.User.FirstName : name))
                                          .ToListAsync();

            List<SelectListItem> items = new List<SelectListItem>();

            if (employees.Count > 0)
            {
                foreach (var employee in employees)
                {
                    items.Add(new SelectListItem { Text = $"{employee.User.LastName} {employee.User.FirstName} {employee.User.MiddleName}", Value = employee.User.Id });
                }
            } else
            {
                items.Add(new SelectListItem { Text = "Не найдено", Value = "0" });
            }

            return PartialView(items);
        }

        /// <summary>
        /// Получение списка запросов, аргументы являются фильтрами
        /// </summary>
        /// <param name="id"> Фильтр по Id </param>
        /// <param name="userName"> Фильтр по запросившему пользователю </param>
        /// <param name="requestStatus"> Фильтр по статусу запроса </param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> GetRequestsList(string id, string userName, string requestStatus)
        {
            return PartialView(await db.Requests.Where(r => r.Id.ToString() == (id == string.Empty ? r.Id.ToString() : id))
                .Where(r => r.Client.FirstName.Contains(userName == string.Empty ? r.Client.FirstName : userName))
                .Where(r => r.Status.ToString() == (requestStatus == string.Empty ? r.Status.ToString() : requestStatus))
                .ToListAsync());
        }

        /// <summary>
        /// Вывод данных о выбранном запросе
        /// </summary>
        /// <param name="id"> Id запроса </param>
        /// <returns></returns>
        public async Task<ActionResult> Details(int? id)
        {
            ActionResult result = HttpNotFound("Неверный id");

            Request request = await db.Requests.FindAsync(id);
            if (request != null)
            {
                result = View(request);
            }

            return result;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
