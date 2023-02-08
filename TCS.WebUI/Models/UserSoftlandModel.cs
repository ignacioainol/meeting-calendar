using System.Collections.Generic;

namespace TCS.WebUI.Models
{
    public class UserSoftlandModel
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public UserSoftlandData Data { get; set; }
    }

    public class UserSoftlandData
    {
        public string ficha { get; set; }
        public string rut {get; set;}
        public string nombre { get; set; }
        public string appaterno { get; set; }
        public string apmaterno { get; set; }
        public string descCC { get; set; }
        public string sexo { get; set; }
        public string direccion { get; set; }
        public string email { get; set; }
        public string telefono1 { get; set; }
        public string telefono2 { get; set; }
        public string estado { get; set; }
    }
}
