using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Runtime.ExceptionServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ASaE.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Address { get; set; }
        public virtual UserBasket UserBasket { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }
    public class FileItem
    {
        public int Id { get; set; }
        public string PathFile { get; set; }
        public int Size { get; set; }
        public virtual ICollection<ObjectItem> ObjectItems { get; set; }
    }
    public class ObjectItem
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public virtual Service Service { get; set; }
        public virtual FileItem File { get; set; }
    }
    public enum ServiceType
    {
        [Description("Предмет")]
        Object,
        [Description("Обучение")]
        Training
    }
    public class Employee
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

    public class UserBasket
    {
        public int Id { get; set; }
        public virtual ICollection<UserBasketItem> Items { get; set; }
        public virtual ApplicationUser User { get; set; }
        public UserBasket()
        {
            Items = new List<UserBasketItem>();
        }
    }
    public class UserBasketItem : ObjectItem
    {
        public virtual UserBasket UserBasket { get; set; }
    }
    public class Service
    {
        public int Id { get; set; }

        [Required, Display(Name = "Имя")]
        public string Name { get; set; }
        [Required, Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Иллюстрация услуги")]
        public byte[] Picture { get; set; }
        [Required, Display(Name = "Цена")]
        public int Cost { get; set; }
        [Required, Display(Name = "Тип услуги")]
        public ServiceType Type { get; set; }
        [Display(Name = "Связанное оборудование")]
        public virtual Equipment Equipment { get; set; }
        public virtual ICollection<UserBasket> UserBaskets { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public Service()
        {
            UserBaskets = new List<UserBasket>();
            Contracts = new List<Contract>();
        }
    }

    public enum EquipmentType
    {
        LaserMachine,
        Plotter,
        Printer,
        Thermopress,
        Miller,
        Other
    }
    public class Equipment
    {
        public int Id { get; set; }
        [Required, Display(Name = "Наименование")]
        public string Name { get; set; }
        [Required, Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Сотрудник отвечающий за оборудование")]
        public virtual ApplicationUser EquipmentManager { get; set; }
        [Display(Name = "Расходные материалы")]
        public virtual ICollection<EquipmentConsumable> EquipmentConsumables { get; set; }
        [Required, Display(Name = "Тип оборудования")]
        public EquipmentType Type { get; set; }
        public virtual ICollection<Service> Services { get; set; }
        public Equipment()
        {
            Services = new List<Service>();
            EquipmentConsumables = new List<EquipmentConsumable>();
        }
    }
    public class EquipmentConsumable
    {
        public int Id { get; set; }
        [Required, Display(Name = "Имя")]
        public string Name { get; set; }
        [Required, Display(Name = "Описание")]
        public string Description { get; set; }
        [Required, Display(Name = "Значение")]
        public int Value { get; set; }
        [Required, Display(Name = "Максимальное количество ресурса")]
        public int MaxValue { get; set; }
        public virtual ICollection<Equipment> Equipments { get; set; }
        public EquipmentConsumable()
        {
            Equipments = new List<Equipment>();
        }
    }

    public enum ContractStatus
    {
        [Display(Name = "Ожидает")]
        Waiting,
        [Display(Name = "Готов")]
        Ready,
        [Display(Name = "Отмемён")]
        Canceled
    }
    public class Contract
    {
        public int Id { get; set; }
        public virtual ApplicationUser Client { get; set; }
        public virtual ApplicationUser Employee { get; set; }
        public ContractStatus Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }
        public string UserComment { get; set; }
        public virtual ICollection<ContractItem> Items { get; set; }
        public Contract()
        {
            Items = new List<ContractItem>();
        }
    }
    public class ContractItem : ObjectItem
    {
        public virtual Contract Contract { get; set; }
    }
    public enum RequestStatus
    {
        [Display(Name = "Ожидает")]
        Waiting,
        [Display(Name = "Готов")]
        Ready,
        [Display(Name = "Отмемён")]
        Canceled
    }
    public class Request
    {
        public int Id { get; set; }
        public virtual ApplicationUser Client { get; set; }
        public RequestStatus Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }
        public string UserComment { get; set; }
        public virtual ICollection<RequestItem> Items { get; set; }
        public Request()
        {
            Items = new List<RequestItem>();
        }
    }
    public class RequestItem : ObjectItem
    {
        public virtual Request Request { get; set; }
    }
    public class ModifiedValueConsumable
    {
        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }
        public int OldValue { get; set; }
        public int NewValue { get; set; }
        public virtual EquipmentConsumable EquipmentConsumable { get; set; }
    }
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Service> Services { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<EquipmentConsumable> EquipmentConsumables { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<UserBasket> UserBaskets { get; set; }
        public DbSet<FileItem> FileItems { get; set; }
        public DbSet<UserBasketItem> UserBasketItems { get; set; }
        public DbSet<ObjectItem> ObjectItems { get; set; }
        public DbSet<RequestItem> RequestItems { get; set; }
        public DbSet<ContractItem> ContractItems { get; set; }
        public DbSet<ModifiedValueConsumable> ModifiedValueConsumables { get; set; }
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>()
                .HasOptional(u => u.UserBasket)
                .WithRequired(ub => ub.User);

            base.OnModelCreating(modelBuilder);
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}