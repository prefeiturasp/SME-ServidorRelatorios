﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SME.SR.Infra;

namespace SME.SR.Data.Interfaces.Sondagem
{
    public interface ISondagemRelatorioRepository
    {
        Task<IEnumerable<OrdemPerguntaRespostaDto>> ConsolidadoCapacidadeLeitura(RelatorioPortuguesFiltroDto filtro);
        Task<IEnumerable<PerguntaRespostaProducaoTextoDto>> ObterDadosProducaoTexto(RelatorioPortuguesFiltroDto filtro);
        Task<IEnumerable<PerguntaRespostaOrdemDto>> MatematicaIADNumeroBimestre(int anoLetivo, string componenteCurricularId, int bimestre, string codigoUe, string codigoDre, ProficienciaSondagemEnum proeficienciaSondagem = ProficienciaSondagemEnum.IAD);
        Task<IEnumerable<PerguntaRespostaOrdemDto>> MatematicaIADAntes2022(int anoLetivo, string componenteCurricularId, int bimestre, string codigoUe, string codigoDre, string periodoId);
        Task<IEnumerable<PerguntaRespostaOrdemDto>> MatematicaNumerosAntes2022(int anoLetivo, int semestre, string codigoUe, string codigoDre, string periodoId);
        Task<IEnumerable<PerguntaRespostaOrdemDto>> ConsolidacaoCampoAditivoMultiplicativo(RelatorioMatematicaFiltroDto filtro);
        Task<IEnumerable<PerguntaRespostaOrdemDto>> ConsolidacaoCampoAditivoMultiplicativoAntes2022(RelatorioMatematicaFiltroDto filtro);
    }
}