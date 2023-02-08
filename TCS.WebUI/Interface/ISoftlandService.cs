using System.Threading.Tasks;
using TCS.WebUI.Models;

namespace TCS.WebUI.Interface
{
    public interface ISoftlandService
    {
        Task<dynamic> GETCurrentEmployee();
        Task<dynamic> GETEmployeeRut(string rut);
    }
}
