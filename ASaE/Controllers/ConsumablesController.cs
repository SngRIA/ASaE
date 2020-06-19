using ASaE.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebGrease;

namespace ASaE.Controllers
{
    public class ConsumablesController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public async Task<ActionResult> Index()
        {
            return View(await db.EquipmentConsumables.ToListAsync());
        }

        /// <summary>
        /// Получение графика потребления расходного материала
        /// </summary>
        /// <param name="id"> Id расходного материала </param>
        /// <returns></returns>
        public async Task<ActionResult> GetGraphConsumable(int? id)
        {
            ActionResult result = HttpNotFound("Неверный id");

            if (id != null)
                result = View(await db.EquipmentConsumables.FindAsync(id));

            return result;
        }


        /// <summary>
        /// Получение списка расходных материалов
        /// </summary>
        /// <param name="consumables"> Передаем расходники которые необходимо пометить как выбранные </param>
        /// <returns> Возвращаяет список SelectListItem </returns>
        public async Task<ActionResult> GetEquipmentConsumablesList(ICollection<EquipmentConsumable> consumables = null)
        {
            var equipmentConsumables = await db.EquipmentConsumables.ToListAsync();

            List<SelectListItem> items = new List<SelectListItem>();

            if (equipmentConsumables.Count > 0)
            {
                foreach (var consumable in equipmentConsumables)
                {
                    bool? containsItem = consumables?.Contains(consumable) ?? false;

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
        /// Возвращает список расходных материалов, аргументы являются фильтрами
        /// </summary>
        /// <param name="name"> Фильтр по имени </param>
        /// <param name="consumableStatus"> Фильтр по статусу материала ( Ok - достаточное кол-во, любое другое значение - недостаточно кол-во ) </param>
        /// <returns></returns>
        public async Task<ActionResult> GetConsumables(string name, string consumableStatus)
        {
            return PartialView(await db.EquipmentConsumables.Where(ec => ec.Name.Contains(name == string.Empty ? ec.Name : name))
                .Where(ec => consumableStatus != "Ok" ? ec.MaxValue / 2 > ec.Value + 1 : ec.MaxValue / 2 < ec.Value + 1)
                .ToListAsync());
        }

        /// <summary>
        /// Возвращает значение расхода выбранного расходного материала
        /// </summary>
        /// <param name="id"> Id расходного материала </param>
        /// <returns> Возвращаем json с данными выбранного расходного материала </returns>
        public async Task<ContentResult> GetConsumableGraph(int? id)
        {
            ContentResult result = null;

            var consumable = await db.EquipmentConsumables.FindAsync(id);
            if (consumable != null)
            {
                var modifiedValues = await db.ModifiedValueConsumables.Where(mv => mv.EquipmentConsumable.Id == consumable.Id) // Получаем расходные данные
                                                                      .ToListAsync();

                if (modifiedValues.Count <= 0) // Если данных нет, создаем псевдо данные
                {
                    for (int i = 0; i < 5; i++)
                    {
                        modifiedValues.Add(new ModifiedValueConsumable // add test data
                        {
                            Id = i + 1,
                            Date = DateTime.Today.AddDays(i + 1),
                            OldValue = 100 * i + 1,
                            NewValue = 100 * i + 1 + 222,
                            EquipmentConsumable = await db.EquipmentConsumables.FindAsync(i + 1)
                        }); ;
                    }
                }

                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


                List<DataPoint> dataPoints = new List<DataPoint>();
                foreach (var item in modifiedValues)
                {
                    dataPoints.Add(new DataPoint(item.Date.ToUniversalTime().Subtract(epoch).TotalMilliseconds, item.NewValue)); // Переобразовываем в Int, для передачи
                }

                result = Content(JsonConvert.SerializeObject(dataPoints, 
                    new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }), "application/json");
            }

            return result;
        }

        /// <summary>
        /// Подготовка странцы для отображения формы с отображением графиков потребления
        /// </summary>
        /// <returns></returns>
        public ActionResult InfoConsumable()
        {
            return View();
        }

        /// <summary>
        /// Подготовка странцы для создания нового расходного материала
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Create()
        {
            return View(new EquipmentConsumable());
        }

        /// <summary>
        /// Подтвержаение создания расходного материала
        /// </summary>
        /// <param name="consumable"> Данные расходного материала </param>
        /// <returns></returns>
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateConfirmed(EquipmentConsumable consumable)
        {
            if (ModelState.IsValid)
            {
                db.EquipmentConsumables.Add(consumable);

                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(consumable);
        }

        /// <summary>
        /// Подготовка странцы для изменение выбранного расходного материала
        /// </summary>
        /// <param name="id"> Id расходного материала </param>
        /// <returns></returns>
        public async Task<ActionResult> Edit(int? id)
        {
            ActionResult result = HttpNotFound("Неверный id");

            var consumable = await db.EquipmentConsumables.FindAsync(id);
            if (consumable != null)
            {
                result = View(consumable);
            }

            return result;
        }

        /// <summary>
        /// Подтвержаение изменения расходного материала
        /// </summary>
        /// <param name="consumable"> Данные расходного материала </param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,Value,MaxValue")] EquipmentConsumable consumable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(consumable).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details", new { id = consumable.Id });
            }
            return View(consumable);
        }

        /// <summary>
        /// Вывод данных о выбранном расходном материале
        /// </summary>
        /// <param name="id"> Id расходного материала </param>
        /// <returns></returns>
        public async Task<ActionResult> Details(int? id)
        {
            ActionResult result = HttpNotFound("Неверный id");

            var consumable = await db.EquipmentConsumables.FindAsync(id);
            if(consumable != null)
            {
                result = View(consumable);
            }    

            return result;
        }
    }
}