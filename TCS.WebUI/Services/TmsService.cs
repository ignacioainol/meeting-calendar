using System.Threading.Tasks;
using TCS.WebUI.Interface;
using TCS.WebUI.Models;

namespace TCS.WebUI.Services
{
    public class TmsService : ITmsService
    {
        public TmsService() { }

        public Task<PieceModel> GetPieceAsync(string pieceNo)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UpdatePieceProgress(PieceModel piece)
        {
            throw new System.NotImplementedException();
        }
    }
}
