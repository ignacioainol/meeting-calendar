using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TCS.WebUI.Models
{
    public class UserAdModel
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public int HttpCode { get; set; }
        public List<string> Errors { get; set; }
        public UserData Data { get; set; }
    }
    public class UserData
    {
        public string UserName { get; set; }
        public string Dni { get; set; }
        public string loginMethod { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public bool IsLocked { get; set; }
        public bool IsVerified { get; set; }
        public string JWToken { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}
