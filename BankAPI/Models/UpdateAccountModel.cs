using BankAPI.Models.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace BankAPI.Models
{
    public class UpdateAccountModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]/d{4}$", ErrorMessage = "Pin must not be more than 4 digits")]
        public string Pin { get; set; }
        [Required]
        [Compare("Pin", ErrorMessage = "Pin not match")]
        public string ConfirmPin { get; set; }
        public DateTime DateLastUpdated { get; set; }

    }
}
