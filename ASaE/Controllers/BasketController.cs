using ASaE.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace ASaE.Controllers
{
    [Authorize]
    public class BasketController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationUser AppUser 
        { 
            get => UserManager.FindById(User.Identity.GetUserId()); 
        }
        public async Task<ActionResult> Index()
        {
            return View(new BasketConfirmViewModel { UserBasket = AppUser?.UserBasket });
        }

        /// <summary>
        /// Удаляем услугу из корзины
        /// </summary>
        /// <param name="id"> Id услуги </param>
        /// <returns></returns>
        public async Task<ActionResult> Delete(int? id)
        {
            ActionResult result = PartialView("Delete");

            var service = await db.Services.FindAsync(id);
            if (service != null)
            {
                await db.UserBaskets.FirstAsync(ub => ub.Id == AppUser.UserBasket.Id).ContinueWith(async (t) =>
                {
                    var userBasket = t.Result;
                    var userBasketItem = userBasket.Items.FirstOrDefault(ubi => ubi.Service == service);

                    userBasket.Items.Remove(userBasketItem);

                    await db.SaveChangesAsync();
                });
            }
            else
            {
                result = HttpNotFound("Несуществующий id");
            }

            return result;
        }

        /// <summary>
        /// Добавление услуги в корзину
        /// </summary>
        /// <param name="serviceDetails"> Вьюшка с параметрами добавления </param>
        /// <param name="fileObj"> Файл из запроса </param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddInBasket(ServiceDetailsViewModel serviceDetails, HttpPostedFileBase fileObj)
        {
            ActionResult result = RedirectToAction("Details", "Services", new { id = serviceDetails.Service.Id });

            var service = await db.Services.FindAsync(serviceDetails.Service.Id);
            if (service != null)
            {
                await db.UserBaskets.FirstAsync(ub => ub.Id == AppUser.UserBasket.Id).ContinueWith(async (t) => // Получение корзины пользователя
                {
                    var userBasket = t.Result;
                        FileItem newFileItem = null;

                    if (service.Type == ServiceType.Object && fileObj != null) // Проверяем явялется ли услуга для создания объекта
                    {
                        newFileItem = AddFileOnServer(fileObj, fileObj.FileName); // Добавляем на сервер файл

                        userBasket.Items.Add(new UserBasketItem
                        {
                            File = newFileItem,
                            Count = serviceDetails.Count,
                            Service = service
                        });

                        await db.SaveChangesAsync();
                    }
                    else if (service.Type == ServiceType.Training)
                    {
                        userBasket.Items.Add(new UserBasketItem
                        {
                            File = newFileItem,
                            Count = serviceDetails.Count,
                            Service = service
                        });

                        await db.SaveChangesAsync();
                    }
                });
            }
            else
            {
                result = HttpNotFound("Несуществующий id");
            }

            return result;
        }

        /// <summary>
        /// Добавление файла клиента на сервер
        /// </summary>
        /// <param name="file"> Файл из запроса </param>
        /// <param name="fileName"> Имя файла </param>
        /// <returns></returns>
        private FileItem AddFileOnServer(HttpPostedFileBase file, string fileName)
        {
            var userId = User.Identity.GetUserId();
            var directory = Directory.CreateDirectory(HostingEnvironment.ApplicationPhysicalPath + "/Content/userfiles/" + userId); // Создание директории для хранения файлов 
            var path = directory.FullName + "\\" + DateTime.Now.Ticks + fileName;

            using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite))
            {
                file.InputStream.CopyTo(fs); // Сохраняем файл на сервере
                return db.FileItems.Add(new FileItem  // Добавляем данные о файле в бд
                {
                    PathFile = path,
                    Size = file.ContentLength
                });
            }
        }
    }
}