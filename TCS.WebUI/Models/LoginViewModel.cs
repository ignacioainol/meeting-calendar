using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TCS.WebUI.Models
{
    public class LoginViewModel
    {
        public string Username { get; set; }
        //public string FullName { get; set; }
        [Required(ErrorMessage = "La contraseña es requerida.")]
        // [StringLength(255, ErrorMessage = "La contraseña debe tener almenos 5 caracteres.", MinimumLength = 5)]
        public string Password { get; set; }
    }
}
