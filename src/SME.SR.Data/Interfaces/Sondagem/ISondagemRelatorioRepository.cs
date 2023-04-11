using System.Collections.Generic;
using System.Threading.Tasks;
using SME.SR.Infra;

namespace SME.SR.Data.Interfaces.Sondagem
{
    public interface ISondagemRelatorioRepository
    {
        Task<IEnumerable<OrdemPerguntaRespostaDto>> ConsolidadoCapacidadeLeitura(RelatorioPortuguesFiltroDto filtro);
        Task<IEnumerable<PerguntaRespostaProducaoTextoDto>> ObterDadosProducaoTexto(RelatorioPortuguesFiltroDto filtro);
        Task<IEnumerable<PerguntaRespostaOrdemDto>> ConsolidacaoCampoAditivoMultiplicativo(RelatorioMatematicaFiltroDto filtro);
    }
}