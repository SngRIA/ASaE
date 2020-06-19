using ASaE.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ASaE.Controllers
{
    [Authorize]
    public class EquipmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public async Task<ActionResult> Index()
        {
            return View(await db.Equipments.ToListAsync());
        }

        /// <summary>
        /// Получение списка расходников
        /// </summary>
        /// <param name="consumables"> Передаем расходники которые необходимо пометить как выбранные </param>
        /// <returns></returns>
        public async Task<ActionResult> GetEquipmentConsumablesList(IEnumerable<EquipmentConsumable> consumables)
        {
            var equipmentConsumables = await db.EquipmentConsumables.ToListAsync();

            List<SelectListItem> items = new List<SelectListItem>();

            if (equipmentConsumables.Count > 0)
            {
                foreach (var consumable in equipmentConsumables)
                {
                    bool? containsItem = consumables?.Contains(consumable);
                    items.Add(new SelectListItem { Text = consumable.Name, Value = consumable.Id.ToString(), Selected = containsItem.Value });
                }
            }
            else
            {
                items.Add(new SelectListItem { Text = "Не найдено", Value = "0" });
            }

            return PartialView(items);
        }

        /// <summary>
        /// Получение списка оборудования
        /// </summary>
        /// <param name="employeeSelectedId"> Id выбранного оборудования </param>
        /// <returns></returns>
        public async Task<ActionResult> GetEmployeesList(int? employeeSelectedId)
        {
            var employees = await db.Employees.ToListAsync();

            List<SelectListItem> items = new List<SelectListItem>();

            if (employees.Count > 0)
            {
                foreach (var employee in employees)
                {
                    items.Add(new SelectListItem { Text = $"{employee.User.FirstName} {employee.User.LastName} {employee.User.MiddleName}", Value = employee.Id.ToString() });
                }

                if (employeeSelectedId != null) items.Find(e => e.Value == employeeSelectedId.ToString()).Selected = true; // Если id оборудования указан, делаем это оборудование выбранным
            }
            else
            {
                items.Add(new SelectListItem { Text = "Не найдено", Value = "0" });
            }

            return PartialView(items);
        }

        /// <summary>
        /// Получение списка оборудования, аргументы являются фильтрами
        /// </summary>
        /// <param name="name"> Фильтр по наименовании оборудования </param>
        /// <param name="consumableName"> Фильтр по наименовании расходного материала </param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> GetEquipmentsList(string name = "", string consumableName = "")
        {
            var query = await db.Equipments.Where(e => e.Name.Contains(name == string.Empty ? e.Name : name))
                .Where(e => e.EquipmentConsumables.Where(ec => ec.Name.Contains(consumableName == string.Empty ? ec.Name : consumableName)) != null)
                .ToListAsync();
            return PartialView(query);
        }

        /// <summary>
        /// Вывод данных о выбранном оборудовании
        /// </summary>
        /// <param name="id"> Id оборудования </param>
        /// <returns></returns>
        public async Task<ActionResult> Details(int? id)
        {
            ActionResult result = HttpNotFound("Неверный id");

            Equipment equipment = await db.Equipments.FindAsync(id);
            if (equipment != null)
            {
                result = View(equipment);
            }

            return result;
        }

        /// <summary>
        /// Подготовка страницы для создания нового оборудования
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View(new Equipment());
        }

        /// <summary>
        /// Создание нового оборудования
        /// </summary>
        /// <param name="equipment"> Данные оборудования </param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,Type")] Equipment equipment)
        {
            ActionResult result = RedirectToAction("Index");
            if (ModelState.IsValid)
            {
                int employeeId = int.Parse(HttpContext.Request.Form["employeeId"]);

                await db.Employees.FindAsync(employeeId).ContinueWith( async (t) => // Поиск оборудования из запроса
                {
                    var employee = t.Result;
                    if (employee != null)
                    {
                        equipment.EquipmentManager = employee.User;

                        db.Equipments.Add(equipment);

                        await db.SaveChangesAsync();
                    } else
                    {
                        result = View(equipment);
                    }
                });

                result = RedirectToAction("Index");
            }

            return result;
        }

        /// <summary>
        /// Подготовка страницы для изменения оборудования
        /// </summary>
        /// <param name="id"> Id оборудования </param>
        /// <returns></returns>
        public async Task<ActionResult> Edit(int? id)
        {
            ActionResult result = HttpNotFound("Неверный id");

            Equipment equipment = await db.Equipments.FindAsync(id);
            if (equipment != null)
            {
                result = View(equipment);
            }

            return result;
        }

        /// <summary>
        /// Создание оборудования
        /// </summary>
        /// <param name="equipment"> Данные оборудования </param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,Cost,Type")] Equipment equipment)
        {
            ActionResult result = RedirectToAction("Index");
            if (ModelState.IsValid)
            {
                var consumableItemsId = Request.Form["consumableItems"]?.Split(',').Select(ci => ci); // Получаем данные выбранных расходных материалов

                if (consumableItemsId != null)
                {
                    var consumables = db.EquipmentConsumables.Where(ec => consumableItemsId.Contains(ec.Id.ToString())); // Формирования массива содержащий связанные с оборудованием расходники

                    await db.Equipments.FindAsync(equipment.Id).ContinueWith(async (t) => // Поиск оборудования
                    {
                        var equipmentResult = t.Result;

                        equipmentResult.Name = equipment.Name;
                        equipmentResult.Description = equipment.Description;
                        equipmentResult.Type = equipment.Type;

                        equipmentResult.EquipmentConsumables.Clear();

                        equipmentResult.EquipmentConsumables = consumables.ToList();

                        await db.SaveChangesAsync();
                    });

                } else
                {
                    db.Entry(equipment).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
            }
            else
            {
                result = View(equipment);
            }

            return result;
        }


        /// <summary>
        /// Подготовка страницы для удаления оборудования
        /// </summary>
        /// <param name="id"> Id оборудования </param>
        /// <returns></returns>
        public async Task<ActionResult> Delete(int? id)
        {
            ActionResult result = HttpNotFound("Неверный id");

            Equipment equipment = await db.Equipments.FindAsync(id);
            if (equipment != null)
            {
                result = View(equipment);
            }

            return result;
        }

        /// <summary>
        /// Удаление оборудования
        /// </summary>
        /// <param name="id"> Id оборудования </param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Equipment equipment = await db.Equipments.FindAsync(id);
            db.Equipments.Remove(equipment);

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