using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Helpers;
using ASaE.Controllers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace ASaE.Models
{
    public class BasketConfirmViewModel
    {
        public UserBasket UserBasket { get; set; }
        public string UserComment { get; set; }
    }
    public class CreateConsumableViewModel
    {
        public int EquipmentId { get; set; }
        public EquipmentConsumable EquipmentConsumable { get; set; }
    }
    public class ContractDetailsViewModel
    {
        public ICollection<Service> Services { get; set; }
        public IDictionary<Service,int> CostConsumables { get; set; }
        public ICollection<Equipment> Equipments { get; set; }
        public ApplicationUser User { get; set; }
        public ApplicationUser Employee { get; set; }
        public ContractStatus Status { get; set; }
        public Contract Contract { get; set; }
    }
    public class ServiceDetailsViewModel
    {
        public Service Service { get; set; }
        public bool IsContainsInBasket { get; set; }
        public int Count { get; set; }
    }
    public class ManageIndexViewModel
    {
        public ICollection<Contract> Contracts { get; set; }
        public ICollection<Request> Requests { get; set; }
        public ApplicationUser User { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
        public ApplicationUser User { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Значение {0} должно содержать символов не менее: {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение нового пароля")]
        [Compare("NewPassword", ErrorMessage = "Новый пароль и его подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Значение {0} должно содержать символов не менее: {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение нового пароля")]
        [Compare("NewPassword", ErrorMessage = "Новый пароль и его подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Номер телефона")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Код")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }
}