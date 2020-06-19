using ASaE.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ASaE.Controllers
{
    public class ContractsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public async Task<ActionResult> Index()
        {
            var employeeId = User.Identity.GetUserId();
            return View(await db.Contracts.Where(c => c.Client.Id == employeeId).ToListAsync()); //Получение только работникам
        }

        /// <summary>
        /// Подтвержаение договора
        /// </summary>
        /// <param name="id"> Id договора </param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmContract(int? id)
        {
            ActionResult result = RedirectToAction("Index");

            if(id != null)
            {
                await db.Contracts.FindAsync(id).ContinueWith(async (t) =>
                {
                    var request = t.Result;
                    if (request != null)
                    {
                        var formRequest = HttpContext.Request.Form;

                        var consumableIdList = formRequest.AllKeys.Where(k => k.StartsWith("consumable")) // Берем только расходные материалы
                                                        .Select(k => k.Replace("consumable-", string.Empty)); // Оставляем только id

                        var consumableWithValueList = consumableIdList.Select(k => new { consumableId = int.Parse(k), value = int.Parse(formRequest["consumable-" + k]) });

                        foreach (var item in consumableWithValueList)
                        {
                            EquipmentConsumable consumable = await db.EquipmentConsumables.FindAsync(item.consumableId);
                            db.ModifiedValueConsumables.Add(new ModifiedValueConsumable
                            {
                                Date = DateTime.Now,
                                EquipmentConsumable = consumable,
                                OldValue = consumable.Value,
                                NewValue = consumable.Value - consumableWithValueList.FirstOrDefault(cv => cv.consumableId == consumable.Id).value // Отнимаем потраченное кол-во материала
                            });
                            consumable.Value -= item.value; // Отнимаем потраченное кол-во материала у расходного материала
                        }

                        request.Status = ContractStatus.Ready;

                        await db.SaveChangesAsync();
                    }
                });
            }
            else
            {
                result = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return result;
        }

        /// <summary>
        /// Возвращает список договоров, аргументы являются фильтрами
        /// </summary>
        /// <param name="id"> Фильтр по id </param>
        /// <param name="userName"> Фильтр по имени </param>
        /// <param name="contractStatus"> Фильтр по статусу </param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> GetContractsList(string id, string userName, string contractStatus)
        {
            return PartialView(await db.Contracts.Where(c => c.Id.ToString() == (id == string.Empty ? c.Id.ToString() : id))
                .Where(c => c.Client.FirstName.Contains(userName == string.Empty ? c.Client.FirstName : userName))
                .Where(c => c.Status.ToString() == (contractStatus == string.Empty ? c.Status.ToString() : contractStatus))
                .ToListAsync());
        }

        /// <summary>
        /// Получаем файл из договора
        /// </summary>
        /// <param name="id"> Id контракта </param>
        /// <param name="serviceId"> Id услуги </param>
        /// <returns></returns>
        public async Task<FileResult> GetFileByContract(int id, int serviceId)
        {
             FileResult result = null;

            await db.Contracts.FindAsync(id).ContinueWith((t) =>
            {
               var contract = t.Result;
               if (contract.Client.Id == User.Identity.GetUserId()) // Доступ к файлу имеет только выбранный рабочий
               {
                   var filePath = contract.Items.FirstOrDefault(i => i.Id == serviceId)?.File.PathFile;
                   if (filePath != null)
                   {
                       var file = System.IO.File.ReadAllBytes(filePath);
                       result = File(file, System.Net.Mime.MediaTypeNames.Application.Octet, filePath.Split('.').LastOrDefault());
                   }
                }
            });

            return result;
        }

        /// <summary>
        /// Подготовка страницы для вывода данных о выбранном договоре
        /// </summary>
        /// <param name="id"> Id договора </param>
        /// <returns></returns>
        public async Task<ActionResult> Details(int? id)
        {
            ActionResult result = HttpNotFound("Неверный id");

            var contract = await db.Contracts.FirstOrDefaultAsync(r => r.Id == id);
            if(contract != null)
            {
                result = View(contract);
            }

            return result;
        }

        /// <summary>
        /// Подготовка страницы для отображения договора со стороны пользователя
        /// </summary>
        /// <param name="id"> Id договора </param>
        /// <returns></returns>
        public async Task<ActionResult> UserDetails(int? id)
        {
            ActionResult result = HttpNotFound("Неверный id");

            var contract = await db.Contracts.FirstOrDefaultAsync(r => r.Id == id);

            if (contract != null)
            {
                result = View(contract);
            }

            return result;
        }
    }
}