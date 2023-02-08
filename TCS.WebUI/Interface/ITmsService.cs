using INVENTORY.EF.Objetos;
using System.Threading.Tasks;
using TCS.WebUI.Models;

namespace TCS.WebUI.Interface
{
    public interface ITmsService
    {
        Task<PieceModel> GetPieceAsync(string pieceNo);

        Task<bool> UpdatePieceProgress(PieceModel piece);
    }
}
