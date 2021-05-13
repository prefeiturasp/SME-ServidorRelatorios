using SME.SR.Infra;
using System.Threading.Tasks;

namespace SME.SR.Application.Interfaces
{
    public interface IRelatorioAcompanhamentoAprendizagemUseCase 
    {
        Task<RelatorioAcompanhamentoAprendizagemDto> Executar(FiltroRelatorioDto filtro);
    }
}