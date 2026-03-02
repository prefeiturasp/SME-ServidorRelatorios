using System.Threading.Tasks;

namespace SME.SR.Infra.Interfaces.Integracoes
{
    public interface ISolicitacaoRelatorioApiClient
    {
        Task FinalizarSolicitacaoAsync(int solicitacaoRelatorioId);
    }
}