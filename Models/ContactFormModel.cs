using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace TL4_SHOP.Models
{
    public class ContactFormModel
    {
        [Required]
        public string Name { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
