using System.Threading.Tasks;

namespace SME.SR.JRSClient.Interfaces
{
    public interface ILoginService
    {
        Task<string> ObterTokenAutenticacao(string login, string senha);
        Task<string> ObterReportStatus();
    }
}
