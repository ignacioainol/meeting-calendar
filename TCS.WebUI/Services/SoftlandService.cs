using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using TCS.WebUI.Interface;
using TCS.WebUI.Models;

namespace TCS.WebUI.Services
{
    public class SoftlandService : ISoftlandService
    {
        //private readonly JWTSettings _jwtSettings;
        //private readonly TJSSettings _tjsSettings;
        //private IMemoryCache _memoryCache;
        //public SoftlandService(IOptions<JWTSettings> jwtSettings, IOptions<TJSSettings> tjsSettings, IMemoryCache memoryCache)
        //{
        //    _jwtSettings = jwtSettings.Value;
        //    _tjsSettings = tjsSettings.Value;
        //    _tjsSettings.Resource = "VerifyUserJSON";
        //    _memoryCache = memoryCache;
        //}

        public async Task<dynamic> GETCurrentEmployee()
        {
            HttpRequestMessage requestSoftlandUser = new HttpRequestMessage(HttpMethod.Get, $"http://crsws04/softland/api/v1/Persona/list");
            HttpClient httpClientSoftlandUser = new HttpClient();
            httpClientSoftlandUser.DefaultRequestHeaders.Add("APIKEY", "test123");
            HttpResponseMessage responseSoftlandUser = await httpClientSoftlandUser.SendAsync(requestSoftlandUser).ConfigureAwait(false);

            string jsongetSoftlandUser = await responseSoftlandUser.Content.ReadAsStringAsync().ConfigureAwait(false);
            var resultSoftland = JsonConvert.DeserializeObject<dynamic>(jsongetSoftlandUser);

            return resultSoftland;
        }

        public async Task<dynamic> GETEmployeeRut(string rut)
        {
            HttpRequestMessage requestSoftlandUser = new HttpRequestMessage(HttpMethod.Get, $"http://crsws04/softland/api/v1/Persona/"+rut);
            HttpClient httpClientSoftlandUser = new HttpClient();
            httpClientSoftlandUser.DefaultRequestHeaders.Add("APIKEY", "test123");
            HttpResponseMessage responseSoftlandUser = await httpClientSoftlandUser.SendAsync(requestSoftlandUser).ConfigureAwait(false);

            string jsongetSoftlandUser = await responseSoftlandUser.Content.ReadAsStringAsync().ConfigureAwait(false);
            var resultSoftland = JsonConvert.DeserializeObject<dynamic>(jsongetSoftlandUser);

            return resultSoftland;
        }

        //Task<dynamic> ISoftlandService.GETEmployeeRut(string rut)
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}
