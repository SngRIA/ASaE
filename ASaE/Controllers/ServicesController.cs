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
using System.EnterpriseServices;
using Microsoft.AspNet.Identity;
using System.IO;

namespace ASaE.Controllers
{
    public class ServicesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            return View(await db.Services.ToListAsync());
        }

        /// <summary>
        /// Получение списка оборудования
        /// </summary>
        /// <param name="equipmentId"> Id оборудования которое будет помечено как выбранное </param>
        /// <returns></returns>
        public async Task<ActionResult> GetEquipmentsList(int? equipmentId)
        {
            var equipments = await db.Equipments.ToListAsync();

            List<SelectListItem> items = new List<SelectListItem>();

            if (equipments.Count > 0)
            {
                foreach (var equipment in equipments)
                {
                    items.Add(new SelectListItem { Text = $"{equipment.Name} ", Value = equipment.Id.ToString() });
                }

                if (equipmentId != null) items.Find(e => e.Value == equipmentId.ToString()).Selected = true; // Если id оборудования указан, делаем это оборудование выбранным
            }
            else
            {
                items.Add(new SelectListItem { Text = "Не найдено", Value = "0" });
            }

            return PartialView(items);
        }

        /// <summary>
        /// Получение списка услуг для поиска, аргументы являются фильтрами
        /// </summary>
        /// <param name="name"> Фильтр по наименованию услуги </param>
        /// <param name="priceMin"> Фильтр по минимальной цене </param>
        /// <param name="priceMax"> Фильтр по максимальной цене </param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> GetServicesList(string name = null, int priceMin = 0, int priceMax = 0) 
        {
            return PartialView(await db.Services.Where(s => s.Name.Contains(name == string.Empty ? s.Name : name))
                .Where(s => priceMin <= s.Cost && s.Cost <= priceMax)
                .ToListAsync());
        }

        /// <summary>
        /// Подготовка страницы для вывода данных о выбранной услуге
        /// </summary>
        /// <param name="id"> Id услуги </param>
        /// <returns></returns>
        public async Task<ActionResult> Details(int? id)
        {
            ActionResult result = HttpNotFound("Ошибка поиска");

            Service service = await db.Services.FindAsync(id);
            if(service != null)
            {
                var userId = User.Identity.GetUserId();
                await db.UserBaskets.SingleOrDefaultAsync(ub => ub.User.Id == userId).ContinueWith(async (t) =>
                {
                    result = View(new ServiceDetailsViewModel { Service = service, IsContainsInBasket = t.Result?.Items.FirstOrDefault(i => i.Service.Id == service.Id) != null });
                });
            } else
            {
                result = HttpNotFound("Несуществующий id");
            }

            return result;
        }

        /// <summary>
        /// Подготовка страницы для создания услуги
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View(new Service());
        }

        /// <summary>
        /// Подтверждение создания услуги
        /// </summary>
        /// <param name="service"> Данные услуги </param>
        /// <param name="uploadImage"> Файл из запроса, должен содержать картинку </param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,Cost,Type")] Service service, HttpPostedFileBase uploadImage)
        {
            if (ModelState.IsValid)
            {
                int equipmentId = int.Parse(HttpContext.Request.Form["equipmentId"]);

                await db.Equipments.FindAsync(equipmentId).ContinueWith((t) => // Поиск оборудования из запроса
                {
                    service.Equipment = t.Result;

                    using (var reader = new BinaryReader(uploadImage.InputStream))
                    {
                        service.Picture = reader.ReadBytes(uploadImage.ContentLength);
                    }

                    db.Services.Add(service);

                    db.SaveChanges();
                });

                return RedirectToAction("Index");
            }

            return View(service);
        }

        /// <summary>
        /// Подготовка страницы дял изменения услуги
        /// </summary>
        /// <param name="id"> Id услуги </param>
        /// <returns></returns>
        public async Task<ActionResult> Edit(int? id)
        {
            ActionResult result = HttpNotFound("Неверный id");

            Service service = await db.Services.FindAsync(id);
            if (service != null)
            {
                result = View(service);
            }

            return result;
        }

        /// <summary>
        /// Подтверждение изменение услуги
        /// </summary>
        /// <param name="service"> Данные услуги </param>
        /// <param name="uploadImage"> Файл из запроса, должен содержать картинку </param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,Cost,Type")] Service service, HttpPostedFileBase uploadImage)
        {
            if (ModelState.IsValid)
            {
                int equipmentId = int.Parse(HttpContext.Request.Form["equipmentId"]); // Получаем id оборудования из запроса

                await db.Equipments.FindAsync(equipmentId).ContinueWith(async (t) => // Поиск оборудования из запроса
                {
                    var serviceOriginal = db.Services.Find(service.Id); // Поиск услуги для обновления данных
                    var dbEntity = db.Entry(serviceOriginal);

                    serviceOriginal.Name = service.Name;
                    serviceOriginal.Description = service.Description;
                    serviceOriginal.Cost = service.Cost;
                    serviceOriginal.Type = service.Type;

                    if (uploadImage != null) // Обновление изображения услуги, при необходимости
                    {
                        using (var reader = new BinaryReader(uploadImage.InputStream))
                        {
                            serviceOriginal.Picture = reader.ReadBytes(uploadImage.ContentLength);
                        }
                    } 
                    else
                    {
                        dbEntity.Property(s => s.Picture).IsModified = false;
                    }

                    serviceOriginal.Equipment = t.Result;

                    dbEntity.State = EntityState.Modified;

                    await db.SaveChangesAsync();
                });

                return RedirectToAction("Index");
            }
            return View(service);
        }

        /// <summary>
        /// Подготовка страницы удаления услуги
        /// </summary>
        /// <param name="id"> Id услуги </param>
        /// <returns></returns>
        public async Task<ActionResult> Delete(int? id)
        {
            ActionResult result = HttpNotFound("Неверный id");

            Service service = await db.Services.FindAsync(id);
            if (service != null)
            {
                result = View(service);
            }

            return result;
        }

        /// <summary>
        /// Подтверждение удаление услуги
        /// </summary>
        /// <param name="id"> Id услуги </param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Service service = await db.Services.FindAsync(id);

            db.Services.Remove(service);

            await db.SaveChangesAsync();

            return RedirectToAction("Index");
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
