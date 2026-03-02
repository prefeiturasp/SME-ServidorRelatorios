using SME.SR.Infra.Dtos.NovoSondagem;
using System.Threading.Tasks;

namespace SME.SR.Infra.Interfaces.Integracoes
{
    public interface ISondagemApiClient
    {
        Task<RetornoApiSondagemQuestionarioDto>
            ObterDadosQuestionarioAsync(FiltroRelatorioSondagemQuestionarioDto filtro);
    }
}